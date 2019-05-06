using Akkatecture.Core;

namespace Projections.Prototype
{
    public interface IProjectionId : IIdentity
    {
        
    }

    public interface IProjection<TProjectionId>
        where TProjectionId : IProjectionId
    {
        TProjectionId Id { get; set; }
    }
}