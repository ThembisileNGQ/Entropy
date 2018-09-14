using System;
using System.Collections.Generic;
using Transcoding.Transcoder.Model;

namespace Transcoding.Transcoder.Actors.Transcoding.Commands
{
    public class ReportTranscodingCompletion
    {
        public Guid TranscodingId { get; }
        public TimeSpan TotalMediaDuration { get; }
        public TimeSpan ProcessedMediaDuration { get; }
        public TimeSpan TotalProcessDuration { get; }
        public DateTime StartedAt { get; }
        public DateTime EndedAt { get; }
        public MediaFile From { get; }
        public MediaFile To { get; }
        public IReadOnlyCollection<string> Log { get; }

        public ReportTranscodingCompletion(
            Guid transcodingId,
            TimeSpan totalMediaDuration,
            TimeSpan processedMediaDuration,
            TimeSpan totalProcessDuration,
            DateTime startedAt,
            DateTime endedAt,
            MediaFile from,
            MediaFile to,
            IReadOnlyCollection<string> log)
        {
            TranscodingId = transcodingId;
            TotalMediaDuration = totalMediaDuration;
            ProcessedMediaDuration = processedMediaDuration;
            StartedAt = startedAt;
            EndedAt = endedAt;
            TotalProcessDuration = totalProcessDuration;
            From = from;
            To = to;
            Log = log;
        }
    }
}