using System;
using System.Threading.Tasks;

namespace Projections.Prototype.Transactions
{
    public interface ICreateAction<out TEvent, out TProjection, out TContext>
    {
        ICreateAction<TEvent, TProjection, TContext> Using(Func<TProjection, TEvent, TContext, Task> projector);
        ICreateAction<TEvent, TProjection, TContext> IgnoringDuplicates();
        ICreateAction<TEvent, TProjection, TContext> OverwritingDuplicates();
        ICreateAction<TEvent, TProjection, TContext> HandlingDuplicatesUsing(Func<TProjection, TEvent, TContext, bool> shouldOverwrite);
    }
}