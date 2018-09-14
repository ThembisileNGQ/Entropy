using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using Akka.Actor;
using Akka.Event;
using Transcoding.Transcoder.Actors.Transcoding.Commands;
using Transcoding.Transcoder.Model;
using Transcoding.Transcoder.Options;
using Transcoding.Transcoder.Util;

namespace Transcoding.Transcoder.Actors.Transcoding
{
    public class TranscodingActor : ReceiveActor
    {
        private readonly string _ffmpegPath;
        private readonly ILoggingAdapter _logger;
        private readonly string _name;
        public Guid TranscodingId { get; }
        public IActorRef Invoker { get; private set; }
        public MediaFile Input { get; }
        public MediaFile Output { get; }
        public Process TranscodingProcess { get; private set; }
        public ConversionOptions ConversionOptions { get; set; }
        public EngineParameters EngineParameters { get; set; }
        public TimeSpan TotalMediaDuration { get; private set; }
        public TimeSpan TimeElapsedSinceStart { get; private set; }
        public DateTime StartedAt { get; private set; }
        public DateTime EndedAt { get; private set; }
        public Stopwatch Stopwatch { get; private set; }
        public Exception PossibleException { get; private set; }
        List<string> ReceivedMessagesLog = new List<string>();
        
        public TranscodingActor(
            Guid transcodingId,
            MediaFile input,
            MediaFile output,
            ConversionOptions options,
            string ffmpegPath = @"C:\Workspace\FFMPEG\bin\ffmpeg.exe")
        {
            TranscodingId = transcodingId;
            Input = input;
            Output = output;
            ConversionOptions = options;
            _ffmpegPath = ffmpegPath;
            _logger = Context.GetLogger();
            _name = Context.Self.Path.Name;
            EngineParameters  = new EngineParameters
            {
                InputFile = Input,
                OutputFile = Output,
                ConversionOptions = ConversionOptions,
                Task = FFmpegTask.Convert
            };
            
            Receive<Start>(Handle);
            
        }

        public bool Handle(Start command)
        {
            Invoker = Sender;
            StartProcess(EngineParameters);
            
            return true;
        }
        
        
        private void StartProcess(EngineParameters engineParameters)
        {

            var processStartInfo = engineParameters.HasCustomArguments
                ? this.GenerateStartInfo(engineParameters.CustomArguments)
                : this.GenerateStartInfo(engineParameters);

            RunProcess(processStartInfo);
        }

        private void RunProcess(ProcessStartInfo processStartInfo)
        {
            
            using (TranscodingProcess = Process.Start(processStartInfo))
            {
                StartedAt = DateTime.UtcNow;
                Stopwatch = Stopwatch.StartNew();
                var invoker = Invoker;
                var logger = _logger;
                TranscodingProcess.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                        return;
        
                    try
                    {
                        ReceivedMessagesLog.Insert(0, e.Data);
                        
                        if (EngineParameters.InputFile != null)
                        {
                            RegexEngine.TestVideo(e.Data, EngineParameters);
                            RegexEngine.TestAudio(e.Data, EngineParameters);
        
                            Match matchDuration = RegexEngine.Index[RegexEngine.Find.Duration].Match(e.Data);
                            if (matchDuration.Success)
                            {
                                if (EngineParameters.InputFile.Metadata == null)
                                {
                                    EngineParameters.InputFile.Metadata = new Metadata();
                                }
                                var totalMediaDuration = new TimeSpan();
                                TimeSpan.TryParse(matchDuration.Groups[1].Value, out totalMediaDuration);
                                TotalMediaDuration = totalMediaDuration;
                                EngineParameters.InputFile.Metadata.Duration = TotalMediaDuration;
                            }
                        }
                        ConversionCompleted convertCompleted;
                        ConvertProgressEmitted progressEvent;
        
                        if (RegexEngine.IsProgressData(e.Data, out progressEvent))
                        {
                            var elapsed = Stopwatch.Elapsed;
                            
                            var command = new ReportTranscodingProgress(
                                TranscodingId,
                                TotalMediaDuration,
                                progressEvent.ProcessedDuration,
                                elapsed,
                                Input,
                                Output);
                            
                            invoker.Tell(command);
                        }
                        else if (RegexEngine.IsConvertCompleteData(e.Data, out convertCompleted))
                        {
                            var elapsed = Stopwatch.Elapsed;
                            
                            
                            var command2 = new ReportTranscodingCompletion(
                                TranscodingId,
                                TotalMediaDuration,
                                convertCompleted.ProcessedDuration,
                                elapsed,
                                elapsed,
                                Input,
                                Output);
                            
                            invoker.Tell(command2);
                            
                            convertCompleted.TotalDuration = TotalMediaDuration;
                            
                            //_logger.Info($"Progress: Done!");
                        }
        
                    }
                    catch (Exception ex)
                    {
                        // catch the exception and kill the process since we're in a faulted state
                        PossibleException = ex;
                        var elapsed = Stopwatch.Elapsed;
                        Stopwatch.Stop();
                        
                        
                        var command3 = new ReportTranscodingFailure(
                            TranscodingId,
                            TotalMediaDuration,
                            elapsed,
                            e.Data,
                            ex,
                            elapsed,
                            Input,
                            Output);
                        
                        invoker.Tell(command3);
                        
                        try
                        {
                            
                            TranscodingProcess.Kill();
                        }
                        catch (InvalidOperationException)
                        {
                            // swallow exceptions that are thrown when killing the process, 
                            // one possible candidate is the application ending naturally before we get a chance to kill it
                        }
                        finally
                        {
                            
                        }
                    }
                };
                //TranscodingProcess.ErrorDataReceived += Handle;
                EndedAt = DateTime.UtcNow;
                TranscodingProcess.BeginErrorReadLine();
                TranscodingProcess.WaitForExit();
                
                

                if ((TranscodingProcess.ExitCode != 0 && TranscodingProcess.ExitCode != 1) || PossibleException != null)
                {
                    throw new Exception(
                        TranscodingProcess.ExitCode + ": " + ReceivedMessagesLog[1] + ReceivedMessagesLog[0],
                        PossibleException);
                }
                
                //Context.Stop(Self);
            }
        }

        public void Handle(object sender, DataReceivedEventArgs e)
        {
            
            
        }

        private void EnsureFFmpegFileExists(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{path}");
            }
        }

        private ProcessStartInfo GenerateStartInfo(string arguments)
        {
            //win-x64
            if (Path.DirectorySeparatorChar == '\\')
            {
                return new ProcessStartInfo
                {
                    Arguments = "-nostdin -y -loglevel info " + arguments,
                    FileName = _ffmpegPath,
                    CreateNoWindow = true,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
            }
            else //linux-x64
            {
                return new ProcessStartInfo
                {
                    Arguments = "-y -loglevel info " + arguments,
                    FileName = _ffmpegPath,
                    CreateNoWindow = true,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
            }
        }

        private ProcessStartInfo GenerateStartInfo(EngineParameters engineParameters)
        {
            string arguments = CommandBuilder.Serialize(engineParameters);

            return this.GenerateStartInfo(arguments);
        }
    }
}
