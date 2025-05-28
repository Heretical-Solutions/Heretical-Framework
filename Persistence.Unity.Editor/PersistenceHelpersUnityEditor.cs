using System;

using HereticalSolutions.Persistence.Builders;
using HereticalSolutions.Persistence.Unity.Editor.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Unity.Editor
{
	public static class PersistenceHelpersUnityEditor
	{
		public static SerializerBuilder AsEditorPrefs(
			this SerializerBuilder builder,
			string keyPrefs,
			PersistenceFactoryUnityEditor persistenceFactoryUnityEditor)
		{
			var context = builder.Context;

			if (context.DeferredBuildSerializationMediumDelegate != null)
			{
				throw new Exception(
					context.Logger.TryFormatException(
						context.GetType(),
						$"SERIALIZATION MEDIUM IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			context.DeferredBuildSerializationMediumDelegate = () =>
			{
				if (context.SerializerContext.SerializationMedium != null)
				{
					throw new Exception(
						context.Logger.TryFormatException(
							context.GetType(),
							$"SERIALIZATION MEDIUM IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				if (!context.SerializerContext.Arguments.Has<IPathArgument>())
				{
					throw new Exception(
						context.Logger.TryFormatException(
							context.GetType(),
							"PATH ARGUMENT MISSING"));
				}

				context.SerializerContext.SerializationMedium =
					persistenceFactoryUnityEditor.BuildEditorPrefsMedium(
						keyPrefs);
			};

			return builder;
		}

		public static SerializerBuilder AsEditorPrefsWithUUID<TUUID>(
			this SerializerBuilder builder,
			string keyPrefsSerializedValuesList,
			string keyPrefsValuePrefix,
			TUUID uuid,
			PersistenceFactoryUnityEditor persistenceFactoryUnityEditor)
		{
			var context = builder.Context;

			if (context.DeferredBuildSerializationMediumDelegate != null)
			{
				throw new Exception(
					context.Logger.TryFormatException(
						context.GetType(),
						$"SERIALIZATION MEDIUM IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			context.DeferredBuildSerializationMediumDelegate = () =>
			{
				if (context.SerializerContext.SerializationMedium != null)
				{
					throw new Exception(
						context.Logger.TryFormatException(
							context.GetType(),
							$"SERIALIZATION MEDIUM IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				if (!context.SerializerContext.Arguments.Has<IPathArgument>())
				{
					throw new Exception(
						context.Logger.TryFormatException(
							context.GetType(),
							"PATH ARGUMENT MISSING"));
				}

				context.SerializerContext.SerializationMedium =
					persistenceFactoryUnityEditor.BuildEditorPrefsWithUUIDMedium<TUUID>(
						keyPrefsSerializedValuesList,
						keyPrefsValuePrefix,
						uuid);
			};

			return builder;
		}
	}
}