using HereticalSolutions.Pools;

using HereticalSolutions.Sample.ECSCharacterControllerSample.Factories;

using HereticalSolutions.Logging;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Sample.ECSCharacterControllerSample.Installers
{
	public class SampleGameObjectPoolsInstaller : MonoInstaller
	{
		[Inject]
		private ILoggerResolver loggerResolver;

		[SerializeField]
		private SampleGameObjectPoolSettings prefabsPoolsSettings;

		public override void InstallBindings()
		{
			var prefabsPool = SampleGameObjectPoolsFactory.BuildPool(
				Container,
				prefabsPoolsSettings,
				null,
				loggerResolver);

			Container
				.Bind<INonAllocDecoratedPool<GameObject>>()
				.FromInstance(prefabsPool)
				.AsCached();
		}
	}
}