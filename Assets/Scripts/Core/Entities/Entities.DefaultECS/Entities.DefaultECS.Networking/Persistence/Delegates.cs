using System;
using System.Collections.Generic;

using DefaultEcs;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Entities
{
    public delegate void ReadComponentToDTOsListDelegate(
        Entity entity,
        IReadOnlyRepository<Type, int> typeToHash,
        List<ECSComponentDTO> componentDTOs);

    public delegate void WriteComponentFromDTODelegate(
        Entity entity,
        ECSComponentDTO componentDTO);
}