using System.Threading.Tasks;

namespace Projections.Prototype
{
    internal static class TaskConstants
    {
        internal static readonly Task<bool> FalseTask = Task.FromResult(false);
        internal static readonly Task<int> ZeroTask = Task.FromResult(0);
    }
}