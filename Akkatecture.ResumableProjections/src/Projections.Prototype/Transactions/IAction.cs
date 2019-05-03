using System;
using System.Threading.Tasks;

namespace Projections.Prototype.Transactions
{
    public interface IAction<TEvent, out TContext>
    {
        void As(Func<TEvent, TContext, Task> action);
        IAction<TEvent, TContext> When(Func<TEvent, TContext, Task<bool>> predicate);
    }

    public interface ICrudAction<TEvent, TProjection, TKey, TContext> : IAction<TEvent, TContext>
    {
        ICreateAction<TEvent, TProjection, TContext> AsCreateOf(Func<TEvent, TKey> getKey);
        [Obsolete("Use AsCreateOf().IgnoringDuplicates() instead")]
        ICreateAction<TEvent, TProjection, TContext> AsCreateIfDoesNotExistOf(Func<TEvent, TKey> getKey);
        IUpdateAction<TEvent, TKey, TProjection, TContext> AsUpdateOf(Func<TEvent, TKey> getKey);
        [Obsolete("Use AsUpdateOf().IgnoringMissing() instead")]
        IUpdateAction<TEvent, TKey, TProjection, TContext> AsUpdateIfExistsOf(Func<TEvent, TKey> getKey);
        [Obsolete("Use AsCreateOf().OverwritingDuplicates() instead")]
        ICreateAction<TEvent, TProjection, TContext>  AsCreateOrUpdateOf(Func<TEvent, TKey> getKey);
        IDeleteAction<TEvent, TKey, TContext> AsDeleteOf(Func<TEvent, TKey> getKey);
        [Obsolete("Use AsDeleteOf().IgnoringMissing() instead")]
        IDeleteAction<TEvent, TKey, TContext> AsDeleteIfExistsOf(Func<TEvent, TKey> getKey);
        new ICrudAction<TEvent, TProjection, TKey, TContext> When(Func<TEvent, TContext, Task<bool>> predicate);
    }
}