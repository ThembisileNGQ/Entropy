using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Projections.Prototype
{
    public interface IAction<TEvent, out TContext>
    {
        void As(Func<TEvent, TContext, Task> action);
        IAction<TEvent, TContext> When(Func<TEvent, TContext, Task<bool>> predicate);
    }
}
