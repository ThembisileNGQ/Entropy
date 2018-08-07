using Akka.Actor;
using Akka.Event;

namespace Akka.Hosting.Actors
{
    public class GreetingActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger;
        public GreetingActor()
        {
            _logger = Context.GetLogger();

            Receive<Hello>(Greet);
            Receive<Goodbye>(Greet);
        }

        public bool Greet(Hello message)
        {
            _logger.Info("Hello from GreetingActor");
            return true;
        }

        public bool Greet(Goodbye message)
        {
            _logger.Info("Goodbye from GreetingActor");
            return true;
        }
    }
}