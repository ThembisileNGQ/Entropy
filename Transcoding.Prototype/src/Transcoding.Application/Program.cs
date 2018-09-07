using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Transcoding.Transcoder.Actors;
using Transcoding.Transcoder.Model;
using Transcoding.Transcoder.Options;

namespace Transcoding.Application
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            
            var input = Path.Combine(Environment.CurrentDirectory, "0.wav");

            var outputs = Enumerable
                .Range(1, 4)
                .Select(x => Path.Combine(Environment.CurrentDirectory, $"{Guid.NewGuid()}.mp3"))
                .ToArray();

            await TranscodeAudio(input, outputs);
        }
        
        public static async Task TranscodeAudio(string sourceFilePath,  params string[] destinationPaths)
        {
            var actorSystem = ActorSystem.Create("transcoding-system");

            var options = new ConversionOptions
            {
                AudioBitRate = 320
            };
    
            
            var ffmpegPath = Path.Combine(@"C:\Workspace\FFMPEG\bin\ffmpeg.exe");

            var inputFile = new MediaFile(sourceFilePath);
            var tasks = new List<Task<Done>>();

            
            var i = 0;
            foreach (var path in destinationPaths)
            {
                i++;
                var outputFile = new MediaFile(path);
                var transcoder = actorSystem.ActorOf(Props.Create(() => new TranscodingActor(inputFile, outputFile, options, ffmpegPath)), $"transcododer-{i}");
                tasks.Add(transcoder.Ask<Done>(new Start(), TimeSpan.FromMinutes(1)));
            }

            var stopWatch = Stopwatch.StartNew();

            var b = await Task.WhenAll(tasks);
            
            stopWatch.Stop();
            
        }
    }
}