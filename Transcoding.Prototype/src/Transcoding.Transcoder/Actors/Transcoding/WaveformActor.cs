using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Akka.Actor;
using Akka.Event;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
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
            EngineParameters = new EngineParameters(ConversionOptions, Input, Output, FFmpegTask.AnalyseAudio);
            
            Receive<Start>(Handle);
            Receive<GetWaveformData>(Handle);
            
        }

        public bool Handle(GetWaveformData command)
        {
            using(var fs = File.Create("/Users/lutandongqakaza/Workspace/Entropy/Transcoding.Prototype/media/0.png"))
            using (var bmp = Image.Load(Output.Filename).CloneAs<Rgba32>())
            {
                var waveform = new List<int>();

                var pic = new Image<Rgba32>(bmp.Width, bmp.Height / 2);
                var clearColor = Rgba32.Black;
                clearColor.A = 0;
                
                for (var i = 0; i < pic.Width; i++)
                {
                    for (var j = 0; j < pic.Height; j++)
                    {
                        var idx = (int) j + bmp.Height / 2;
                        var a = bmp[i, idx];
                        if (bmp[i, idx] == clearColor)
                        {
                            pic[i, j] = Rgba32.Blue;
                            waveform.Add(j);
                            j = bmp.Height;

                        }
                        else
                        {
                            pic[i, j] = Rgba32.Blue;
                        }

                    }
                }
                pic.SaveAsPng(fs);

            }
            
            return true;
        }
        public bool Handle(Start command)
        {
            Invoker = Sender;
            StartProcess(EngineParameters);
            
            return true;
        }
        
        
        private void StartProcess(EngineParameters engineParameters)
        {

            var processStartInfo = GenerateStartInfo(engineParameters);

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
                            //RegexEngine.TestAudio(e.Data, EngineParameters);
        
                            var matchDuration = RegexEngine.Index[RegexEngine.Find.Duration].Match(e.Data);
                            if (matchDuration.Success)
                            {
                                var totalMediaDuration = new TimeSpan();
                                TimeSpan.TryParse(matchDuration.Groups[1].Value, out totalMediaDuration);
                                TotalMediaDuration = totalMediaDuration;
                                //EngineParameters.InputFile.Metadata.Duration = TotalMediaDuration;
                            }
                        }
                        ConversionCompleted convertCompleted;
                        ConversionProgressed progressEvent;
        
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

                Self.Tell(new GetWaveformData());
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
            var arguments = CommandBuilder.Serialize(engineParameters);

            return this.GenerateStartInfo(arguments);
        }
        
        public class GetWaveformData{}
        
        public static Image<Rgba32> InitPic(Image<Rgba32> bmp)
        {
            for (var i = 0; i < bmp.Width; i++)
            {
                for (var j = 0; j < bmp.Height; j++)
                {
                    bmp[i, j] = Rgba32.White;
                }
            }
            return bmp;
        }
    }
}
