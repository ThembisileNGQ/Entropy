using System;
using System.Net.WebSockets;
using Akka.Actor;
using Akka.Websockets.Manager.Actors.Commands;

namespace Akka.Websockets.Manager.Actors
{
    public class LiveWebsocketActor : ReceiveActor
    {
        private readonly Guid _connectionId;
        private readonly Guid _userId;
        private readonly WebSocket _connection;
        private readonly IActorRef _connectionAggregate;

        public LiveWebsocketActor(
            Guid connectionId,
            Guid userId,
            WebSocket connection,
            IActorRef connectionAggregate)
        {
            _connectionId = connectionId;
            _userId = userId;
            _connection = connection;
            _connectionAggregate = connectionAggregate;
            
            var command = new AddUserConnection(
                new UserConnection(connectionId,userId,Self));
            
            _connectionAggregate.Tell(command);
            Receive<SendWebsocketMessage>(Handle);
        }

        private bool Handle(SendWebsocketMessage handle)
        {
            throw new NotImplementedException();
        }
    }
}