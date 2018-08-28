using System;
using System.Collections.Generic;
using Akka.Actor;

namespace Akka.Websockets.Manager.Actors
{
    public class UserConnectionAggregate : ReceiveActor
    {
        public Guid UserId { get; }
        public Dictionary<Guid, UserConnection> UserConnections { get; }

        public UserConnectionAggregate(Guid userId)
        {
            UserId = userId;
            
            UserConnections = new Dictionary<Guid, UserConnection>();

            Receive<AddUserConnection>(Handle);
            Receive<RemoveUserConnection>(Handle);
        }

        public bool Handle(AddUserConnection command)
        {
            if (UserConnections.ContainsKey(command.UserConnection.ConnectionId))
            {
                UserConnections[command.UserConnection.ConnectionId] = command.UserConnection;
            }
            else
            {
                UserConnections.Add(command.UserConnection.ConnectionId,command.UserConnection);
            }
            return true;
        }
        
        public bool Handle(RemoveUserConnection command)
        {
            UserConnections.Remove(command.ConnectionId);
            return true;
        }
        
        
    }

    public class UserConnection
    {
        public Guid ConnectionId { get; }
        public Guid UserId { get; }
        public IActorRef LiveWebsocketActorRef { get; }

        public UserConnection(
            Guid connectionId,
            Guid userId,
            IActorRef liveWebsocketActorRef)
        {
            ConnectionId = connectionId;
            UserId = userId;
            LiveWebsocketActorRef = liveWebsocketActorRef;
        }
    }

    public class AddUserConnection
    {
        public UserConnection UserConnection { get; }
        
        public AddUserConnection(UserConnection userConnection)
        {
            UserConnection = userConnection;
        }
    }

    public class RemoveUserConnection
    {
        public Guid ConnectionId { get; }

        public RemoveUserConnection(Guid connectionId)
        {
            ConnectionId = connectionId;
        }
    }
}