using System.Threading.Tasks;

namespace Projections.Prototype
{
    public interface IEventMap<in TProjectionContext>
    {
        Task<bool> Handle(object anEvent, TProjectionContext context);
    }
}