using System;
using Transcoding.Transcoder.Model;

namespace Transcoding.Transcoder.Actors.Transcoding.Commands
{
    public class ReportTranscodingFailure
    {
        public Guid TranscodingId { get; }
        public TimeSpan TotalMediaDuration { get; }
        public TimeSpan TotalProcessDuration { get; }
        public string Data { get; }
        public Exception Exception { get; }
        public TimeSpan Elapsed { get; }
        public MediaFile From { get; }
        public MediaFile To { get; }

        public ReportTranscodingFailure(
            Guid transcodingId,
            TimeSpan totalMediaDuration,
            TimeSpan totalProcessDuration,
            string data,
            Exception exception,
            TimeSpan elapsed,
            MediaFile from,
            MediaFile to)
        {
            TranscodingId = transcodingId;
            TotalMediaDuration = totalMediaDuration;
            TotalProcessDuration = totalProcessDuration;
            Data = data;
            Exception = exception;
            Elapsed = elapsed;
            From = from;
            To = to;
        }
    }
}