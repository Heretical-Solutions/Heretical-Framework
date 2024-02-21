using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HereticalSolutions.Entities
{
	public abstract class ASceneEntity<TEntityID> : MonoBehaviour
	{
		[SerializeField] // FOR DEBUG PURPOSES ONLY
		protected string persistentID;

		public List<ASceneEntity<TEntityID>> childEntities;

		public abstract TEntityID EntityID { get; }

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (string.IsNullOrEmpty(persistentID))
            {
				persistentID = AllocateID().ToString();

                UnityEditor.Undo.RecordObject(this, "Assigned entity ID");
            }

            EditorUtility.SetDirty(this);
        }

		protected abstract TEntityID AllocateID();
#endif
	}
}