using System;
using Transcoding.Transcoder.Model;

namespace Transcoding.Transcoder.Actors.Transcoding.Responses
{
    public class TranscodingResult
    {    
        public Guid TranscodingId { get; }
        public bool Success { get; }
        public TimeSpan TotalProcessTime { get; }
        public MediaFile FromLocation { get; }
        public MediaFile ToLocation { get; }
        public DateTime StartedAt { get; }
        public DateTime FinishedAt { get; }

        public TranscodingResult(
            Guid transcodingId,
            bool success,
            TimeSpan totalProcessTime,
            MediaFile fromLocation,
            MediaFile toLocation,
            DateTime startedAt,
            DateTime finishedAt)
        {
            TranscodingId = transcodingId;
            Success = success;
            TotalProcessTime = totalProcessTime;
            FromLocation = fromLocation;
            ToLocation = toLocation;
            StartedAt = startedAt;
            FinishedAt = finishedAt;
        }
    }
}