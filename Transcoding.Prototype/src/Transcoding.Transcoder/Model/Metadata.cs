using System;

namespace Transcoding.Transcoder.Model
{
    public class Metadata
    {
        internal Metadata() { }
        public TimeSpan Duration { get; internal set; }
        public Audio AudioData { get; internal set; }
        public class Audio 
        {
            internal Audio() { }

            public string Format { get; internal set; }
            public string SampleRate { get; internal set; }
            public string ChannelOutput { get; internal set; }
            public int BitRateKbs { get; internal set; }
        }
    }
}