using System;

namespace HereticalSolutions.Entities
{
    [Serializable]
    public struct ECSComponentDeltaDTO
    {
        public string EntityGUID;
        
        public ECSComponentDTO ComponentDTO;
    }
}