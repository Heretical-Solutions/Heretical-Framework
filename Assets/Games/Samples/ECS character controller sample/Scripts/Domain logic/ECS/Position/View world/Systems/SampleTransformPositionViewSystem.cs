using DefaultEcs;
using DefaultEcs.System;

using UnityEngine;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleTransformPositionViewSystem : AEntitySetSystem<float>
	{
		public SampleTransformPositionViewSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<SampleTransformPositionViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var sampleTransformPositionViewComponent = ref entity.Get<SampleTransformPositionViewComponent>();

			if (!sampleTransformPositionViewComponent.Dirty)
			{
				return;
			}

			sampleTransformPositionViewComponent.PositionTransform.position = new Vector3(
				sampleTransformPositionViewComponent.Position.x,
				0f,
				sampleTransformPositionViewComponent.Position.y);

			sampleTransformPositionViewComponent.Dirty = false;
		}
	}
}