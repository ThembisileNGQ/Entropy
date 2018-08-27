using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;

namespace Akka.Websockets.Manager.Actors.Commands
{
    public class RemoveUserConnection
    {
        public IActorRef WebsocketActorRef { get; }
        public Guid UserId { get; }
        public Guid ConnectionId { get; }

        public RemoveUserConnection(
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
