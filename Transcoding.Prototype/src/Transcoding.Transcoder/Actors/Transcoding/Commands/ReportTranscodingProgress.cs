using System;
using Transcoding.Transcoder.Model;

namespace Transcoding.Transcoder.Actors.Transcoding.Commands
{
    public class ReportTranscodingProgress
    {
        public Guid TranscodingId { get; }
        public double Progress { get; }
        public TimeSpan TotalMediaDuration { get; }
        public TimeSpan ProcessedMediaDuration { get; }
        public TimeSpan CurrentProcessDuration { get; }
        public DateTime StartedAt { get; }
        public MediaFile From { get; }
        public MediaFile To { get; }

        public ReportTranscodingProgress(
            Guid transcodingId,
            TimeSpan totalMediaDuration,
            TimeSpan processedMediaDuration,
            TimeSpan currentProcessDuration,
            DateTime startedAt,
            MediaFile from,
            MediaFile to)
        {
            TranscodingId = transcodingId;
            TotalMediaDuration = totalMediaDuration;
            ProcessedMediaDuration = processedMediaDuration;
            StartedAt = startedAt;
            From = from;
            CurrentProcessDuration = currentProcessDuration;
            To = to;
            Progress = ProcessedMediaDuration.Ticks / (double)TotalMediaDuration.Ticks;
        }
    }
}