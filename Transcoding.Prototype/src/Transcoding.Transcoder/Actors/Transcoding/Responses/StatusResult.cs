namespace Transcoding.Transcoder.Actors.Transcoding.Responses
{
    public class StatusResult
    {
        public int InProgress { get; }
        public int Completed { get; }
        public int Failed { get; }

        public StatusResult(
            int inProgress,
            int completed,
            int failed)
        {
            InProgress = inProgress;
            Completed = completed;
            Failed = failed;
        }
    }
}