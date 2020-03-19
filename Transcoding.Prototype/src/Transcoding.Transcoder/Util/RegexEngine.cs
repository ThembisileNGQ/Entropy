using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Transcoding.Transcoder.Model;

namespace Transcoding.Transcoder.Util
{
    /// <summary>
    ///     Contains all Regex tasks
    /// </summary>
    internal static class RegexEngine
    {
        /// <summary>
        ///     Dictionary containing every Regex test.
        /// </summary>
        internal static readonly Dictionary<Find, Regex> Index = new Dictionary<Find, Regex>
        {
            {Find.BitRate, new Regex(@"([0-9]*)\s*kb/s")},
            {Find.Duration, new Regex(@"Duration: ([^,]*), ")},
            {Find.ConvertProgressFrame, new Regex(@"frame=\s*([0-9]*)")},
            {Find.ConvertProgressFps, new Regex(@"fps=\s*([0-9]*\.?[0-9]*?)")},
            {Find.ConvertProgressSize, new Regex(@"size=\s*([0-9]*)kB")},
            {Find.ConvertProgressFinished, new Regex(@"Lsize=\s*([0-9]*)kB")},
            {Find.ConvertProgressTime, new Regex(@"time=\s*([^ ]*)")},
            {Find.ConvertProgressBitrate, new Regex(@"bitrate=\s*([0-9]*\.?[0-9]*?)kbits/s")},
            {Find.MetaAudio, new Regex(@"(Stream\s*#[0-9]*:[0-9]*\(?[^\)]*?\)?: Audio:.*)")},
            {Find.AudioFormatHzChannel, new Regex(@"Audio:\s*([^,]*),\s([^,]*),\s([^,]*)")}
        };

        /// <summary>
        ///     <para> ---- </para>
        ///     <para>Establishes whether the data contains progress information.</para>
        /// </summary>
        /// <param name="data">Event data from the FFmpeg console.</param>
        /// <param name="progressEmitted">
        ///     <para>If successful, outputs a <see cref="ConversionProgressed"/> which is </para>
        ///     <para>generated from the data. </para>
        /// </param>
        internal static bool IsProgressData(string data, out ConversionProgressed progressEmitted)
        {
            progressEmitted = null;

            var matchFrame = Index[Find.ConvertProgressFrame].Match(data);
            var matchFps = Index[Find.ConvertProgressFps].Match(data);
            var matchSize = Index[Find.ConvertProgressSize].Match(data);
            var matchTime = Index[Find.ConvertProgressTime].Match(data);
            var matchBitrate = Index[Find.ConvertProgressBitrate].Match(data);

            if (!matchSize.Success || !matchTime.Success || !matchBitrate.Success)
                return false;

            TimeSpan.TryParse(matchTime.Groups[1].Value, out var processedDuration);

            var frame = GetLongValue(matchFrame);
            var fps = GetDoubleValue(matchFps);
            var sizeKb = GetIntValue(matchSize);
            var bitrate = GetDoubleValue(matchBitrate);

            progressEmitted =
                new ConversionProgressed(processedDuration, TimeSpan.Zero, frame, fps, sizeKb, bitrate);

            return true;
        }

        private static long? GetLongValue(Match match)
        {
            try
            {
                return Convert.ToInt64(match.Groups[1].Value, CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        private static double? GetDoubleValue(Match match)
        {
            try
            {
                return Convert.ToDouble(match.Groups[1].Value, CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        private static int? GetIntValue(Match match)
        {
            try
            {
                return Convert.ToInt32(match.Groups[1].Value, CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        ///     <para> ---- </para>
        ///     <para>Establishes whether the data indicates the conversion is complete</para>
        /// </summary>
        /// <param name="data">Event data from the FFmpeg console.</param>
        /// <param name="conversionCompleted">
        ///     <para>If successful, outputs a <see cref="ConversionCompleted"/> which is </para>
        ///     <para>generated from the data. </para>
        /// </param>
        internal static bool IsConvertCompleteData(string data, out ConversionCompleted conversionCompleted)
        {
            conversionCompleted = null;

            var matchFrame = Index[Find.ConvertProgressFrame].Match(data);
            var matchFps = Index[Find.ConvertProgressFps].Match(data);
            var matchFinished = Index[Find.ConvertProgressFinished].Match(data);
            var matchTime = Index[Find.ConvertProgressTime].Match(data);
            var matchBitrate = Index[Find.ConvertProgressBitrate].Match(data);

            if (!matchFrame.Success || !matchFps.Success || !matchFinished.Success || !matchTime.Success ||
                !matchBitrate.Success) return false;

            TimeSpan processedDuration;
            TimeSpan.TryParse(matchTime.Groups[1].Value, out processedDuration);

            var frame = Convert.ToInt64(matchFrame.Groups[1].Value, CultureInfo.InvariantCulture);
            var fps = Convert.ToDouble(matchFps.Groups[1].Value, CultureInfo.InvariantCulture);
            var sizeKb = Convert.ToInt32(matchFinished.Groups[1].Value, CultureInfo.InvariantCulture);
            var bitrate = Convert.ToDouble(matchBitrate.Groups[1].Value, CultureInfo.InvariantCulture);

            conversionCompleted =
                new ConversionCompleted(processedDuration, TimeSpan.Zero, frame, fps, sizeKb, bitrate);

            return true;
        }

        internal static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrEmpty(value) || value.Trim()
                .Length == 0;
        }

        internal enum Find
        {
            AudioFormatHzChannel,
            ConvertProgressBitrate,
            ConvertProgressFps,
            ConvertProgressFrame,
            ConvertProgressSize,
            ConvertProgressFinished,
            ConvertProgressTime,
            Duration,
            MetaAudio,
            BitRate,
        }
    }
}