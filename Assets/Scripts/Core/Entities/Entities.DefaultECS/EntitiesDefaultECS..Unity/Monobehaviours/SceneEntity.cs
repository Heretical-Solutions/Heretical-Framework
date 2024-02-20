using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations.Factories;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HereticalSolutions.Entities
{
	/// <summary>
	/// Represents a scene entity.
	/// </summary>
	public class SceneEntity : MonoBehaviour
	{
		[SerializeField] // FOR DEBUG PURPOSES ONLY
		private string guid;

		public List<SceneEntity> childEntities;

		public Guid GUID
		{
			get => Guid.Parse(guid);
		}

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (string.IsNullOrEmpty(guid))
            {
                guid = IDAllocationsFactory.BuildGUID().ToString();

                UnityEditor.Undo.RecordObject(this, "Assigned GUID");
            }

            EditorUtility.SetDirty(this);
        }
#endif
	}
}