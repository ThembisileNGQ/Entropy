using System.Threading.Tasks;

namespace Projections.Prototype
{
    public interface IEventMap<in TProjectionContext>
        where TProjectionContext : ProjectionContext, new()
    {
        Task<bool> Handle(object anEvent, TProjectionContext context);
    }
}