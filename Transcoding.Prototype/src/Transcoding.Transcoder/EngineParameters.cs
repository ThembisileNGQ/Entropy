using Transcoding.Transcoder.Model;
using Transcoding.Transcoder.Options;

namespace Transcoding.Transcoder
{
    public class EngineParameters
    {
        public ConversionOptions ConversionOptions { get; }
        public MediaFile InputFile { get;}
        public MediaFile OutputFile { get; }
        public FFmpegTask Task { get; }

        public EngineParameters(
            ConversionOptions conversionOptions, 
            MediaFile inputFile,
            MediaFile outputFile,
            FFmpegTask task)
        {
            ConversionOptions = conversionOptions;
            InputFile = inputFile;
            OutputFile = outputFile;
            Task = task;
        }
    }
}