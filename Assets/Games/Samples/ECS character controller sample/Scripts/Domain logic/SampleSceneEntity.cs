using System;

using HereticalSolutions.Entities;

using HereticalSolutions.Allocations.Factories;

namespace HereticalSolutions.Sample.ECSCharacterControllerSample
{
	public class SampleSceneEntity :
		ASceneEntity<Guid>
	{
		public override Guid EntityID { get => Guid.Parse(persistentID); }

#if UNITY_EDITOR
		protected override Guid AllocateID()
		{
			return IDAllocationsFactory.BuildGUID();
		}
#endif
	}
}