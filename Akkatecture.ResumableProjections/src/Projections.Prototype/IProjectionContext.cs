using System;
using System.Collections.Generic;
using System.Text;

namespace Projections.Prototype
{
    public interface IProjectionContext
    {
        void MarkForDeletion();
        bool IsMarkedForDeletion { get; }
        string ReadModelId { get; }
    }
}
