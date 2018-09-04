using System;
using System.IO;
using System.Threading.Tasks;
using Transcoding.Transcoder;
using Transcoding.Transcoder.Model;
using Transcoding.Transcoder.Options;

namespace Transcoding.Application
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            
            var input = Path.Combine(Environment.CurrentDirectory, "../../media/0.wav");
            var output = Path.Combine(Environment.CurrentDirectory, $"../../media/{Guid.NewGuid()}.mp3");

            await TranscodeAudio(input, output);
        }
        
        public static Task TranscodeAudio(string sourceFilePath, string destinationFilePath)
        {
            var options = new ConversionOptions
            {
                AudioBitRate = 320
            };
    
            var ffmpegPath = Path.Combine("/usr/local/bin/ffmpeg");

            var inputFile = new MediaFile(sourceFilePath);
            var outputFile = new MediaFile(destinationFilePath);

            using (var engine = new Engine(ffmpegPath, true))
            {
                engine.Convert(inputFile, outputFile, options);
            }

            return Task.CompletedTask;
        }
    }
}