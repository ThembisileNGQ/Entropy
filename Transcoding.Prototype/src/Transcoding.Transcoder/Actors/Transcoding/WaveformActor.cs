using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Akka.Actor;
using Akka.Event;
using Transcoding.Transcoder.Actors.Transcoding.Commands;
using Transcoding.Transcoder.Model;
using Transcoding.Transcoder.Options;
using Transcoding.Transcoder.Util;

namespace Transcoding.Transcoder.Actors.Transcoding
{
    public class WaveformActor : ReceiveActor
    {
        private readonly string _ffmpegPath;
        private readonly ILoggingAdapter _logger;
        public IActorRef Invoker { get; private set; }
        public Status Status { get; private set; }
        public Guid TranscodingId { get; }
        public MediaFile Input { get; }
        public MediaFile Output { get; }
        public Process TranscodingProcess { get; private set; }
        public ConversionOptions ConversionOptions { get; set; }
        public EngineParameters EngineParameters { get; set; }
        public TimeSpan TotalMediaDuration { get; private set; }
        public TimeSpan ProcessedMediaDuration { get; private set; }
        public TimeSpan TotalProcessDuration { get; private set; }
        public DateTime StartedAt { get; private set; }
        public DateTime EndedAt { get; private set; }
        public Stopwatch Stopwatch { get; private set; }
        public List<string> ReceivedMessagesLog { get; private set; }
        public List<Exception> Exceptions { get; private set; }


        public WaveformActor(
            Guid transcodingId,
            MediaFile input,
            MediaFile output,
            ConversionOptions options,
            string ffmpegPath)
        {
            TranscodingId = transcodingId;
            Input = input;
            Output = output;
            ConversionOptions = options;
            _ffmpegPath = ffmpegPath;
            _logger = Context.GetLogger();
            ReceivedMessagesLog = new List<string>();
            Exceptions = new List<Exception>();
            Status = Status.NotStarted;
            EngineParameters  = new EngineParameters
            {
                InputFile = Input,
                OutputFile = Output,
                ConversionOptions = ConversionOptions,
                Task = FFmpegTask.GetWaveform
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
                Status = Status.InProgress;
                var invoker = Invoker;

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
                            ProcessedMediaDuration = progressEvent.ProcessedDuration;
                            
                            var command = new ReportTranscodingProgress(
                                TranscodingId,
                                TotalMediaDuration,
                                ProcessedMediaDuration,
                                elapsed,
                                StartedAt,
                                Input,
                                Output);
                            
                            invoker.Tell(command);
                        }
                        else if (RegexEngine.IsConvertCompleteData(e.Data, out convertCompleted))
                        {
                            //does this even work?
                            var a = Stopwatch.Elapsed;
                            //_logger.Info($"Progress: Done!");
                        }
        
                    }
                    catch (Exception ex)
                    {
                        // catch the exception and kill the process since we're in a faulted state
                        Exceptions.Add(ex);
                        
                        try
                        {
                            Status = Status.Failed;
                            TranscodingProcess.Kill();
                        }
                        catch (InvalidOperationException invalidOperationException)
                        {
                            Exceptions.Add(invalidOperationException);
                            // swallow exceptions that are thrown when killing the process, 
                            // one possible candidate is the application ending naturally before we get a chance to kill it
                        }
                        finally
                        {
                            
                        }
                    }
                };
                

                TranscodingProcess.BeginErrorReadLine();
                TranscodingProcess.WaitForExit();
                
                

                if ((TranscodingProcess.ExitCode != 0 && TranscodingProcess.ExitCode != 1) || Exceptions.Count > 0)
                {
                    Status = Status.Failed;
                    throw new Exception(
                        TranscodingProcess.ExitCode + ": " + ReceivedMessagesLog[1] + ReceivedMessagesLog[0]);
                }
                else
                {
                    Status = Status.Completed;

                }

                EndedAt = DateTime.UtcNow;
                TotalProcessDuration = Stopwatch.Elapsed;
                Stopwatch.Stop();


                switch (Status)
                {

                    case Status.Completed:
                    {
                        var command = new ReportTranscodingCompletion(
                            TranscodingId,
                            TotalMediaDuration,
                            ProcessedMediaDuration,
                            TotalProcessDuration,
                            StartedAt,
                            EndedAt,
                            Input,
                            Output,
                            ReceivedMessagesLog);

                        invoker.Tell(command);
                    }
                        break;
                    case Status.Failed:
                    {
                        var command = new ReportTranscodingFailure(
                            TranscodingId,
                            TotalMediaDuration,
                            TotalProcessDuration,
                            StartedAt,
                            EndedAt,
                            Input,
                            Output,
                            Exceptions,
                            ReceivedMessagesLog);

                        invoker.Tell(command);
                        }
                        break;
                }

                
                Context.Stop(Self);
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
