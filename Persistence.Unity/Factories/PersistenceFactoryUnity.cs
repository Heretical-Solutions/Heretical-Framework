using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Unity.Factories
{
	public class PersistenceFactoryUnity
	{
		private readonly ILoggerResolver loggerResolver;

		public PersistenceFactoryUnity(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		#region Serialization mediums

		public PlayerPrefsMedium BuildPlayerPrefsMedium(
			string keyPrefs)
		{
			return new PlayerPrefsMedium(
				keyPrefs,
				loggerResolver?.GetLogger<PlayerPrefsMedium>());
		}

		#endregion
	}
}