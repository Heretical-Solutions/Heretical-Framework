using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Unity.Editor.Factories
{
	public class PersistenceFactoryUnityEditor
	{
		private readonly ILoggerResolver loggerResolver;

		public PersistenceFactoryUnityEditor(
			ILoggerResolver loggerResolver)
		{
			this.loggerResolver = loggerResolver;
		}

		#region Serialization mediums

		public EditorPrefsMedium BuildEditorPrefsMedium(
			string keyPrefs)
		{
			return new EditorPrefsMedium(
				keyPrefs,
				loggerResolver?.GetLogger<EditorPrefsMedium>());
		}

		public EditorPrefsWithUUIDMedium<TUUID> BuildEditorPrefsWithUUIDMedium<TUUID>(
			string keyPrefsSerializedValuesList,
			string keyPrefsValuePrefix,
			TUUID uuid)
		{
			return new EditorPrefsWithUUIDMedium<TUUID>(
				keyPrefsSerializedValuesList,
				keyPrefsValuePrefix,
				uuid,
				loggerResolver?.GetLogger<EditorPrefsWithUUIDMedium<TUUID>>());
		}

		#endregion
	}
}