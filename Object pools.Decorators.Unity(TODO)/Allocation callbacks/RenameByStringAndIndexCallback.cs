using HereticalSolutions.Allocations;

using UnityEngine;

namespace HereticalSolutions.ObjectPools.Decorators.Unity
{
	public class RenameByStringAndIndexCallback
		: IAllocationCallback<GameObject>
	{
		private readonly string name;

		private int index = 0;

		public RenameByStringAndIndexCallback(string name)
		{
			this.name = name;
		}

		public void OnAllocated(
			GameObject instance)
		{
			if (instance == null)
				return;

			instance.name = $"{name} {index.ToString()}";

			index++;
		}
	}
}