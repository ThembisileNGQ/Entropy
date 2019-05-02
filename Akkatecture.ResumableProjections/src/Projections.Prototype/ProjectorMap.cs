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

    public class ProjectorMap<TProjection, TKey, TContext> : ProjectorMap<TContext>
    {
        public CreationHandler<TProjection, TKey, TContext> Create { get; set; } = (key, context, projector, shouldOverwrite) =>
            throw new NotSupportedException("No handler has been set-up for creations.");

        public UpdateHandler<TProjection, TKey, TContext> Update { get; set; } = (key, context, projector, createIfMissing) =>
            throw new NotSupportedException("No handler has been set-up for updates.");

        public DeletionHandler<TKey, TContext> Delete { get; set; } = (key, context) =>
            throw new NotSupportedException("No handler has been set-up for deletions.");
    }

    public class ProjectorMap<TContext>
    {
        public CustomHandler<TContext> Custom { get; set; } = (context, projector)
            => throw new NotSupportedException("No handler has been set-up for custom actions.");
    }
}
