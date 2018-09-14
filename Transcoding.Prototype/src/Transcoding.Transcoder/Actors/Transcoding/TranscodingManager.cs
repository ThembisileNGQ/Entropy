using Akka.Actor;
using Akka.Event;
using Transcoding.Transcoder.Actors.Transcoding.Commands;
using Transcoding.Transcoder.Actors.Transcoding.Responses;

namespace Transcoding.Transcoder.Actors.Transcoding
{
    public class TranscodingManager : ReceiveActor
    {
        private readonly ILoggingAdapter _logger;
        private int Completed = 0;
        private int InProgress = 0;
        private int Failed = 0;
        public TranscodingManager()
        {
            _logger = Context.GetLogger();

            Receive<StatusPls>(Handle);
            Receive<StartTranscoding>(Handle);
            Receive<ReportTranscodingProgress>(Handle);
            Receive<ReportTranscodingCompletion>(Handle);
            Receive<ReportTranscodingFailure>(Handle);
        }
        
        
        public bool Handle(StartTranscoding command)
        {
            var actor = Context.ActorOf(Props.Create(() => new TranscodingActor(
                command.TranscodingId,
                command.Input,
                command.Output,
                command.ConversionOptions,
                command._ffmpegPath)),$"transcododer-{command.TranscodingId}");
            
            actor.Tell(new Start());
            InProgress++;
            
            return true;
        }
        
        public bool Handle(ReportTranscodingProgress command)
        {
            _logger.Info($"{command.TranscodingId}: Progress: {command.Progress} %" );
            return true;
        }
        
        public bool Handle(ReportTranscodingCompletion command)
        {
            _logger.Info($"{command.TranscodingId}: Completed Sucessfully Elapsed: {command.TotalProcessDuration.Seconds}s %" );
            Completed++;
            InProgress--;
            return true;
        }

        public bool Handle(ReportTranscodingFailure command)
        {
            _logger.Warning($"{command.TranscodingId}: Completed UnSucessfully Elapsed: {command.TotalProcessDuration.Seconds}s %" );
            Failed++;
            InProgress--;
            return true;
        }

        public bool Handle(StatusPls query)
        {
            Sender.Tell(new StatusResult(InProgress,Completed,Failed));

            return true;
        }

    }
}
