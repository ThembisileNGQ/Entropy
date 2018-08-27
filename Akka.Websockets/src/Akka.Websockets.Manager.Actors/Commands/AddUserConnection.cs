using System;
using Akka.Actor;

namespace Akka.Websockets.Manager.Actors.Commands
{
    public class AddUserConnection
    {
        public IActorRef WebsocketActorRef { get; }
        public Guid UserId { get; }
        public Guid ConnectionId { get; }

        public AddUserConnection(
            IActorRef websocketRef,
            Guid userId,
            Guid connectionId)
        {
            WebsocketActorRef = websocketRef;
            UserId = userId;
            ConnectionId = connectionId;
        }
        
    }
}