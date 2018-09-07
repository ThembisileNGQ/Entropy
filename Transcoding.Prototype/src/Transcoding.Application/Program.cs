using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Akka.Actor;
using Transcoding.Transcoder;
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
            var output = Path.Combine(Environment.CurrentDirectory, $"{Guid.NewGuid()}.mp3");

            await TranscodeAudio(input, output);
        }
        
        public static async Task TranscodeAudio(string sourceFilePath, string destinationFilePath)
        {
            var actorSystem = ActorSystem.Create("transcoding-system");

            var options = new ConversionOptions
            {
                AudioBitRate = 320
            };
    
            
           // var ffmpegPath = Path.Combine("/usr/local/bin/ffmpeg");
            var ffmpegPath = Path.Combine(@"C:\Workspace\FFMPEG\bin\ffmpeg.exe");

            var inputFile = new MediaFile(sourceFilePath);
            var outputFile = new MediaFile(destinationFilePath);
            var stopWatch = Stopwatch.StartNew();
            var transcoder = actorSystem.ActorOf(Props.Create(() => new TranscodingActor(inputFile,outputFile,options,ffmpegPath)), "transcododer-1");
            var a = await  transcoder.Ask<Done>(new Start(),TimeSpan.FromMinutes(1));
            
            /*using (var engine = new Engine(ffmpegPath, true))
            {
                engine.Convert(inputFile, outputFile, options);
            }*/
            stopWatch.Stop();
            
        }
    }
}