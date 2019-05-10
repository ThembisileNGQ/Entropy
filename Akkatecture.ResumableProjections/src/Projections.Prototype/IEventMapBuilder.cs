using System;
using System.Collections.Generic;
using System.Text;

namespace Projections.Prototype
{
    public interface IEventMapBuilder<TProjectionContext>
        where TProjectionContext : ProjectionContext, new()
    {
        IEventMap<TProjectionContext> Build(ProjectorMap<TProjectionContext> projector);
    }

    public interface IEventMapBuilder<TProjection, TProjectionId, TProjectionContext>
        where TProjection : class, IProjection<TProjectionId>, new()
        where TProjectionContext : ProjectionContext, new()
        where TProjectionId : IProjectionId
    {
        IEventMap<TProjectionContext> Build(ProjectorMap<TProjection, TProjectionId, TProjectionContext> projector);
    }
}
