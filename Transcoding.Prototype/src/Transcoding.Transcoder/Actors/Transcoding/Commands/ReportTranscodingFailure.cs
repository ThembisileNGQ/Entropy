using System;
using Transcoding.Transcoder.Model;

namespace Transcoding.Transcoder.Actors.Transcoding.Commands
{
    public class ReportTranscodingFailure
    {
        public Guid TranscodingId { get; }
        public TimeSpan TotalMediaDuration { get; }
        public TimeSpan ProcessedMediaDuration { get; }
        public TimeSpan TotalProcessDuration { get; }
        public TimeSpan Elapsed { get; }
        public MediaFile From { get; }
        public MediaFile To { get; }

        public ReportTranscodingFailure(
            Guid transcodingId,
            TimeSpan totalMediaDuration,
            TimeSpan processedMediaDuration,
            TimeSpan totalProcessDuration,
            TimeSpan elapsed,
            MediaFile from,
            MediaFile to)
        {
            TranscodingId = transcodingId;
            TotalMediaDuration = totalMediaDuration;
            ProcessedMediaDuration = processedMediaDuration;
            TotalProcessDuration = totalProcessDuration;
            Elapsed = elapsed;
            From = from;
            To = to;
        }
    }
}