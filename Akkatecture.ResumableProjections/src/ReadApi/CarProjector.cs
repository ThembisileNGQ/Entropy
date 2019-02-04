using Akka.Actor;

namespace ReadApi
{
    public class CarProjector : ReceiveActor
    {
        private static string _schema = "projection";
        private static string _table = "cars";
    }
}
