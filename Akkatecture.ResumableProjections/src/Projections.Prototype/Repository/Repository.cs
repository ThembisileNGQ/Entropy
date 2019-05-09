using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Akka.Actor;

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
        public ProjectionRepository<TProjection, TProjectionId> Repository { get; }

        public RepositoryActor()
        {
            Repository = new ProjectionRepository<TProjection, TProjectionId>();

            Receive<Create<TProjection, TProjectionId>>(Handle);
            Receive<Read<TProjectionId>>(Handle);
            Receive<Update<TProjection, TProjectionId>>(Handle);
            Receive<Delete<TProjectionId>>(Handle);

        }

        public bool Handle(Create<TProjection, TProjectionId> command)
        {
            Repository.Add(command.Projection);
            return true;
        }
        
        public bool Handle(Read<TProjectionId> command)
        {
            Sender.Tell(Repository.Find(command.Key));
            return true;
        }
        
        public bool Handle(Update<TProjection, TProjectionId> command)
        {
            Repository.RemoveByKey(command.Projection.Id);
            Repository.Add(command.Projection);
            return true;
        }
        
        public bool Handle(Delete<TProjectionId> command)
        {
            Repository.RemoveByKey(command.Key);
            return true;
        }
    }
    
    public class ProjectionRepository<TProjection, TProjectionId> : Repository<TProjection, TProjectionId>
        where TProjection : class, IProjection<TProjectionId>
        where TProjectionId : IProjectionId
        
    {
        protected static List<TProjection> Projections { get; } = new List<TProjection>();
        protected override IQueryable<TProjection> Entities { get; } = Projections.AsQueryable();
        
        public override TProjection Find(TProjectionId identity)
        {
            return Entities.SingleOrDefault(x => x.Id.Equals(identity));
        }

        protected override void AddEntity(TProjection entity)
        {
            Projections.Add(entity);
        }

        protected override void RemoveEntity(TProjection entity)
        {
            Projections.RemoveAll(x => x.Id.Equals(entity.Id));
        }
    }
    
    public abstract class Repository<TProjection,TProjectionId> : IQueryable<TProjection> 
        where TProjection : class, IProjection<TProjectionId>
        where TProjectionId : IProjectionId
    {
        protected abstract IQueryable<TProjection> Entities { get; }

        public int Count => Entities.Count();

        public Type ElementType => Entities.ElementType;

        public Expression Expression => Entities.Expression;

        public IQueryProvider Provider => Entities.Provider;
        
        public abstract TProjection Find(TProjectionId identity);

        public void AddRange(IEnumerable<TProjection> entities)
        {
            foreach (var entity in entities)
            {
                Add(entity);
            }
        }

        public void Add(TProjection entity)
        {
            AddEntity(entity);
        }

        protected abstract void AddEntity(TProjection entity);

        public void RemoveRange(IEnumerable<TProjection> entities)
        {
            foreach (var entity in entities)
            {
                Remove(entity);
            }
        }

        public void RemoveByKey(TProjectionId key)
        {
            TProjection entity = Entities.SingleOrDefault(e => e.Id.Equals(key));
            if (entity != null)
            {
                RemoveEntity(entity);
            }
        }

        public void Remove(TProjection entity)
        {
            RemoveEntity(entity);
        }

        public void Clear()
        {
            RemoveRange(Entities);
        }

        protected abstract void RemoveEntity(TProjection entity);

        public IEnumerator<TProjection> GetEnumerator()
        {
            return Entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Entities.GetEnumerator();
        }


    }
}