using System;
using System.Threading.Tasks;

namespace Projections.Prototype.Transactions
{
    public interface IUpdateAction<out TEvent, TKey, out TProjection, out TContext>
    {
        IUpdateAction<TEvent, TKey, TProjection, TContext> Using(Func<TProjection, TEvent, TContext, Task> updateAction);
        IUpdateAction<TEvent, TKey, TProjection, TContext> ThrowingIfMissing();
        IUpdateAction<TEvent, TKey, TProjection, TContext> IgnoringMisses();
        IUpdateAction<TEvent, TKey, TProjection, TContext> CreatingIfMissing();
        IUpdateAction<TEvent, TKey, TProjection, TContext> HandlingMissesUsing(Func<TKey, TContext, bool> action);
    }
}