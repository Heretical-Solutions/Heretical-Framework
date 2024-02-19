using System;

namespace HereticalSolutions.Entities
{
    [Serializable]
    public struct ECSEntityDTO
    {
        public string GUID;

        public string PrototypeID;

        public ECSComponentDTO[] ComponentDTOs;
    }
}