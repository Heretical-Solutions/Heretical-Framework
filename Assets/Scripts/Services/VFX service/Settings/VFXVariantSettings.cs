using System;

using HereticalSolutions.Allocations;

using UnityEngine;

namespace HereticalSolutions.Services.Settings
{
    [Serializable]
    public class VFXVariantSettings
    {
        public float Chance;

        public GameObject Prefab;
    
        public float Duration;

        public AllocationCommandDescriptor Initial;

        public AllocationCommandDescriptor Additional;
    }
}