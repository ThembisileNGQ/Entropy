using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Akka.Actor;
using Akka.Event;
using Transcoding.Transcoder.Model;
using Transcoding.Transcoder.Options;
using Transcoding.Transcoder.Util;

namespace Transcoding.Transcoder.Actors
{
    public class TranscodingActor : ReceiveActor
    {
        private readonly string _ffmpegPath;
        private readonly ILoggingAdapter _logger;
        private readonly string _name;
        public MediaFile Input { get; set; }
        public MediaFile Output { get; set; }
        public Process TranscodingProcess { get; private set; }
        public ConversionOptions ConversionOptions { get; set; }
        public EngineParameters EngineParameters { get; set; }
        public TimeSpan TotalMediaDuration { get; private set; }
        public Exception PossibleException { get; private set; }
        List<string> ReceivedMessagesLog = new List<string>();
        
        public TranscodingActor(
            MediaFile input,
            MediaFile output,
            ConversionOptions options,
            string ffmpegPath = @"C:\Workspace\FFMPEG\bin\ffmpeg.exe")
        {
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

            StartProcess(EngineParameters);
            Sender.Tell(new Done());
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
                TranscodingProcess.ErrorDataReceived += Handle;

                TranscodingProcess.BeginErrorReadLine();
                TranscodingProcess.WaitForExit();

                if ((TranscodingProcess.ExitCode != 0 && TranscodingProcess.ExitCode != 1) || PossibleException != null)
                {
                    throw new Exception(
                        TranscodingProcess.ExitCode + ": " + ReceivedMessagesLog[1] + ReceivedMessagesLog[0],
                        PossibleException);
                }
            }
        }

        public void Handle(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;


            //Console.WriteLine(e.Data);

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
                    //progress calculated here
                    progressEvent.TotalDuration = TotalMediaDuration;
                    var progress = (double)progressEvent.ProcessedDuration.Ticks / (double)TotalMediaDuration.Ticks;
                    _logger.Info($"{_name}: Progress: {progress} %" );
                    //this.OnProgressChanged(progressEvent);
                }
                else if (RegexEngine.IsConvertCompleteData(e.Data, out convertCompleted))
                {
                    convertCompleted.TotalDuration = TotalMediaDuration;
                    _logger.Info($"Progress: Done!");
                    //this.OnConversionComplete(convertCompleteEvent);
                }

            }
            catch (Exception ex)
            {
                // catch the exception and kill the process since we're in a faulted state
                PossibleException = ex;

                try
                {
                    TranscodingProcess.Kill();
                }
                catch (InvalidOperationException)
                {
                    // swallow exceptions that are thrown when killing the process, 
                    // one possible candidate is the application ending naturally before we get a chance to kill it
                }
            }
            
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
