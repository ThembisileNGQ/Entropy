using System;

namespace Transcoding.Transcoder
{
    public class ConversionProgressed : EventArgs
    {
        
        public long? Frame { get; }
        public double? Fps { get; }
        public int? SizeKb { get;  }
        public TimeSpan ProcessedDuration { get;  }
        public double? Bitrate { get;  }
        public TimeSpan TotalDuration { get;  }
        public ConversionProgressed(
            TimeSpan processed,
            TimeSpan totalDuration,
            long? frame,
            double? fps,
            int? sizeKb,
            double? bitrate)
        {
            ProcessedDuration = processed;
            TotalDuration = totalDuration;
            Frame = frame;
            Fps = fps;
            SizeKb = sizeKb;
            Bitrate = bitrate;
        }

    }
}