using System;
using System.Collections.Generic;
using Transcoding.Transcoder.Model;

namespace Transcoding.Transcoder.Actors.Transcoding.Commands
{
    public class ReportTranscodingFailure
    {
        public Guid TranscodingId { get; }
        public TimeSpan TotalMediaDuration { get; }
        public TimeSpan TotalProcessDuration { get; }
        public DateTime StartedAt { get; }
        public DateTime EndedAt { get; }
        public MediaFile From { get; }
        public MediaFile To { get; }
        public IReadOnlyCollection<Exception> Exceptions { get; }
        public IReadOnlyCollection<string> Log { get; }

        public ReportTranscodingFailure(
            Guid transcodingId,
            TimeSpan totalMediaDuration,
            TimeSpan totalProcessDuration,
            DateTime startedAt,
            DateTime endedAt,
            MediaFile from,
            MediaFile to,
            IReadOnlyCollection<Exception> exceptions,
            IReadOnlyCollection<string> log)
        {
            TranscodingId = transcodingId;
            TotalMediaDuration = totalMediaDuration;
            TotalProcessDuration = totalProcessDuration;
            StartedAt = startedAt;
            EndedAt = endedAt;
            From = from;
            To = to;
            Exceptions = exceptions;
            Log = log;
        }
    }
}