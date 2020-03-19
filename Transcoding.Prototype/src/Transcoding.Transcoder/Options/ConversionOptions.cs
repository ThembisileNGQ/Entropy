using System;

namespace Transcoding.Transcoder.Options
{
    public class ConversionOptions
    {
        /// <summary>
        ///     Audio bit rate
        /// </summary>
        public int? AudioBitRate = null;

        /// <summary>
        ///     Audio sample rate
        /// </summary>
        public AudioSampleRate AudioSampleRate = AudioSampleRate.Default;
    }
}