using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Projections.Prototype
{
    public delegate Task CustomHandler<in TContext>(TContext context, Func<Task> projector);
    public delegate Task CreationHandler<out TProjection, in TKey, in TContext>(TKey key,TContext context,Func<TProjection, Task> projector,Func<TProjection, bool> shouldOverwite);
    public delegate Task UpdateHandler<out TProjection, in TKey, in TContext>(TKey key, TContext context, Func<TProjection, Task> projector,Func<bool> createIfMissing);
    public delegate Task<bool> DeletionHandler<in TKey, in TContext>(TKey key, TContext context);

    public class ProjectorMap<TProjection, TProjectionId, TProjectionContext> : ProjectorMap<TProjectionContext>
        where TProjection : class, IProjection<TProjectionId>, new()
        where TProjectionContext : ProjectionContext, new()
        where TProjectionId : IProjectionId
        
    {
        public CreationHandler<TProjection, TProjectionId, TProjectionContext> Create { get; set; } = (key, context, projector, shouldOverwrite) =>
            throw new NotSupportedException("No handler has been set-up for creations.");

        public UpdateHandler<TProjection, TProjectionId, TProjectionContext> Update { get; set; } = (key, context, projector, createIfMissing) =>
            throw new NotSupportedException("No handler has been set-up for updates.");

        public DeletionHandler<TProjectionId, TProjectionContext> Delete { get; set; } = (key, context) =>
            throw new NotSupportedException("No handler has been set-up for deletions.");
    }

    public class ProjectorMap<TProjectionContext>
        where TProjectionContext : ProjectionContext, new()
    {
        public CustomHandler<TProjectionContext> Custom { get; set; } = (context, projector)
            => throw new NotSupportedException("No handler has been set-up for custom actions.");
    }
}
