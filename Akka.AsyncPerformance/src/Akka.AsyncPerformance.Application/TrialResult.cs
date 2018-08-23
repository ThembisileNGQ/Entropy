using System.Runtime.InteropServices.ComTypes;

namespace Akka.AsyncPerformance.Application
{
    public class TrialResult
    {
        public int Store { get; }
        public int NumberOfMessages { get; }
        public long TimeMillis { get; }
        public bool Pass => NumberOfMessages == Store;
        
        public TrialResult(
            int store,
            int numberOfMessages,
            long timeMillis)
        {
            Store = store;
            NumberOfMessages = numberOfMessages;
            TimeMillis = timeMillis;
        }

        public override string ToString()
        {
            return $"Time : {TimeMillis} ms.\nStore : {Store}.\nTest Pass : {Pass}";
        }
    }
}