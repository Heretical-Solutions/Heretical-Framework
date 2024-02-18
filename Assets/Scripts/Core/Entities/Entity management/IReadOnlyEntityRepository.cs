using System;
using System.Collections.Generic;

namespace HereticalSolutions.GameEntities
{
    public interface IReadOnlyEntityRepository<TEntityID, TEntity>
    {
        bool HasEntity(TEntityID entityID);
        
        TEntity GetRegistryEntity(TEntityID entityID);

        EntityDescriptor<TEntityID>[] AllRegistryEntities { get; }
        
        IEnumerable<TEntityID> AllAllocatedIDs { get; }
    }
}