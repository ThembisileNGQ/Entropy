using System;
using System.Globalization;
using System.Text;
using Transcoding.Transcoder.Model;
using Transcoding.Transcoder.Options;
using Transcoding.Transcoder.Util;

namespace Transcoding.Transcoder
{
    internal static class CommandBuilder
    {
        internal static string Serialize(EngineParameters engineParameters)
        {
            switch (engineParameters.Task)
            {
                case FFmpegTask.TranscodeAudio:
                    return Convert(engineParameters.InputFile, engineParameters.OutputFile, engineParameters.ConversionOptions);
                case FFmpegTask.AnalyseAudio:
                    return GetWaveform(engineParameters.InputFile, engineParameters.OutputFile, engineParameters.ConversionOptions);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string GetWaveform(MediaFile inputFile, MediaFile outputFile, ConversionOptions conversionOptions)
        {
            var commandBuilder = new StringBuilder();

            commandBuilder.AppendFormat(" -i \"{0}\" ", inputFile.Filename);
            
            commandBuilder.AppendFormat(
                " -filter_complex \"aformat=channel_layouts=mono,showwavespic=s=4000x1000\" -frames:v 1 \"{0}\" ",
                outputFile.Filename);

            return commandBuilder.ToString();
        }

        private static string Convert(MediaFile inputFile, MediaFile outputFile, ConversionOptions conversionOptions)
        {
            var commandBuilder = new StringBuilder();

            // Default conversion
            if (conversionOptions == null)
                return commandBuilder.AppendFormat(" -i \"{0}\"  \"{1}\" ", inputFile.Filename, outputFile.Filename).ToString();

            commandBuilder.AppendFormat(" -i \"{0}\" ", inputFile.Filename);

            // Audio bit rate
            if (conversionOptions.AudioBitRate != null)
                commandBuilder.AppendFormat(" -ab {0}k", conversionOptions.AudioBitRate);

            // Audio sample rate
            if (conversionOptions.AudioSampleRate != AudioSampleRate.Default)
                commandBuilder.AppendFormat(" -ar {0} ", conversionOptions.AudioSampleRate.Remove("Hz"));

            return commandBuilder.AppendFormat(" \"{0}\" ", outputFile.Filename).ToString();
        }
        
        internal static string Remove(this Enum enumerable, string text)
        {
            return enumerable.ToString()
                .Replace(text, "");
        }
    }
}