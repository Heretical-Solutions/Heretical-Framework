//using System;

using UnityEngine;

namespace HereticalSolutions.Services.Settings
{
    //[Serializable]
    [CreateAssetMenu(fileName = "Game object pool settings", menuName = "Settings/Object pools/Game object pool settings", order = 0)]
    public class GameObjectPoolSettings : ScriptableObject
    {
        public string PoolID;

        public GameObjectWithAddressSettings[] Elements;
    }
}