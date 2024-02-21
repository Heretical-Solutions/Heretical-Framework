using System.Collections.Generic;

using UnityEngine;

namespace HereticalSolutions.Entities
{
	public abstract class ASceneEntity : MonoBehaviour
	{
		[SerializeField]
		private string prototypeID;

		public List<ASceneEntity> childEntities;

		public string PrototypeID
		{
			get => prototypeID;
		}
	}
}