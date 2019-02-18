using System.ComponentModel.Design.Serialization;
using System.Reflection.Metadata.Ecma335;
using Akka.Actor;

namespace ReadApi
{
    public class CarProjector : ReceiveActor
    {
        private static string _schema = "projection";
        private static string _table = "cars";
        private readonly string _version;

        public CarProjector(ProjectorSettings settings)
        {
            Self.Tell(BeginStreaming.Instance);   
        }
    }

    public class BeginStreaming
    {
        public static BeginStreaming Instance => new BeginStreaming();
    }
    public class ProjectorSettings
    {
        public string Version { get; }

        public ProjectorSettings(
            string version)
        {
            Version = version;
        }
    }
}
