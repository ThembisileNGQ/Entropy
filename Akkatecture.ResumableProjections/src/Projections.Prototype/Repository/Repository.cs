using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Akka.Actor;
using Akka.Event;
using Newtonsoft.Json;

namespace Projections.Prototype.Repository
{
    public class Create<TProjection, TProjectionId>
        where TProjection : class, IProjection<TProjectionId>
        where TProjectionId : IProjectionId
    {
        public TProjection Projection { get; set; }
    }
    public class Read<TProjectionId>
        where TProjectionId : IProjectionId
    {
        public TProjectionId Key { get; set; }
    }
    
    public class Update<TProjection, TProjectionId>
        where TProjection : class, IProjection<TProjectionId>
        where TProjectionId : IProjectionId
    {
        public TProjection Projection { get; set; }
    }
    public class Delete<TProjectionId>
        where TProjectionId : IProjectionId
    {
        public TProjectionId Key { get; set; }
    }
    public class RepositoryActor<TProjection, TProjectionId> : ReceiveActor
        where TProjection : class, IProjection<TProjectionId>
        where TProjectionId : IProjectionId
    {
        public Dictionary<string, string> Repository { get; }

        public RepositoryActor()
        {
            Repository = new Dictionary<string, string>();

            Receive<Create<TProjection, TProjectionId>>(Handle);
            Receive<Read<TProjectionId>>(Handle);
            Receive<Update<TProjection, TProjectionId>>(Handle);
            Receive<Delete<TProjectionId>>(Handle);
            
        }

        public bool Handle(Create<TProjection, TProjectionId> command)
        {
            Repository[command.Projection.Id.Value] = JsonConvert.SerializeObject(command.Projection);
            Context.GetLogger().Info("{0} has been created.", command.Projection.Id.Value);
            return true;
        }
        
        public bool Handle(Read<TProjectionId> command)
        {
            var projection = Repository[command.Key.Value];

            var obj = JsonConvert.DeserializeObject<TProjection>(projection);
            
            Sender.Tell(obj);
            
            Context.GetLogger().Info("{0} has been read.", command.Key.Value);
            return true;
        }
        
        public bool Handle(Update<TProjection, TProjectionId> command)
        {
            Repository[command.Projection.Id.Value] = JsonConvert.SerializeObject(command.Projection);
            
            Context.GetLogger().Info("{0} has been updated.", command.Projection.Id.Value);
            return true;
        }
        
        public bool Handle(Delete<TProjectionId> command)
        {
            Repository.Remove(command.Key.Value);
            
            Context.GetLogger().Info("{0} has been deleted.", command.Key.Value);
            return true;
        }
    }
    
    
}