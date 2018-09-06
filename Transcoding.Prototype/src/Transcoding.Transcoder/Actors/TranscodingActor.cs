using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Akka.Actor;
using Akka.Event;
using Transcoding.Transcoder.Model;
using Transcoding.Transcoder.Options;

namespace Transcoding.Transcoder.Actors
{
    public class TranscodingActor : ReceiveActor
    {
        public MediaFile Input { get; set; }
        public MediaFile Output { get; set; }
        public ConversionOptions ConversionOptions { get; set; }
        private readonly string _ffmpegPath;  
        public Process TranscodingProcess { get; private set; }
        List<string> ReceivedMessagesLog = new List<string>();
        private readonly ILoggingAdapter _logger;
        public EngineParameters EngineParameters { get; }

        public TranscodingActor(
            MediaFile input,
            MediaFile output,
            string ffmpegPath,
            ConversionOptions options)
        {
            Input = input;
            Output = output;
            ConversionOptions = options;
            _ffmpegPath = ffmpegPath;
            _logger = Context.GetLogger();
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
            return true;
        }

        private void StartProcess(EngineParameters engineParameters)
        {
            List<string> receivedMessagesLog = new List<string>();
            TimeSpan totalMediaDuration = new TimeSpan();

            ProcessStartInfo processStartInfo = engineParameters.HasCustomArguments
                ? this.GenerateStartInfo(engineParameters.CustomArguments)
                : this.GenerateStartInfo(engineParameters);
        }

        private void RunProcess(ProcessStartInfo processStartInfo)
        {
            using (TranscodingProcess = Process.Start(processStartInfo))
            {
                TranscodingProcess.ErrorDataReceived += Handle;
            }
        }

        private void Handle(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null) return;
            Console.WriteLine(e.Data);

            try
            {
                ReceivedMessagesLog.Insert(0, e.Data);
                
                if (engineParameters.InputFile != null)
                {
                    RegexEngine.TestVideo(received.Data, engineParameters);
                    RegexEngine.TestAudio(received.Data, engineParameters);

                    Match matchDuration = RegexEngine.Index[RegexEngine.Find.Duration].Match(received.Data);
                    if (matchDuration.Success)
                    {
                        if (engineParameters.InputFile.Metadata == null)
                        {
                            engineParameters.InputFile.Metadata = new Metadata();
                        }

                        TimeSpan.TryParse(matchDuration.Groups[1].Value, out totalMediaDuration);
                        engineParameters.InputFile.Metadata.Duration = totalMediaDuration;
                    }
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
