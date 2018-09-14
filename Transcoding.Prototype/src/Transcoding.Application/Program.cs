using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Akka.Actor;
using Transcoding.Transcoder.Actors.Transcoding;
using Transcoding.Transcoder.Actors.Transcoding.Commands;
using Transcoding.Transcoder.Actors.Transcoding.Responses;
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
                    .Range(1, 6)
                    .Select(x => Path.Combine(Environment.CurrentDirectory, $"{Guid.NewGuid()}.mp3"))
                    .ToArray();

                await TranscodeAudio(ffmpegPath,input, outputs);
            } 
            else if (false) //mac
            {
                var ffmpegPath = Path.Combine(@"/usr/local/bin/ffmpeg");
                var input = Path.Combine(Environment.CurrentDirectory, "../../media/0.wav");

                var outputs = Enumerable
                    .Range(1, 1)
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

            var transcoderManager = actorSystem.ActorOf(Props.Create(() => new TranscodingManager()));
            
            foreach (var path in destinationPaths)
            {
                var outputFile = new MediaFile(path);
                await Task.Delay(1000);
                var command = new StartTranscoding(Guid.NewGuid(),inputFile,outputFile,options,ffmpegPath);
                transcoderManager.Tell(command);
            }
            
            Console.WriteLine("Done");
            
            
            while (true)
            {
                Console.WriteLine("Return to Get Status");
                //Console.ReadLine();
                try
                {
                    await Task.Delay(1000);
                    var result = await transcoderManager.Ask<StatusResult>(new StatusPls(), TimeSpan.FromSeconds(5));
                    Console.WriteLine($"P{result.InProgress} - C{result.Completed} - F{result.Failed}");
                }
                catch
                {
                    
                }
                
            }
            
        }
    }
}