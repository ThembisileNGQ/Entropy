using System;
using System.Threading.Tasks;

namespace Projections.Prototype.Transactions
{
    public interface IUpsertActionBuilder<TEvent, TProjection, out TContext>
    {
        void Using(Func<TProjection, TEvent, TContext, Task> projector);
    }
}