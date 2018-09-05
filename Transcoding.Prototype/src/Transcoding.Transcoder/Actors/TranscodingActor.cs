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
        private readonly ILoggingAdapter _logger;

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
            Receive<Start>(Handle);
        }

        public bool Handle(Start command)
        {
            EngineParameters engineParams = new EngineParameters
            {
                InputFile = Input,
                OutputFile = Output,
                ConversionOptions = ConversionOptions,
                Task = FFmpegTask.Convert
            };

            StartProcess(engineParams);
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
