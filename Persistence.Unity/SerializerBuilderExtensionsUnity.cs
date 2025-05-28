using System;

using HereticalSolutions.Persistence.Builders;
using HereticalSolutions.Persistence.Factories;
using HereticalSolutions.Persistence.Unity.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Unity
{
	public static class SerializerBuilderExtensionsUnity
	{
		public static SerializerBuilder FromApplicationDataPath(
			this SerializerBuilder builder,
			FileAtApplicationDataPathSettings filePathSettings,
			PersistenceFactory persistenceFactory)
		{
			var context = builder.Context;

			context.EnsureArgumentsExist();

			if (!context.SerializerContext.Arguments.TryAdd<IPathArgument>(
				persistenceFactory.BuildPathArgument(
					filePathSettings.FullPath)))
			{
				throw new Exception(
					context.Logger.TryFormatException(
						context.GetType(),
						$"PATH ARGUMENT IS ALREADY PRESENT: {context.SerializerContext.Arguments.Get<IPathArgument>().Path}. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			return builder;
		}

		public static SerializerBuilder FromPersistentDataPath(
			this SerializerBuilder builder,
			FileAtPersistentDataPathSettings filePathSettings,
			PersistenceFactory persistenceFactory)
		{
			var context = builder.Context;

			context.EnsureArgumentsExist();

			if (!context.SerializerContext.Arguments.TryAdd<IPathArgument>(
				persistenceFactory.BuildPathArgument(
					filePathSettings.FullPath)))
			{
				throw new Exception(
					context.Logger.TryFormatException(
						context.GetType(),
						$"PATH ARGUMENT IS ALREADY PRESENT: {context.SerializerContext.Arguments.Get<IPathArgument>().Path}. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			return builder;
		}

		public static SerializerBuilder AsPlayerPrefs(
			this SerializerBuilder builder,
			string keyPrefs,
			PersistenceFactoryUnity persistenceFactoryUnity)
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
					persistenceFactoryUnity.BuildPlayerPrefsMedium(
						keyPrefs);
			};

			return builder;
		}
	}
}