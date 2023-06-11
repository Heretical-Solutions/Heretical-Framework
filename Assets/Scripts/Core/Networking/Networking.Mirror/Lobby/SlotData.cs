using System;

using HereticalSolutions.Networking.Mirror;

using Mirror;

namespace HereticalSolutions.Networking
{
    public class SlotData
    {
        public int Index { get; private set; }

        public bool Occupied
        {
            get => Connection != null;
        }

        public NetworkConnectionToClient Connection { get; private set; }

        public LobbySlotBehaviour Behaviour { get; private set; }
        
        //public GameObject IdentityGameObject { get; private set; }

        public SlotData(
            int index)
        {
            Connection = null;
            
            Index = index;

            Behaviour = null;

            //IdentityGameObject = null;
        }

        public void Claim(
            NetworkConnectionToClient connection) //,
            //GameObject identityGameObject)
        {
            Connection = connection;

            //IdentityGameObject = identityGameObject;
            
            UnityEngine.Debug.Log($"[SlotData] SLOT {Index} CLAIMED BY {connection.address}");
        }

        public void Free()
        {
            Connection = null;

            //IdentityGameObject = null;

            //Slot = null;
            
            UnityEngine.Debug.Log($"[SlotData] SLOT {Index} FREED");
        }
        
        public void RegisterBehaviour(LobbySlotBehaviour slotBehaviour)
        {
            Behaviour = slotBehaviour;
            
            UnityEngine.Debug.Log($"[SlotData] SLOT {Index} BEHAVIOUR REGISTERED", slotBehaviour);
        }

        public void VacateBehaviour(LobbySlotBehaviour slotBehaviour)
        {
            if (Behaviour != slotBehaviour)
                throw new Exception("[SlotData] INVALID SLOT");
            
            Behaviour = null;
            
            UnityEngine.Debug.Log($"[SlotData] SLOT {Index} BEHAVIOUR VACATED", slotBehaviour);
        }
        
        public void VacateBehaviour()
        {
            Behaviour = null;
            
            UnityEngine.Debug.Log($"[SlotData] SLOT {Index} BEHAVIOUR VACATED");
        }
    }
}