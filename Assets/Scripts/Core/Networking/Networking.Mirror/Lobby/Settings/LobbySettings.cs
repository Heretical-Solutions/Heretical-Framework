using HereticalSolutions.Networking.Mirror;

using UnityEngine;
using UnityEngine.Serialization;

namespace HereticalSolutions.Networking
{
    [CreateAssetMenu(fileName = "Lobby settings", menuName = "Settings/Networking/Mirror lobby settings", order = 0)]
    public class LobbySettings : ScriptableObject
    {
        [Header("Lobby Settings")]

        [SerializeField]
        [Tooltip("Maximum number of players")]
        public int MaxPlayers = 16;
        
        [FormerlySerializedAs("LobbyPlayerSlotPrefab")]
        [SerializeField]
        [Tooltip("Prefab to use for the Lobby Player Slot")]
        public LobbySlotBehaviour lobbySlotBehaviourPrefab;
    }
}