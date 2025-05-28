using UnityEngine;

namespace HereticalSolutions.ObjectPools.Decorators.Unity
{
	public class ParentTransformArgument : IPoolPopArgument
	{
		public Transform Parent;

		public bool WorldPositionStays = true;
	}
}