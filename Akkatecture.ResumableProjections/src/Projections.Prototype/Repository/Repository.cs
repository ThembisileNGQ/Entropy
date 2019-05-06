using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Projections.Prototype.Repository
{
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