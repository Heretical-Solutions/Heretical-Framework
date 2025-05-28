using System;

using HereticalSolutions.Builders;

using HereticalSolutions.Persistence.Factories;

using HereticalSolutions.Metadata.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Builders
{
	public class SerializerBuilder
		: ABuilder<SerializerBuilderContext>
	{
		private readonly PersistenceFactory persistenceFactory;

		private readonly MetadataFactory metadataFactory;

		private readonly ILogger logger;

		public SerializerBuilder(
			PersistenceFactory persistenceFactory,
			MetadataFactory metadataFactory,
			ILogger logger)
		{
			this.persistenceFactory = persistenceFactory;

			this.metadataFactory = metadataFactory;

			this.logger = logger;

			this.context = null;
		}

		#region SerializerBuilder

		public SerializerBuilder NewSerializer()
		{
			context = new SerializerBuilderContext
			{
				SerializerContext = new SerializerContext(),

				MetadataFactory = metadataFactory,

				Logger = logger
			};

			return this;
		}

		public SerializerBuilder RecycleSerializer(
			ISerializer serializer)
		{
			context = new SerializerBuilderContext
			{
				SerializerContext = serializer.Context as ISerializerContext,

				Logger = logger
			};

			return this;
		}

		#region Visitor

		public SerializerBuilder WithVisitor(
			IVisitor visitor)
		{
			if (context.SerializerContext.Visitor != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"VISITOR IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			context.SerializerContext.Visitor = visitor;

			return this;
		}

		public SerializerBuilder EraseVisitor()
		{
			context.SerializerContext.Visitor = null;

			return this;
		}

		#endregion

		#region Path settings

		public SerializerBuilder FromAbsolutePath(
			FileAtAbsolutePathSettings filePathSettings)
		{
			context.EnsureArgumentsExist();

			if (!context.SerializerContext.Arguments.TryAdd<IPathArgument>(
				persistenceFactory.BuildPathArgument(
					filePathSettings.FullPath)))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"PATH ARGUMENT IS ALREADY PRESENT: {context.SerializerContext.Arguments.Get<IPathArgument>().Path}. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			return this;
		}

		public SerializerBuilder FromRelativePath(
			FileAtRelativePathSettings filePathSettings)
		{
			context.EnsureArgumentsExist();

			if (!context.SerializerContext.Arguments.TryAdd<IPathArgument>(
				persistenceFactory.BuildPathArgument(
					filePathSettings.FullPath)))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"PATH ARGUMENT IS ALREADY PRESENT: {context.SerializerContext.Arguments.Get<IPathArgument>().Path}. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			return this;
		}

		public SerializerBuilder FromTempPath(
			FileAtTempPathSettings filePathSettings)
		{
			context.EnsureArgumentsExist();

			if (!context.SerializerContext.Arguments.TryAdd<IPathArgument>(
				persistenceFactory.BuildPathArgument(
					filePathSettings.FullPath)))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"PATH ARGUMENT IS ALREADY PRESENT: {context.SerializerContext.Arguments.Get<IPathArgument>().Path}. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			return this;
		}

		public SerializerBuilder From<TPathSettings>(
			TPathSettings pathSettings)
			where TPathSettings : IPathSettings
		{
			context.EnsureArgumentsExist();

			var pathSettingsCasted = pathSettings as IPathSettings;

			if (!context.SerializerContext.Arguments.TryAdd<IPathArgument>(
				persistenceFactory.BuildPathArgument(
					pathSettingsCasted.FullPath)))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"PATH ARGUMENT IS ALREADY PRESENT: {context.SerializerContext.Arguments.Get<IPathArgument>().Path}. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			return this;
		}

		public SerializerBuilder ErasePath()
		{
			context.EnsureArgumentsExist();

			context.SerializerContext.Arguments.TryRemove<IPathArgument>();

			return this;
		}

		#endregion

		#region Format serializer

		public SerializerBuilder ToObject()
		{
			if (context.DeferredBuildFormatSerializerDelegate != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"FORMAT SERIALIZER IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			context.DeferredBuildFormatSerializerDelegate = () =>
			{
				if (context.SerializerContext.FormatSerializer != null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"FORNMAT SERIALIZER IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				context.SerializerContext.FormatSerializer =
					persistenceFactory.BuildObjectSerializer();
			};

			return this;
		}

		public SerializerBuilder ToBinary()
		{
			if (context.DeferredBuildFormatSerializerDelegate != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"FORMAT SERIALIZER IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			context.DeferredBuildFormatSerializerDelegate = () =>
			{
				if (context.SerializerContext.FormatSerializer != null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"FORNMAT SERIALIZER IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				context.SerializerContext.FormatSerializer =
					persistenceFactory.BuildBinaryFormatterSerializer();
			};

			return this;
		}

		public SerializerBuilder To<TFormatSerializer>(
			object[] arguments)
			where TFormatSerializer : IFormatSerializer
		{
			if (context.DeferredBuildFormatSerializerDelegate != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"FORMAT SERIALIZER IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			context.DeferredBuildFormatSerializerDelegate = () =>
			{
				if (context.SerializerContext.FormatSerializer != null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"FORNMAT SERIALIZER IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				context.SerializerContext.FormatSerializer =
					persistenceFactory.BuildFormatSerializer<TFormatSerializer>(
						arguments);
			};

			return this;
		}

		public SerializerBuilder EraseFormatSerializer()
		{
			context.SerializerContext.FormatSerializer = null;

			return this;
		}

		#endregion

		#region Data converters

		public SerializerBuilder WithByteArrayFallback()
		{
			context.DeferredBuildDataConverterDelegate += () =>
			{
				if (context.SerializerContext.DataConverter == null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"DATA CONVERTER IS NULL"));
				}

				context.SerializerContext.DataConverter =
					persistenceFactory.BuildByteArrayFallbackConverter(
						context.SerializerContext.DataConverter,
						null,
						null);
			};

			return this;
		}

		public SerializerBuilder WithDataConverter<TDataConverter>(
			object[] arguments)
			where TDataConverter : IDataConverter
		{
			context.EnsureArgumentsExist();

			if (!context.SerializerContext.Arguments.Has<IDataConversionArgument>())
			{
				context.SerializerContext.Arguments.TryAdd<IDataConversionArgument>(
					persistenceFactory.BuildDataConversionArgument());
			}

			context.DeferredBuildDataConverterDelegate += () =>
			{
				if (context.SerializerContext.DataConverter == null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"DATA CONVERTER IS NULL"));
				}

				context.SerializerContext.DataConverter =
					persistenceFactory.BuildDataConverter<TDataConverter>(
						context.SerializerContext.DataConverter,
						arguments);
			};

			return this;
		}

		public SerializerBuilder EraseDataConverters()
		{
			context.SerializerContext.DataConverter = null;

			return this;
		}

		#endregion

		#region Serialization mediums

		public SerializerBuilder AsString(
			Func<string> valueGetter,
			Action<string> valueSetter)
		{
			if (context.DeferredBuildSerializationMediumDelegate != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"SERIALIZATION MEDIUM IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			context.DeferredBuildSerializationMediumDelegate = () =>
			{
				if (context.SerializerContext.SerializationMedium != null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"SERIALIZATION MEDIUM IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				context.SerializerContext.SerializationMedium =
					persistenceFactory.BuildStringMedium(
						valueGetter,
						valueSetter);
			};

			return this;
		}

		public SerializerBuilder AsCachedString()
		{
			if (context.DeferredBuildSerializationMediumDelegate != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"SERIALIZATION MEDIUM IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			context.DeferredBuildSerializationMediumDelegate = () =>
			{
				if (context.SerializerContext.SerializationMedium != null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"SERIALIZATION MEDIUM IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				context.SerializerContext.SerializationMedium =
					persistenceFactory.BuildCachedStringMedium();
			};

			return this;
		}

		public SerializerBuilder AsTextFile()
		{
			if (context.DeferredBuildSerializationMediumDelegate != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"SERIALIZATION MEDIUM IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			context.DeferredBuildSerializationMediumDelegate = () =>
			{
				if (context.SerializerContext.SerializationMedium != null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"SERIALIZATION MEDIUM IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				if (!context.SerializerContext.Arguments.Has<IPathArgument>())
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							"PATH ARGUMENT MISSING"));
				}
	
				context.SerializerContext.SerializationMedium =
					persistenceFactory.BuildTextFileMedium(
						context.SerializerContext.Arguments.Get<IPathArgument>().Path);
			};

			return this;
		}

		public SerializerBuilder AsBinaryFile()
		{
			if (context.DeferredBuildSerializationMediumDelegate != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"SERIALIZATION MEDIUM IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			context.DeferredBuildSerializationMediumDelegate = () =>
			{
				if (context.SerializerContext.SerializationMedium != null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"SERIALIZATION MEDIUM IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				if (!context.SerializerContext.Arguments.Has<IPathArgument>())
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							"PATH ARGUMENT MISSING"));
				}
	
				context.SerializerContext.SerializationMedium =
					persistenceFactory.BuildBinaryFileMedium(
						context.SerializerContext.Arguments.Get<IPathArgument>().Path);
			};

			return this;
		}

		public SerializerBuilder AsTextStream(
			bool flushAutomatically = true)
		{
			if (context.DeferredBuildSerializationMediumDelegate != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"SERIALIZATION MEDIUM IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			context.DeferredBuildSerializationMediumDelegate = () =>
			{
				if (context.SerializerContext.SerializationMedium != null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"SERIALIZATION MEDIUM IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				if (!context.SerializerContext.Arguments.Has<IPathArgument>())
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							"PATH ARGUMENT MISSING"));
				}
	
				context.SerializerContext.SerializationMedium =
					persistenceFactory.BuildTextStreamMedium(
						context.SerializerContext.Arguments.Get<IPathArgument>().Path,
						flushAutomatically);
			};

			return this;
		}

		public SerializerBuilder AsFileStream(
			bool flushAutomatically = true)
		{
			if (context.DeferredBuildSerializationMediumDelegate != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"SERIALIZATION MEDIUM IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			context.DeferredBuildSerializationMediumDelegate = () =>
			{
				if (context.SerializerContext.SerializationMedium != null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"SERIALIZATION MEDIUM IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				if (!context.SerializerContext.Arguments.Has<IPathArgument>())
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							"PATH ARGUMENT MISSING"));
				}
	
				context.SerializerContext.SerializationMedium =
					persistenceFactory.BuildFileStreamMedium(
						context.SerializerContext.Arguments.Get<IPathArgument>().Path,
						flushAutomatically);
			};

			return this;
		}

		public SerializerBuilder AsMemoryStream(
			byte[] buffer = null,
			int index = -1,
			int count = -1)
		{
			if (context.DeferredBuildSerializationMediumDelegate != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"SERIALIZATION MEDIUM IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			context.DeferredBuildSerializationMediumDelegate = () =>
			{
				if (context.SerializerContext.SerializationMedium != null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"SERIALIZATION MEDIUM IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				if (buffer != null)
				{
					if (!context.SerializerContext.Arguments.TryAdd<ISourceByteArrayArgument>(
						persistenceFactory.BuildSourceByteArrayArgument(
							buffer)))
					{
						throw new Exception(
							logger.TryFormatException(
								GetType(),
								$"SOURCE BYTE ARRAY ARGUMENT IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
					}
				}

				context.SerializerContext.SerializationMedium =
					persistenceFactory.BuildMemoryStreamMedium(
						buffer,
						index,
						count);
			};

			return this;
		}

		public SerializerBuilder AsIsolatedStorageFileStream(
			bool flushAutomatically = true)
		{
			if (context.DeferredBuildSerializationMediumDelegate != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"SERIALIZATION MEDIUM IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			context.DeferredBuildSerializationMediumDelegate = () =>
			{
				if (context.SerializerContext.SerializationMedium != null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"SERIALIZATION MEDIUM IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				if (!context.SerializerContext.Arguments.Has<IPathArgument>())
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							"PATH ARGUMENT MISSING"));
				}
	
				context.SerializerContext.SerializationMedium =
					persistenceFactory.BuildIsolatedStorageMedium(
						context.SerializerContext.Arguments.Get<IPathArgument>().Path,
						flushAutomatically);
			};

			return this;
		}

		public SerializerBuilder As<TSerializationMedium>(
			object[] arguments)
			where TSerializationMedium : ISerializationMedium
		{
			if (context.DeferredBuildSerializationMediumDelegate != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"SERIALIZATION MEDIUM IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			context.DeferredBuildSerializationMediumDelegate = () =>
			{
				if (context.SerializerContext.SerializationMedium != null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"SERIALIZATION MEDIUM IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				context.SerializerContext.SerializationMedium =
					persistenceFactory.BuildSerializationMedium<TSerializationMedium>(
						arguments);
			};

			return this;
		}

		public SerializerBuilder EraseMedium()
		{
			context.SerializerContext.SerializationMedium = null;

			return this;
		}

		#endregion

		#region Arguments

		public SerializerBuilder WithPath(
			string path)
		{
			context.EnsureArgumentsExist();

			if (!context.SerializerContext.Arguments.TryAdd<IPathArgument>(
				persistenceFactory.BuildPathArgument(
					path)))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"PATH ARGUMENT IS ALREADY PRESENT: {context.SerializerContext.Arguments.Get<IPathArgument>().Path}. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			return this;
		}

		public SerializerBuilder WithAppend()
		{
			context.EnsureArgumentsExist();

			if (!context.SerializerContext.Arguments.TryAdd<IAppendArgument>(
				persistenceFactory.BuildAppendArgument()))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"APPEND ARGUMENT IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			return this;
		}

		public SerializerBuilder WithReadWriteAccess()
		{
			context.EnsureArgumentsExist();

			if (!context.SerializerContext.Arguments.TryAdd<IReadAndWriteAccessArgument>(
				persistenceFactory.BuildReadAndWriteAccessArgument()))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"READ WRITE ACCESS ARGUMENT IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			return this;
		}

		public SerializerBuilder WithBlockSerialization()
		{
			context.EnsureArgumentsExist();

			if (!context.SerializerContext.Arguments.TryAdd<IBlockSerializationArgument>(
				persistenceFactory.BuildBlockSerializationArgument()))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"BLOCK SERIALIZATION IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			return this;
		}

		public SerializerBuilder WithFallbackToSync()
		{
			context.EnsureArgumentsExist();

			if (!context.SerializerContext.Arguments.TryAdd<IFallbackToSyncArgument>(
				persistenceFactory.BuildFallbackToSyncArgument()))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"FALLBHACK TO SYNC ARGUMENT IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			return this;
		}

		public SerializerBuilder With<TInterface, TArgument>(
			object[] arguments)
			where TArgument : TInterface
		{
			context.EnsureArgumentsExist();

			if (!context.SerializerContext.Arguments.TryAdd<TInterface>(
				persistenceFactory.BuildSerializationArgument<TArgument>(
					arguments)))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"{nameof(TInterface)} ARGUMENT IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			return this;
		}

		public SerializerBuilder Without<TInterface>()
		{
			context.EnsureArgumentsExist();

			context.SerializerContext.Arguments.TryRemove<TInterface>();

			return this;
		}

		public SerializerBuilder EraseArguments()
		{
			context.EnsureArgumentsExist();

			context.SerializerContext.Arguments.Clear();

			return this;
		}

		#endregion

		#region Build

		public Serializer BuildSerializer()
		{
			AssembleContext();
			
			var result = persistenceFactory.BuildSerializer(
				context.SerializerContext);

			Cleanup();

			return result;
		}

		public ConcurrentSerializer BuildConcurrentSerializer()
		{
			AssembleContext();

			var result = persistenceFactory.BuildConcurrentSerializer(
				context.SerializerContext);

			Cleanup();

			return result;
		}

		#endregion

		public void Refurbish()
		{
			AssembleContext();
		}

		#endregion

		private void AssembleContext()
		{
			if (context.SerializerContext == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"SERIALIZER CONTEXT MISSING. PLEASE SPECIFY A SERIALIZER CONTEXT BEFORE BUILDING"));
			}

			context.EnsureArgumentsExist();

			if (context.DeferredBuildSerializationMediumDelegate != null)
			{
				context.DeferredBuildSerializationMediumDelegate();

				context.DeferredBuildSerializationMediumDelegate = null;
			}

			if (context.DeferredBuildFormatSerializerDelegate != null)
			{
				context.DeferredBuildFormatSerializerDelegate();

				context.DeferredBuildFormatSerializerDelegate = null;
			}

			if (context.SerializerContext.FormatSerializer == null)
			{
				context.SerializerContext.FormatSerializer =
					persistenceFactory.BuildObjectSerializer();
			}

			if (context.DeferredBuildDataConverterDelegate != null)
			{
				context.SerializerContext.DataConverter =
					persistenceFactory.BuildInvokeMediumConverter();

				context.DeferredBuildDataConverterDelegate();

				context.DeferredBuildDataConverterDelegate = null;

				//if (serializerContext.DataConverter.GetType()
				//	!= typeof(ByteArrayFallbackConverter))
				//{
				//	serializerContext.DataConverter =
				//		persistenceFactory.BuildByteArrayFallbackConverter(
				//			serializerContext.DataConverter,
				//			null,
				//			null);
				//}
			}

			if (context.SerializerContext.DataConverter == null)
			{
				context.SerializerContext.DataConverter =
					persistenceFactory.BuildInvokeMediumConverter();

				//serializerContext.DataConverter =
				//	persistenceFactory.BuildByteArrayFallbackConverter(
				//		serializerContext.DataConverter,
				//		null,
				//		null);
			}

			if (context.SerializerContext.SerializationMedium == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"SERIALIZATION MEDIUM MISSING. PLEASE SPECIFY A SERIALIZATION MEDIUM BEFORE BUILDING"));
			}

			if (context.SerializerContext.Visitor == null)
			{
				context.SerializerContext.Visitor = persistenceFactory.
					BuildDefaultDispatchVisitor();
			}
		}
	}
}