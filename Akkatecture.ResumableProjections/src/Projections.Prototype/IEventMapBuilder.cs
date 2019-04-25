using System;
using System.Collections.Generic;
using System.Text;

namespace Projections.Prototype
{
    public interface IEventMapBuilder<TContext>
    {
        IEventMap<TContext> Build(ProjectorMap<TContext> projector);
    }

    public interface IEventMapBuilder<TProjection, TKey, TContext>
    {
        IEventMap<TContext> Build(ProjectorMap<TProjection, TKey, TContext> projector);
    }
}
