using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Transcoding.Transcoder.Actors.Transcoding;
using Transcoding.Transcoder.Model;
using Transcoding.Transcoder.Options;

namespace Transcoding.Application
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            if (true) //windoze
            {
                var ffmpegPath = Path.Combine(@"C:\Workspace\FFMPEG\bin\ffmpeg.exe");
                var input = Path.Combine(Environment.CurrentDirectory, "0.wav");

                var outputs = Enumerable
                    .Range(1, 4)
                    .Select(x => Path.Combine(Environment.CurrentDirectory, $"{Guid.NewGuid()}.mp3"))
                    .ToArray();

                await TranscodeAudio(ffmpegPath,input, outputs);
            } 
            else if (false) //mac
            {
                var ffmpegPath = Path.Combine(@"/usr/local/bin/ffmpeg");
                var input = Path.Combine(Environment.CurrentDirectory, "../../media/0.wav");

                var outputs = Enumerable
                    .Range(1, 2)
                    .Select(x => Path.Combine(Environment.CurrentDirectory, $"../../media/{Guid.NewGuid()}.mp3"))
                    .ToArray();

                await TranscodeAudio(ffmpegPath,input, outputs);
            }
            
        }
        
        public static async Task TranscodeAudio(string ffmpegPath, string sourceFilePath,  params string[] destinationPaths)
        {
            var config = @"akka.loglevel = DEBUG
                           akka.actor.debug.receive = on
                           akka.actor.debug.unhandled = on
                           akka.actor.debug.event-stream = on
                           akka.stdout-loglevel = Debug";
            var actorSystem = ActorSystem.Create("transcoding-system", config);

            var options = new ConversionOptions
            {
                AudioBitRate = 320
            };

            var inputFile = new MediaFile(sourceFilePath);
            var tasks = new List<Task<Done>>();

            
            var i = 0;
            foreach (var path in destinationPaths)
            {
                i++;
                var outputFile = new MediaFile(path);
                var transcoder = actorSystem.ActorOf(Props.Create(() => new TranscodingActor(Guid.NewGuid(),  inputFile, outputFile, options, ffmpegPath)), $"transcododer-{i}");
                tasks.Add(transcoder.Ask<Done>(new Start(), TimeSpan.FromMinutes(1)));
            }

            var stopWatch = Stopwatch.StartNew();

            var b = await Task.WhenAll(tasks);
            
            stopWatch.Stop();
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}