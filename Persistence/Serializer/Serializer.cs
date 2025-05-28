using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public class Serializer
		: ISerializer,
		  IBlockSerializer,
		  IAsyncSerializer,
		  IAsyncBlockSerializer
	{
		private readonly IReadOnlySerializerContext context;

		private readonly ILogger logger;

		public Serializer(
			IReadOnlySerializerContext context,
			ILogger logger)
		{
			this.context = context;

			this.logger = logger;
		}

		#region ISerializer

		#region IHasIODestination

		public void EnsureIODestinationExists()
		{
			if (context.SerializationMedium is
				IHasIODestination mediumWithIODestination)
			{
				mediumWithIODestination.EnsureIODestinationExists();
			}
		}

		public bool IODestinationExists()
		{
			if (context.SerializationMedium is
				IHasIODestination mediumWithIODestination)
			{
				return mediumWithIODestination.IODestinationExists();
			}

			return false;
		}

		public void CreateIODestination()
		{
			if (context.SerializationMedium is
				IHasIODestination mediumWithIODestination)
			{
				mediumWithIODestination.CreateIODestination();
			}
		}

		public void EraseIODestination()
		{
			if (context.SerializationMedium is
				IHasIODestination mediumWithIODestination)
			{
				mediumWithIODestination.EraseIODestination();
			}
		}

		#endregion

		#region IHasReadWriteControl

		public bool SupportsSimultaneousReadAndWrite
		{
			get
			{
				if (context.SerializationMedium is
					IHasReadWriteControl mediumWithReadWriteControl)
				{
					return mediumWithReadWriteControl.SupportsSimultaneousReadAndWrite;
				}

				return false;
			}
		}

		public void InitializeRead()
		{
			if (context.SerializationMedium is
				IHasReadWriteControl mediumWithReadWriteControl)
			{
				mediumWithReadWriteControl.InitializeRead();
			}
		}

		public void FinalizeRead()
		{
			if (context.SerializationMedium is
				IHasReadWriteControl mediumWithReadWriteControl)
			{
				mediumWithReadWriteControl.FinalizeRead();
			}
		}


		public void InitializeWrite()
		{
			if (context.SerializationMedium is
				IHasReadWriteControl mediumWithReadWriteControl)
			{
				mediumWithReadWriteControl.InitializeWrite();
			}
		}

		public void FinalizeWrite()
		{
			if (context.SerializationMedium is
				IHasReadWriteControl mediumWithReadWriteControl)
			{
				mediumWithReadWriteControl.FinalizeWrite();
			}
		}


		public void InitializeAppend()
		{
			if (context.SerializationMedium is
				IHasReadWriteControl mediumWithReadWriteControl)
			{
				mediumWithReadWriteControl.InitializeAppend();
			}
		}

		public void FinalizeAppend()
		{
			if (context.SerializationMedium is
				IHasReadWriteControl mediumWithReadWriteControl)
			{
				mediumWithReadWriteControl.FinalizeAppend();
			}
		}


		public void InitializeReadAndWrite()
		{
			if (context.SerializationMedium is
				IHasReadWriteControl mediumWithReadWriteControl)
			{
				mediumWithReadWriteControl.InitializeReadAndWrite();
			}
		}

		public void FinalizeReadAndWrite()
		{
			if (context.SerializationMedium is
				IHasReadWriteControl mediumWithReadWriteControl)
			{
				mediumWithReadWriteControl.FinalizeReadAndWrite();
			}
		}

		#endregion

		public IReadOnlySerializerContext Context { get => context; }

		public void EnsureMediumInitializedForDeserialization()
		{
			EnsureMediumInitializedForDeserialization(
				context);
		}

		public void EnsureMediumFinalizedForDeserialization()
		{
			EnsureMediumFinalizedForDeserialization(
				context);
		}

		public void EnsureMediumInitializedForSerialization()
		{
			EnsureMediumInitializedForSerialization(
				context);
		}

		public void EnsureMediumFinalizedForSerialization()
		{
			EnsureMediumFinalizedForSerialization(
				context);
		}

		#region Serialize

		public bool Serialize<TValue>(
			TValue value)
		{
			try
			{
				EnsureMediumInitializedForSerialization(
					context);

				if (TryGetDTO<TValue>(
					value,
					out var dto))
				{
					if (!context.FormatSerializer.Serialize(
						dto.GetType(),
						context,
						dto))
					{
						logger?.LogError(
							GetType(),
							$"SERIALIZATION FAILED: {dto.GetType().Name}");

						EnsureMediumFinalizedForSerialization(
							context);

						return false;
					}

					EnsureMediumFinalizedForSerialization(
						context);

					return true;
				}

				if (!context.FormatSerializer.Serialize<TValue>(
					context,
					value))
				{
					logger?.LogError(
						GetType(),
						$"SERIALIZATION FAILED: {typeof(TValue).Name}");

					EnsureMediumFinalizedForSerialization(
						context);

					return false;
				}

				EnsureMediumFinalizedForSerialization(
					context);

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE SERIALIZING: {exception.Message}");

				EnsureMediumFinalizedForSerialization(
					context);

				return false;
			}
		}

		public bool Serialize(
			Type valueType,
			object valueObject)
		{
			try
			{
				EnsureMediumInitializedForSerialization(
					context);

				if (TryGetDTO(
					valueType,
					valueObject,
					out var dto))
				{
					if (!context.FormatSerializer.Serialize(
						dto.GetType(),
						context,
						dto))
					{
						logger?.LogError(
							GetType(),
							$"SERIALIZATION FAILED: {dto.GetType().Name}");

						EnsureMediumFinalizedForSerialization(
							context);

						return false;
					}

					EnsureMediumFinalizedForSerialization(
						context);

					return true;
				}

				if (!context.FormatSerializer.Serialize(
					valueType,
					context,
					valueObject))
				{
					logger?.LogError(
						GetType(),
						$"SERIALIZATION FAILED: {valueType.Name}");

					EnsureMediumFinalizedForSerialization(
						context);

					return false;
				}

				EnsureMediumFinalizedForSerialization(
					context);

				return true;			
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE SERIALIZING: {exception.Message}");

				EnsureMediumFinalizedForSerialization(
					context);

				return false;
			}
		}

		#endregion

		#region Deserialize

		public bool Deserialize<TValue>(
			out TValue value)
		{
			value = default;

			try
			{
				EnsureMediumInitializedForDeserialization(
					context);

				if (TryGetLoadVisitor<TValue>(
					value,
					out var loadVisitor))
				{
					Type dtoType = loadVisitor.GetDTOType<TValue>(
						value);
	
					if (!context.FormatSerializer.Deserialize(
						dtoType,
						context,
						out object dto))
					{
						logger?.LogError(
							GetType(),
							$"DESERIALIZATION FAILED: {dtoType.Name}");

						EnsureMediumFinalizedForDeserialization(
							context);

						return false;
					}
	
					if (!loadVisitor.VisitLoad<TValue>(
						dto,
						out value,
						context.Visitor))
					{
						logger?.LogError(
							GetType(),
							$"VISIT LOAD FAILED: {typeof(TValue).Name}");

						EnsureMediumFinalizedForDeserialization(
							context);

						return false;
					}

					EnsureMediumFinalizedForDeserialization(
						context);

					return true;
				}

				if (!context.FormatSerializer.Deserialize<TValue>(
					context,
					out value))
				{
					logger?.LogError(
						GetType(),
						$"DESERIALIZATION FAILED: {typeof(TValue).Name}");

					EnsureMediumFinalizedForDeserialization(
						context);

					return false;
				}

				EnsureMediumFinalizedForDeserialization(
					context);

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE DESERIALIZING: {exception.Message}");

				EnsureMediumFinalizedForDeserialization(
					context);

				return false;
			}
		}

		public bool Deserialize(
			Type valueType,
			out object valueObject)
		{
			valueObject = default;

			try
			{
				EnsureMediumInitializedForDeserialization(
					context);

				if (TryGetLoadVisitor(
					valueType,
					out var loadVisitor))
				{
					Type dtoType = loadVisitor.GetDTOType(valueType);
	
					if (!context.FormatSerializer.Deserialize(
						dtoType,
						context,
						out object dto))
					{
						logger?.LogError(
							GetType(),
							$"DESERIALIZATION FAILED: {dtoType.Name}");

						EnsureMediumFinalizedForDeserialization(
							context);

						return false;
					}
	
					if (!loadVisitor.VisitLoad(
						dto,
						valueType,
						out valueObject,
						context.Visitor))
					{
						logger?.LogError(
							GetType(),
							$"VISIT LOAD FAILED: {valueType.Name}");

						EnsureMediumFinalizedForDeserialization(
							context);

						return false;
					}

					EnsureMediumFinalizedForDeserialization(
						context);

					return true;
				}

				if (!context.FormatSerializer.Deserialize(
					valueType,
					context,
					out valueObject))
				{
					logger?.LogError(
						GetType(),
						$"DESERIALIZATION FAILED: {valueType.Name}");

					EnsureMediumFinalizedForDeserialization(
						context);

					return false;
				}

				EnsureMediumFinalizedForDeserialization(
						context);

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE DESERIALIZING: {exception.Message}");

				EnsureMediumFinalizedForDeserialization(
					context);

				return false;
			}
		}

		#endregion

		#region Populate

		public bool Populate<TValue>(
			TValue value)
		{
			try
			{
				EnsureMediumInitializedForDeserialization(
					context);

				if (TryGetPopulateVisitor<TValue>(
					value,
					out var populateVisitor))
				{
					Type dtoType = populateVisitor.GetDTOType<TValue>(
						value);
	
					if (!context.FormatSerializer.Deserialize(
						dtoType,
						context,
						out object dto))
					{
						logger?.LogError(
							GetType(),
							$"DESERIALIZATION FAILED: {dtoType.Name}");

						EnsureMediumFinalizedForDeserialization(
							context);

						return false;
					}

					var result = TryPopulate<TValue>(
						value,
						dto,
						populateVisitor);

					EnsureMediumFinalizedForDeserialization(
						context);

					return result;
				}
				
				if (!context.FormatSerializer.Populate<TValue>(
					context,
					value))
				{
					logger?.LogError(
						GetType(),
						$"POPULATING FAILED: {typeof(TValue).Name}");

					EnsureMediumFinalizedForDeserialization(
						context);

					return false;
				}

				EnsureMediumFinalizedForDeserialization(
					context);

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE POPULATING: {exception.Message}");

				EnsureMediumFinalizedForDeserialization(
					context);

				return false;
			}
		}

		public bool Populate(
			Type valueType,
			object valueObject)
		{
			try
			{
				EnsureMediumInitializedForDeserialization(
					context);

				if (TryGetPopulateVisitor(
					valueType,
					out var populateVisitor))
				{
					Type dtoType = populateVisitor.GetDTOType(valueType);
	
					if (!context.FormatSerializer.Deserialize(
						dtoType,
						context,
						out object dto))
					{
						logger?.LogError(
							GetType(),
							$"DESERIALIZATION FAILED: {dtoType.Name}");

						EnsureMediumFinalizedForDeserialization(
							context);

						return false;
					}

					var result = TryPopulate(
						valueType,
						valueObject,
						dto,
						populateVisitor);

					EnsureMediumFinalizedForDeserialization(
						context);

					return result;
				}
				
				if (!context.FormatSerializer.Populate(
					valueType,
					context,
					valueObject))
				{
					logger?.LogError(
						GetType(),
						$"POPULATING FAILED: {valueType.Name}");

					EnsureMediumFinalizedForDeserialization(
						context);

					return false;
				}

				EnsureMediumFinalizedForDeserialization(
						context);

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE POPULATING: {exception.Message}");

				EnsureMediumFinalizedForDeserialization(
						context);

				return false;
			}
		}

		#endregion

		#endregion

		#region IBlockSerializer

		#region Serialize

		public bool SerializeBlock<TValue>(
			TValue value,
			int blockOffset,
			int blockSize)
		{
			try
			{
				var blockFormatSerializer = context.FormatSerializer
					as IBlockFormatSerializer;

				if (blockFormatSerializer == null)
				{
					logger?.LogError(
						GetType(),
						$"FORMAT SERIALIZER IS NOT A BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

					return false;
				}

				if (TryGetDTO<TValue>(
					value,
					out var dto))
				{
					if (!blockFormatSerializer.SerializeBlock(
						dto.GetType(),
						context,
						dto,
						blockOffset,
						blockSize))
					{
						logger?.LogError(
							GetType(),
							$"BLOCK SERIALIZATION FAILED: {dto.GetType().Name}");

						return false;
					}

					return true;
				}

				if (!blockFormatSerializer.SerializeBlock<TValue>(
					context,
					value,
					blockOffset,
					blockSize))
				{
					logger?.LogError(
						GetType(),
						$"BLOCK SERIALIZATION FAILED: {typeof(TValue).Name}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE SERIALIZING BLOCK: {exception.Message}");

				return false;
			}
		}

		public bool SerializeBlock(
			Type valueType,
			object valueObject,
			int blockOffset,
			int blockSize)
		{
			try
			{
				var blockFormatSerializer = context.FormatSerializer
					as IBlockFormatSerializer;

				if (blockFormatSerializer == null)
				{
					logger?.LogError(
						GetType(),
						$"FORMAT SERIALIZER IS NOT A BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

					return false;
				}

				if (TryGetDTO(
					valueType,
					valueObject,
					out var dto))
				{
					if (!blockFormatSerializer.SerializeBlock(
						dto.GetType(),
						context,
						dto,
						blockOffset,
						blockSize))
					{
						logger?.LogError(
							GetType(),
							$"BLOCK SERIALIZATION FAILED: {dto.GetType().Name}");

						return false;
					}

					return true;
				}

				if (!blockFormatSerializer.SerializeBlock(
					valueType,
					context,
					valueObject,
					blockOffset,
					blockSize))
				{
					logger?.LogError(
						GetType(),
						$"BLOCK SERIALIZATION FAILED: {valueType.Name}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE SERIALIZING BLOCK: {exception.Message}");

				return false;
			}
		}

		#endregion

		#region Deserialize

		public bool DeserializeBlock<TValue>(
			int blockOffset,
			int blockSize,
			out TValue value)
		{
			value = default;

			try
			{
				var blockFormatSerializer = context.FormatSerializer
					as IBlockFormatSerializer;

				if (blockFormatSerializer == null)
				{
					logger?.LogError(
						GetType(),
						$"FORMAT SERIALIZER IS NOT A BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

					return false;
				}

				if (TryGetLoadVisitor<TValue>(
					value,
					out var loadVisitor))
				{
					Type dtoType = loadVisitor.GetDTOType<TValue>(
						value);

					if (!blockFormatSerializer.DeserializeBlock(
						dtoType,
						context,
						blockOffset,
						blockSize,
						out object dto))
					{
						logger?.LogError(
							GetType(),
							$"BLOCK DESERIALIZATION FAILED: {dtoType.Name}");

						return false;
					}

					if (!loadVisitor.VisitLoad<TValue>(
						dto,
						out value,
						context.Visitor))
					{
						logger?.LogError(
							GetType(),
							$"VISIT LOAD FAILED: {typeof(TValue).Name}");

						return false;
					}

					return true;
				}

				if (!blockFormatSerializer.DeserializeBlock<TValue>(
					context,
					blockOffset,
					blockSize,
					out value))
				{
					logger?.LogError(
						GetType(),
						$"BLOCK DESERIALIZATION FAILED: {typeof(TValue).Name}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE DESERIALIZING BLOCK: {exception.Message}");

				return false;
			}
		}

		public bool DeserializeBlock(
			Type valueType,
			int blockOffset,
			int blockSize,
			out object valueObject)
		{
			valueObject = default;

			try
			{
				var blockFormatSerializer = context.FormatSerializer
					as IBlockFormatSerializer;

				if (blockFormatSerializer == null)
				{
					logger?.LogError(
						GetType(),
						$"FORMAT SERIALIZER IS NOT A BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

					return false;
				}

				if (TryGetLoadVisitor(
					valueType,
					out var loadVisitor))
				{
					Type dtoType = loadVisitor.GetDTOType(valueType);

					if (!blockFormatSerializer.DeserializeBlock(
						dtoType,
						context,
						blockOffset,
						blockSize,
						out object dto))
					{
						logger?.LogError(
							GetType(),
							$"BLOCK DESERIALIZATION FAILED: {dtoType.Name}");

						return false;
					}

					if (!loadVisitor.VisitLoad(
						dto,
						valueType,
						out valueObject,
						context.Visitor))
					{
						logger?.LogError(
							GetType(),
							$"VISIT LOAD FAILED: {valueType.Name}");

						return false;
					}

					return true;
				}

				if (!blockFormatSerializer.DeserializeBlock(
					valueType,
					context,
					blockOffset,
					blockSize,
					out valueObject))
				{
					logger?.LogError(
						GetType(),
						$"BLOCK DESERIALIZATION FAILED: {valueType.Name}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE DESERIALIZING BLOCK: {exception.Message}");

				return false;
			}
		}

		#endregion

		#region Populate

		public bool PopulateBlock<TValue>(
			TValue value,
			int blockOffset,
			int blockSize)
		{
			try
			{
				var blockFormatSerializer = context.FormatSerializer
					as IBlockFormatSerializer;

				if (blockFormatSerializer == null)
				{
					logger?.LogError(
						GetType(),
						$"FORMAT SERIALIZER IS NOT A BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

					return false;
				}

				if (TryGetPopulateVisitor<TValue>(
					value,
					out var populateVisitor))
				{
					Type dtoType = populateVisitor.GetDTOType<TValue>(
						value);

					if (!blockFormatSerializer.DeserializeBlock(
						dtoType,
						context,
						blockOffset,
						blockSize,
						out object dto))
					{
						logger?.LogError(
							GetType(),
							$"BLOCK DESERIALIZATION FAILED: {dtoType.Name}");

						return false;
					}

					return TryPopulate<TValue>(
						value,
						dto,
						populateVisitor);
				}

				if (!blockFormatSerializer.PopulateBlock<TValue>(
					context,
					value,
					blockOffset,
					blockSize))
				{
					logger?.LogError(
						GetType(),
						$"POPULATING BLOCK FAILED: {typeof(TValue).Name}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE POPULATING BLOCK: {exception.Message}");

				return false;
			}
		}

		public bool PopulateBlock(
			Type valueType,
			object valueObject,
			int blockOffset,
			int blockSize)
		{
			try
			{
				var blockFormatSerializer = context.FormatSerializer
					as IBlockFormatSerializer;

				if (blockFormatSerializer == null)
				{
					logger?.LogError(
						GetType(),
						$"FORMAT SERIALIZER IS NOT A BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

					return false;
				}

				if (TryGetPopulateVisitor(
					valueType,
					out var populateVisitor))
				{
					Type dtoType = populateVisitor.GetDTOType(valueType);

					if (!blockFormatSerializer.DeserializeBlock(
						dtoType,
						context,
						blockOffset,
						blockSize,
						out object dto))
					{
						logger?.LogError(
							GetType(),
							$"BLOCK DESERIALIZATION FAILED: {dtoType.Name}");

						return false;
					}

					return TryPopulate(
						valueType,
						valueObject,
						dto,
						populateVisitor);
				}

				if (!blockFormatSerializer.PopulateBlock(
					valueType,
					context,
					valueObject,
					blockOffset,
					blockSize))
				{
					logger?.LogError(
						GetType(),
						$"POPULATING BLOCK FAILED: {valueType.Name}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE POPULATING BLOCK: {exception.Message}");

				return false;
			}
		}

		#endregion

		#endregion

		#region IAsyncSerializer

		#region Serialize

		public async Task<bool> SerializeAsync<TValue>(
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			try
			{
				EnsureMediumInitializedForSerialization(
					context);

				var asyncFormatSerializer = context.FormatSerializer
					as IAsyncFormatSerializer;

				if (asyncFormatSerializer == null)
				{
					if (context.Arguments.Has<IFallbackToSyncArgument>())
					{
						var result = Serialize<TValue>(
							value);

						EnsureMediumFinalizedForSerialization(
							context);

						return result;
					}

					logger?.LogError(
						GetType(),
						$"FORMAT SERIALIZER IS NOT AN ASYNC FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

					EnsureMediumFinalizedForSerialization(
						context);

					return false;
				}

				if (TryGetDTO<TValue>(
					value,
					out var dto))
				{
					if (!await asyncFormatSerializer.SerializeAsync(
						dto.GetType(),
						context,
						dto,
						
						asyncContext)
						.ThrowExceptionsIfAny<bool, Serializer>(
							logger))
					{
						logger?.LogError(
							GetType(),
							$"SERIALIZATION FAILED: {dto.GetType().Name}");

						EnsureMediumFinalizedForSerialization(
							context);

						return false;
					}

					EnsureMediumFinalizedForSerialization(
						context);

					return true;
				}

				if (!await asyncFormatSerializer.SerializeAsync<TValue>(
					context,
					value,
					
					asyncContext)
					.ThrowExceptionsIfAny<bool, Serializer>(
						logger))
				{
					logger?.LogError(
						GetType(),
						$"SERIALIZATION FAILED: {typeof(TValue).Name}");

					EnsureMediumFinalizedForSerialization(
						context);

					return false;
				}

				EnsureMediumFinalizedForSerialization(
					context);

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE SERIALIZING: {exception.Message}");

				EnsureMediumFinalizedForSerialization(
					context);

				return false;
			}
		}

		public async Task<bool> SerializeAsync(
			Type valueType,
			object valueObject,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			try
			{
				EnsureMediumInitializedForSerialization(
					context);

				var asyncFormatSerializer = context.FormatSerializer
					as IAsyncFormatSerializer;

				if (asyncFormatSerializer == null)
				{
					if (context.Arguments.Has<IFallbackToSyncArgument>())
					{
						var result = Serialize(
							valueType,
							valueObject);

						EnsureMediumFinalizedForSerialization(
							context);

						return result;
					}

					logger?.LogError(
						GetType(),
						$"FORMAT SERIALIZER IS NOT AN ASYNC FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

					EnsureMediumFinalizedForSerialization(
						context);

					return false;
				}

				if (TryGetDTO(
					valueType,
					valueObject,
					out var dto))
				{
					if (!await asyncFormatSerializer.SerializeAsync(
						dto.GetType(),
						context,
						dto,
						
						asyncContext)
						.ThrowExceptionsIfAny<bool, Serializer>(
							logger))
					{
						logger?.LogError(
							GetType(),
							$"SERIALIZATION FAILED: {dto.GetType().Name}");

						EnsureMediumFinalizedForSerialization(
							context);

						return false;
					}

					EnsureMediumFinalizedForSerialization(
							context);

					return true;
				}

				if (!await asyncFormatSerializer.SerializeAsync(
					valueType,
					context,
					valueObject,
					
					asyncContext)
					.ThrowExceptionsIfAny<bool, Serializer>(
						logger))
				{
					logger?.LogError(
						GetType(),
						$"SERIALIZATION FAILED: {valueType.Name}");

					EnsureMediumFinalizedForSerialization(
						context);

					return false;
				}

				EnsureMediumFinalizedForSerialization(
					context);

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE SERIALIZING: {exception.Message}");

				EnsureMediumFinalizedForSerialization(
					context);

				return false;
			}
		}

		#endregion

		#region Deserialize

		public async Task<(bool, TValue)> DeserializeAsync<TValue>(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			TValue value = default;

			var asyncFormatSerializer = context.FormatSerializer
				as IAsyncFormatSerializer;

			if (asyncFormatSerializer == null)
			{
				if (context.Arguments.Has<IFallbackToSyncArgument>())
				{
					bool syncResult = Deserialize<TValue>(
						out value);

					return (syncResult, value);
				}

				logger?.LogError(
					GetType(),
					$"FORMAT SERIALIZER IS NOT AN ASYNC FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

				return (false, value);
			}

			try
			{
				EnsureMediumInitializedForDeserialization(
					context);

				if (TryGetLoadVisitor<TValue>(
					value,
					out var loadVisitor))
				{
					Type dtoType = loadVisitor.GetDTOType<TValue>(
						value);

					var asyncResult1 = await asyncFormatSerializer.DeserializeAsync(
						dtoType,
						context,

						asyncContext)
						.ThrowExceptionsIfAny<(bool, object), Serializer>(
							logger);

					if (!asyncResult1.Item1)
					{
						logger?.LogError(
							GetType(),
							$"DESERIALIZATION FAILED: {dtoType.Name}");

						EnsureMediumFinalizedForDeserialization(
							context);

						return (false, value);
					}

					if (!loadVisitor.VisitLoad<TValue>(
						asyncResult1.Item2,
						out value,
						context.Visitor))
					{
						logger?.LogError(
							GetType(),
							$"VISIT LOAD FAILED: {typeof(TValue).Name}");

						EnsureMediumFinalizedForDeserialization(
							context);

						return (false, value);
					}

					EnsureMediumFinalizedForDeserialization(
						context);

					return (true, value);
				}

				var asyncResult2 = await asyncFormatSerializer.DeserializeAsync<TValue>(
					context,
					
					asyncContext)
					.ThrowExceptionsIfAny<(bool, TValue), Serializer>(
						logger);

				if (!asyncResult2.Item1)
				{
					logger?.LogError(
						GetType(),
						$"DESERIALIZATION FAILED: {typeof(TValue).Name}");

					EnsureMediumFinalizedForDeserialization(
						context);

					return (false, value);
				}

				EnsureMediumFinalizedForDeserialization(
					context);

				return (true, asyncResult2.Item2);
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE DESERIALIZING: {exception.Message}");

				EnsureMediumFinalizedForDeserialization(
					context);

				return (false, value);
			}
		}

		public async Task<(bool, object)> DeserializeAsync(
			Type valueType,
			
			//Async tail
			AsyncExecutionContext asyncContext)                                       
		{
			object valueObject = default;

			var asyncFormatSerializer = context.FormatSerializer
				as IAsyncFormatSerializer;

			if (asyncFormatSerializer == null)
			{
				if (context.Arguments.Has<IFallbackToSyncArgument>())
				{
					bool syncResult = Deserialize(
						valueType,
						out valueObject);

					return (syncResult, valueObject);
				}

				logger?.LogError(
					GetType(),
					$"FORMAT SERIALIZER IS NOT AN ASYNC FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

				return (false, valueObject);
			}

			try
			{
				EnsureMediumInitializedForDeserialization(
					context);

				if (TryGetLoadVisitor(
					valueType,
					out var loadVisitor))
				{
					Type dtoType = loadVisitor.GetDTOType(valueType);

					var asyncResult1 = await asyncFormatSerializer.DeserializeAsync(
						dtoType,
						context,

						asyncContext)
						.ThrowExceptionsIfAny<(bool, object), Serializer>(
							logger);

					if (!asyncResult1.Item1)
					{
						logger?.LogError(
							GetType(),
							$"DESERIALIZATION FAILED: {dtoType.Name}");

						EnsureMediumFinalizedForDeserialization(
							context);

						return (false, valueObject);
					}

					if (!loadVisitor.VisitLoad(
						asyncResult1.Item2,
						valueType,
						out valueObject,
						context.Visitor))
					{
						logger?.LogError(
							GetType(),
							$"VISIT LOAD FAILED: {valueType.Name}");

						EnsureMediumFinalizedForDeserialization(
							context);

						return (false, valueObject);
					}

					EnsureMediumFinalizedForDeserialization(
						context);

					return (true, asyncResult1.Item2);
				}

				var asyncResult2 = await asyncFormatSerializer.DeserializeAsync(
					valueType,
					context,
					
					asyncContext)
					.ThrowExceptionsIfAny<(bool, object), Serializer>(
						logger);

				if (!asyncResult2.Item1)
				{
					logger?.LogError(
						GetType(),
						$"DESERIALIZATION FAILED: {valueType.Name}");

					EnsureMediumFinalizedForDeserialization(
						context);

					return (false, valueObject);
				}

				EnsureMediumFinalizedForDeserialization(
					context);

				return (true, asyncResult2.Item2);
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE DESERIALIZING: {exception.Message}");

				EnsureMediumFinalizedForDeserialization(
					context);

				return (false, default);
			}
		}

		#endregion

		#region Populate

		public async Task<bool> PopulateAsync<TValue>(
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			try
			{
				EnsureMediumInitializedForDeserialization(
					context);

				var asyncFormatSerializer = context.FormatSerializer
					as IAsyncFormatSerializer;

				if (asyncFormatSerializer == null)
				{
					if (context.Arguments.Has<IFallbackToSyncArgument>())
					{
						var result = Populate<TValue>(
							value);

						EnsureMediumFinalizedForDeserialization(
							context);

						return result;
					}

					logger?.LogError(
						GetType(),
						$"FORMAT SERIALIZER IS NOT AN ASYNC FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

					EnsureMediumFinalizedForDeserialization(
						context);

					return false;
				}

				if (TryGetPopulateVisitor<TValue>(
					value,
					out var populateVisitor))
				{
					Type dtoType = populateVisitor.GetDTOType<TValue>(
						value);

					var asyncResult1 = await asyncFormatSerializer.DeserializeAsync(
						dtoType,
						context,

						asyncContext)
						.ThrowExceptionsIfAny<(bool, object), Serializer>(
							logger);

					if (!asyncResult1.Item1)
					{
						logger?.LogError(
							GetType(),
							$"DESERIALIZATION FAILED: {dtoType.Name}");

						EnsureMediumFinalizedForDeserialization(
							context);

						return false;
					}

					var result = TryPopulate<TValue>(
						value,
						asyncResult1.Item2,
						populateVisitor);

					EnsureMediumFinalizedForDeserialization(
						context);

					return result;
				}

				if (!await asyncFormatSerializer.PopulateAsync<TValue>(
					context,
					value,

					asyncContext)
					.ThrowExceptionsIfAny<bool, Serializer>(
						logger))
				{
					logger?.LogError(
						GetType(),
						$"POPULATING FAILED: {typeof(TValue).Name}");

					EnsureMediumFinalizedForDeserialization(
						context);

					return false;
				}

				EnsureMediumFinalizedForDeserialization(
					context);

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE POPULATING: {exception.Message}");

				EnsureMediumFinalizedForDeserialization(
					context);

				return false;
			}
		}

		public async Task<bool> PopulateAsync(
			Type valueType,
			object valueObject,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			try
			{
				EnsureMediumInitializedForDeserialization(
					context);

				var asyncFormatSerializer = context.FormatSerializer
					as IAsyncFormatSerializer;

				if (asyncFormatSerializer == null)
				{
					if (context.Arguments.Has<IFallbackToSyncArgument>())
					{
						var result = Populate(
							valueType,
							valueObject);

						EnsureMediumFinalizedForDeserialization(
							context);

						return result;
					}

					logger?.LogError(
						GetType(),
						$"FORMAT SERIALIZER IS NOT AN ASYNC FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

					EnsureMediumFinalizedForDeserialization(
						context);

					return false;
				}

				if (TryGetPopulateVisitor(
					valueType,
					out var populateVisitor))
				{
					Type dtoType = populateVisitor.GetDTOType(valueType);

					var asyncResult1 = await asyncFormatSerializer.DeserializeAsync(
						dtoType,
						context,

						asyncContext)
						.ThrowExceptionsIfAny<(bool, object), Serializer>(
							logger);

					if (!asyncResult1.Item1)
					{
						logger?.LogError(
							GetType(),
							$"DESERIALIZATION FAILED: {dtoType.Name}");

						EnsureMediumFinalizedForDeserialization(
							context);

						return false;
					}

					var result = TryPopulate(
						valueType,
						valueObject,
						asyncResult1.Item2,
						populateVisitor);

					EnsureMediumFinalizedForDeserialization(
						context);

					return result;
				}

				if (!await asyncFormatSerializer.PopulateAsync(
					valueType,
					context,
					valueObject,

					asyncContext)
					.ThrowExceptionsIfAny<bool, Serializer>(
						logger))
				{
					logger?.LogError(
						GetType(),
						$"POPULATING FAILED: {valueType.Name}");

					EnsureMediumFinalizedForDeserialization(
						context);

					return false;
				}

				EnsureMediumFinalizedForDeserialization(
					context);

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE POPULATING: {exception.Message}");

				EnsureMediumFinalizedForDeserialization(
					context);

				return false;
			}
		}

		#endregion

		#endregion

		#region IAsyncBlockSerializer

		#region Serialize

		public async Task<bool> SerializeBlockAsync<TValue>(
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			try
			{
				var asyncBlockFormatSerializer = context.FormatSerializer
					as IAsyncBlockFormatSerializer;

				if (asyncBlockFormatSerializer == null)
				{
					if (context.Arguments.Has<IFallbackToSyncArgument>())
					{
						if (context.FormatSerializer is IBlockFormatSerializer)
						{
							return SerializeBlock<TValue>(
								value,
								blockOffset,
								blockSize);
						}
						
						logger?.LogError(
							GetType(),
							$"FORMAT SERIALIZER IS NOT A BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");
	
						return false;
					}

					logger?.LogError(
						GetType(),
						$"FORMAT SERIALIZER IS NOT AN ASYNC BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

					return false;
				}

				if (TryGetDTO<TValue>(
					value,
					out var dto))
				{
					if (!await asyncBlockFormatSerializer.SerializeBlockAsync(
						dto.GetType(),
						context,
						dto,
						blockOffset,
						blockSize,

						asyncContext)
						.ThrowExceptionsIfAny<bool, Serializer>(
							logger))
					{
						logger?.LogError(
							GetType(),
							$"BLOCK SERIALIZATION FAILED: {dto.GetType().Name}");

						return false;
					}

					return true;
				}

				if (!await asyncBlockFormatSerializer.SerializeBlockAsync<TValue>(
					context,
					value,
					blockOffset,
					blockSize,

					asyncContext)
					.ThrowExceptionsIfAny<bool, Serializer>(
						logger))
				{
					logger?.LogError(
						GetType(),
						$"BLOCK SERIALIZATION FAILED: {typeof(TValue).Name}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE SERIALIZING BLOCK: {exception.Message}");

				return false;
			}
		}

		public async Task<bool> SerializeBlockAsync(
			Type valueType,
			object valueObject,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			try
			{
				var asyncBlockFormatSerializer = context.FormatSerializer
					as IAsyncBlockFormatSerializer;

				if (asyncBlockFormatSerializer == null)
				{
					if (context.Arguments.Has<IFallbackToSyncArgument>())
					{
						if (context.FormatSerializer is IBlockFormatSerializer)
						{
							return SerializeBlock(
								valueType,
								valueObject,
								blockOffset,
								blockSize);
						}
					
						logger?.LogError(
							GetType(),
							$"FORMAT SERIALIZER IS NOT A BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

						return false;
					}

					logger?.LogError(
						GetType(),
						$"FORMAT SERIALIZER IS NOT AN ASYNC BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

					return false;
				}

				if (TryGetDTO(
					valueType,
					valueObject,
					out var dto))
				{
					if (!await asyncBlockFormatSerializer.SerializeBlockAsync(
						dto.GetType(),
						context,
						dto,
						blockOffset,
						blockSize,

						asyncContext)
						.ThrowExceptionsIfAny<bool, Serializer>(
							logger))
					{
						logger?.LogError(
							GetType(),
							$"BLOCK SERIALIZATION FAILED: {dto.GetType().Name}");

						return false;
					}

					return true;
				}

				if (!await asyncBlockFormatSerializer.SerializeBlockAsync(
					valueType,
					context,
					valueObject,
					blockOffset,
					blockSize,

					asyncContext)
					.ThrowExceptionsIfAny<bool, Serializer>(
						logger))
				{
					logger?.LogError(
						GetType(),
						$"BLOCK SERIALIZATION FAILED: {valueType.Name}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE SERIALIZING BLOCK: {exception.Message}");

				return false;
			}
		}

		#endregion

		#region Deserialize

		public async Task<(bool, TValue)> DeserializeBlockAsync<TValue>(
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			TValue value = default;

			var asyncBlockFormatSerializer = context.FormatSerializer
				as IAsyncBlockFormatSerializer;

			if (asyncBlockFormatSerializer == null)
			{
				if (context.Arguments.Has<IFallbackToSyncArgument>())
				{
					if (context.FormatSerializer is IBlockFormatSerializer)
					{
						bool syncResult = DeserializeBlock<TValue>(
							blockOffset,
							blockSize,
							out value);
	
						return (syncResult, value);
					}
				
					logger?.LogError(
						GetType(),
						$"FORMAT SERIALIZER IS NOT A BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

					return (false, value);
				}

				logger?.LogError(
					GetType(),
					$"FORMAT SERIALIZER IS NOT AN ASYNC BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

				return (false, value);
			}
			
			try
			{
				if (TryGetLoadVisitor<TValue>(
					value,
					out var loadVisitor))
				{
					Type dtoType = loadVisitor.GetDTOType<TValue>(
						value);

					var asyncResult1 = await asyncBlockFormatSerializer
						.DeserializeBlockAsync(
							dtoType,
							context,
							blockOffset,
							blockSize,
	
							asyncContext)
							.ThrowExceptionsIfAny<(bool, object), Serializer>(
								logger);

					if (!asyncResult1.Item1)
					{
						logger?.LogError(
							GetType(),
							$"BLOCK DESERIALIZATION FAILED: {dtoType.Name}");

						return (false, value);
					}

					if (!loadVisitor.VisitLoad<TValue>(
						asyncResult1.Item2,
						out value,
						context.Visitor))
					{
						logger?.LogError(
							GetType(),
							$"VISIT LOAD FAILED: {typeof(TValue).Name}");

						return (false, value);
					}

					return (true, value);
				}

				var asyncResult2 = await asyncBlockFormatSerializer.
					DeserializeBlockAsync<TValue>(
						context,
						blockOffset,
						blockSize,
	
						asyncContext)
						.ThrowExceptionsIfAny<(bool, TValue), Serializer>(
							logger);

				if (!asyncResult2.Item1)
				{
					logger?.LogError(
						GetType(),
						$"BLOCK DESERIALIZATION FAILED: {typeof(TValue).Name}");

					return (false, value);
				}

				return (true, asyncResult2.Item2);
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE DESERIALIZING BLOCK: {exception.Message}");

				return (false, value);
			}
		}

		public async Task<(bool, object)> DeserializeBlockAsync(
			Type valueType,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			object valueObject = default;

			var asyncBlockFormatSerializer = context.FormatSerializer
				as IAsyncBlockFormatSerializer;

			if (asyncBlockFormatSerializer == null)
			{
				if (context.Arguments.Has<IFallbackToSyncArgument>())
				{
					if (context.FormatSerializer is IBlockFormatSerializer)
					{
						bool syncResult = DeserializeBlock(
							valueType,
							blockOffset,
							blockSize,
							out valueObject);
	
						return (syncResult, valueObject);
					}

					logger?.LogError(
						GetType(),
						$"FORMAT SERIALIZER IS NOT A BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

					return (false, valueObject);
				}

				logger?.LogError(
					GetType(),
					$"FORMAT SERIALIZER IS NOT AN ASYNC BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

				return (false, valueObject);
			}

			try
			{
				if (TryGetLoadVisitor(
					valueType,
					out var loadVisitor))
				{
					Type dtoType = loadVisitor.GetDTOType(valueType);

					var asyncResult1 = await asyncBlockFormatSerializer
						.DeserializeBlockAsync(
							dtoType,
							context,
							blockOffset,
							blockSize,
	
							asyncContext)
							.ThrowExceptionsIfAny<(bool, object), Serializer>(
								logger);

					if (!asyncResult1.Item1)
					{
						logger?.LogError(
							GetType(),
							$"BLOCK DESERIALIZATION FAILED: {dtoType.Name}");

						return (false, valueObject);
					}

					if (!loadVisitor.VisitLoad(
						asyncResult1.Item2,
						valueType,
						out valueObject,
						context.Visitor))
					{
						logger?.LogError(
							GetType(),
							$"VISIT LOAD FAILED: {valueType.Name}");

						return (false, valueObject);
					}

					return (true, asyncResult1.Item2);
				}

				var asyncResult2 = await asyncBlockFormatSerializer
					.DeserializeBlockAsync(
						valueType,
						context,
						blockOffset,
						blockSize,
	
						asyncContext)
						.ThrowExceptionsIfAny<(bool, object), Serializer>(
							logger);

				if (!asyncResult2.Item1)
				{
					logger?.LogError(
						GetType(),
						$"BLOCK DESERIALIZATION FAILED: {valueType.Name}");

					return (false, valueObject);
				}

				return (true, asyncResult2.Item2);
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE DESERIALIZING BLOCK: {exception.Message}");

				return (false, default);
			}
		}

		#endregion

		#region Populate

		public async Task<bool> PopulateBlockAsync<TValue>(
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			try
			{
				var asyncBlockFormatSerializer = context.FormatSerializer
					as IAsyncBlockFormatSerializer;

				if (asyncBlockFormatSerializer == null)
				{
					if (context.Arguments.Has<IFallbackToSyncArgument>())
					{
						if (context.FormatSerializer is IBlockFormatSerializer)
						{
							return PopulateBlock<TValue>(
								value,
								blockOffset,
								blockSize);
						}
					
						logger?.LogError(
							GetType(),
							$"FORMAT SERIALIZER IS NOT A BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

						return false;
					}

					logger?.LogError(
						GetType(),
						$"FORMAT SERIALIZER IS NOT AN ASYNC BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

					return false;
				}

				if (TryGetPopulateVisitor<TValue>(
					value,
					out var populateVisitor))
				{
					Type dtoType = populateVisitor.GetDTOType<TValue>(
						value);

					var asyncResult1 = await asyncBlockFormatSerializer
						.DeserializeBlockAsync(
							dtoType,
							context,
							blockOffset,
							blockSize,
	
							asyncContext)
							.ThrowExceptionsIfAny<(bool, object), Serializer>(
								logger);

					if (!asyncResult1.Item1)
					{
						logger?.LogError(
							GetType(),
							$"BLOCK DESERIALIZATION FAILED: {dtoType.Name}");

						return false;
					}

					return TryPopulate<TValue>(
						value,
						asyncResult1.Item2,
						populateVisitor);
				}

				if (!await asyncBlockFormatSerializer.PopulateBlockAsync<TValue>(
					context,
					value,
					blockOffset,
					blockSize,

					asyncContext)
					.ThrowExceptionsIfAny<bool, Serializer>(
						logger))
				{
					logger?.LogError(
						GetType(),
						$"POPULATING BLOCK FAILED: {typeof(TValue).Name}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE POPULATING BLOCK: {exception.Message}");

				return false;
			}
		}

		public async Task<bool> PopulateBlockAsync(
			Type valueType,
			object valueObject,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			try
			{
				var asyncBlockFormatSerializer = context.FormatSerializer
					as IAsyncBlockFormatSerializer;

				if (asyncBlockFormatSerializer == null)
				{
					if (context.Arguments.Has<IFallbackToSyncArgument>())
					{
						if (context.FormatSerializer is IBlockFormatSerializer)
						{
							return PopulateBlock(
								valueType,
								valueObject,
								blockOffset,
								blockSize);
						}
					
						logger?.LogError(
							GetType(),
							$"FORMAT SERIALIZER IS NOT A BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

						return false;
					}

					logger?.LogError(
						GetType(),
						$"FORMAT SERIALIZER IS NOT AN ASYNC BLOCK FORMAT SERIALIZER: {context.FormatSerializer.GetType().Name}");

					return false;
				}

				if (TryGetPopulateVisitor(
					valueType,
					out var populateVisitor))
				{
					Type dtoType = populateVisitor.GetDTOType(valueType);

					var asyncResult1 = await asyncBlockFormatSerializer
						.DeserializeBlockAsync(
							dtoType,
							context,
							blockOffset,
							blockSize,
	
							asyncContext)
							.ThrowExceptionsIfAny<(bool, object), Serializer>(
								logger);

					if (!asyncResult1.Item1)
					{
						logger?.LogError(
							GetType(),
							$"BLOCK DESERIALIZATION FAILED: {dtoType.Name}");

						return false;
					}

					return TryPopulate(
						valueType,
						valueObject,
						asyncResult1.Item2,
						populateVisitor);
				}

				if (!await asyncBlockFormatSerializer.PopulateBlockAsync(
					valueType,
					context,
					valueObject,
					blockOffset,
					blockSize,

					asyncContext)
					.ThrowExceptionsIfAny<bool, Serializer>(
						logger))
				{
					logger?.LogError(
						GetType(),
						$"BLOCK POPULATING FAILED: {valueType.Name}");

					return false;
				}

				return true;
			}
			catch (Exception exception)
			{
				logger?.LogError(
					GetType(),
					$"CAUGHT EXCEPTION WHILE POPULATING BLOCK: {exception.Message}");

				return false;
			}
		}

		#endregion

		#endregion

		#region Ensure medium initialized / finalized

		protected void EnsureMediumInitializedForDeserialization(
			ISerializationCommandContext context)
		{
			var mediumWithReadWriteControl =
				context.SerializationMedium as IHasReadWriteControl;

			IHasIODestination mediumWithIODestination =
				context.SerializationMedium as IHasIODestination;

			IMediumWithStream mediumWithStream =
				context.SerializationMedium as IMediumWithStream;

			//Before reading we need to ensure that
			//1. There IS something to read from
			//2. Nothing is actively reading from or writing to it right now
			//(for instance, a stream)
			if (mediumWithIODestination != null
				&& (mediumWithStream == null
					|| !mediumWithStream.StreamOpen))
			{
				if (!mediumWithIODestination.IODestinationExists())
				{
					throw new InvalidOperationException(
						logger.FormatException(
							GetType(),
							$"THE DESTINATION FOR READ DOES NOT EXIST"));
				}
			}

			//If it's a streaming medium and the stream is not open, then open the write stream
			if (mediumWithStream != null
				&& !mediumWithStream.StreamOpen
				&& mediumWithReadWriteControl != null)
			{
				if (context.Arguments.Has<IReadAndWriteAccessArgument>())
				{
					if (!mediumWithReadWriteControl.SupportsSimultaneousReadAndWrite)
					{
						throw new InvalidOperationException(
							logger.FormatException(
								GetType(),
								$"THE MEDIUM {context.SerializationMedium.GetType().Name} DOES NOT SUPPORT SIMULTANEOUS READ AND WRITE"));
					}

					mediumWithReadWriteControl.InitializeReadAndWrite();
				}
				else
				{
					mediumWithReadWriteControl.InitializeRead();
				}
			}
		}

		protected void EnsureMediumFinalizedForDeserialization(
			ISerializationCommandContext context)
		{
			IHasReadWriteControl mediumWithReadWriteControl =
				context.SerializationMedium as IHasReadWriteControl;

			IMediumWithStream mediumWithStream =
				context.SerializationMedium as IMediumWithStream;

			//If it's a streaming medium and the stream is open, then close the write stream
			if (mediumWithStream != null
				&& mediumWithStream.StreamOpen
				&& mediumWithReadWriteControl != null)
			{
				if (mediumWithStream.CurrentMode == EStreamMode.READ_AND_WRITE)
				{
					mediumWithReadWriteControl.FinalizeReadAndWrite();
				}
				else if (mediumWithStream.CurrentMode == EStreamMode.READ)
				{
					mediumWithReadWriteControl.FinalizeRead();
				}
				else
				{
					throw new InvalidOperationException(
						logger.FormatException(
							GetType(),
							$"THE MEDIUM IS IN AN INVALID MODE: {mediumWithStream.CurrentMode}"));
				}
			}
		}

		protected void EnsureMediumInitializedForSerialization(
			ISerializationCommandContext context)
		{
			if (context.Arguments.Has<IReadAndWriteAccessArgument>())
			{
				EnsureMediumInitializedForReadWrite(
					context);
			}
			else if (context.Arguments.Has<IAppendArgument>())
			{
				EnsureMediumInitializedForAppend(
					context);
			}
			else
			{
				EnsureMediumInitializedForWrite(
					context);
			}
		}

		protected void EnsureMediumInitializedForReadWrite(
			ISerializationCommandContext context)
		{
			IHasReadWriteControl mediumWithReadWriteControl =
				context.SerializationMedium as IHasReadWriteControl;

			IHasIODestination mediumWithIODestination =
				context.SerializationMedium as IHasIODestination;

			IMediumWithStream mediumWithStream =
				context.SerializationMedium as IMediumWithStream;

			//Before read/writing we need to ensure that
			//1. There IS something to read from / write to
			//2. Nothing is actively reading from or writing to it right now
			//(for instance, a stream)
			if (mediumWithIODestination != null
				&& (mediumWithStream == null
					|| !mediumWithStream.StreamOpen))
			{
				//Ensure that if there is no destination, then create it
				mediumWithIODestination.EnsureIODestinationExists();
			}

			//If it's a streaming medium and the stream is not open, then open the write stream
			if (mediumWithStream != null
				&& !mediumWithStream.StreamOpen
				&& mediumWithReadWriteControl != null)
			{
				if (!mediumWithReadWriteControl.SupportsSimultaneousReadAndWrite)
				{
					throw new InvalidOperationException(
						logger.FormatException(
							GetType(),
							$"THE MEDIUM {context.SerializationMedium.GetType().Name} DOES NOT SUPPORT SIMULTANEOUS READ AND WRITE"));
				}

				mediumWithReadWriteControl.InitializeReadAndWrite();
			}
		}

		protected void EnsureMediumInitializedForAppend(
			ISerializationCommandContext context)
		{
			IHasReadWriteControl mediumWithReadWriteControl =
				context.SerializationMedium as IHasReadWriteControl;

			IHasIODestination mediumWithIODestination =
				context.SerializationMedium as IHasIODestination;

			IMediumWithStream mediumWithStream =
				context.SerializationMedium as IMediumWithStream;

			//Before appending we need to ensure that
			//1. There IS something to append to
			//2. Nothing is actively reading from or writing to it right now
			//(for instance, a stream)
			if (mediumWithIODestination != null
				&& (mediumWithStream == null
					|| !mediumWithStream.StreamOpen))
			{
				//Ensure that if there is no folder, then create it
				mediumWithIODestination.EnsureIODestinationExists();
			}

			//If it's a streaming medium and the stream is not open, then open the write stream
			if (mediumWithStream != null
				&& !mediumWithStream.StreamOpen
				&& mediumWithReadWriteControl != null)
			{
				mediumWithReadWriteControl.InitializeAppend();
			}
		}

		protected void EnsureMediumInitializedForWrite(
			ISerializationCommandContext context)
		{
			IHasReadWriteControl mediumWithReadWriteControl = context.SerializationMedium
				as IHasReadWriteControl;

			IHasIODestination mediumWithIODestination = context.SerializationMedium
				as IHasIODestination;

			IMediumWithStream mediumWithStream = context.SerializationMedium
				as IMediumWithStream;

			//Before writing we need to ensure that
			//1. Whatever may be at the destination, it is erased
			//2. There IS something to write to
			//3. Nothing is actively reading from or writing to it right now
			//(for instance, a stream)
			if (mediumWithIODestination != null)
			{
				if (mediumWithStream == null
					|| !mediumWithStream.StreamOpen)
				{
					//Ensure the file with the same path does not exist
					if (mediumWithIODestination.IODestinationExists())
					{
						mediumWithIODestination.EraseIODestination();
					}

					//Ensure that if there is no folder, then create it
					mediumWithIODestination.EnsureIODestinationExists();
				}
			}

			//If it's a streaming medium and the stream is not open, then open the write stream
			if (mediumWithStream != null
				&& !mediumWithStream.StreamOpen
				&& mediumWithReadWriteControl != null)
			{
				mediumWithReadWriteControl.InitializeWrite();
			}
		}

		protected void EnsureMediumFinalizedForSerialization(
			ISerializationCommandContext context)
		{
			if (context.Arguments.Has<IReadAndWriteAccessArgument>())
			{
				EnsureMediumFinalizedForReadWrite(
					context);
			}
			else if (context.Arguments.Has<IAppendArgument>())
			{
				EnsureMediumFinalizedForAppend(
					context);
			}
			else
			{
				EnsureMediumFinalizedForWrite(
					context);
			}
		}

		protected void EnsureMediumFinalizedForReadWrite(
			ISerializationCommandContext context)
		{
			IHasReadWriteControl mediumWithReadWriteControl =
				context.SerializationMedium as IHasReadWriteControl;

			IMediumWithStream mediumWithStream =
				context.SerializationMedium as IMediumWithStream;

			if (mediumWithStream != null
				&& mediumWithStream.StreamOpen
				&& mediumWithReadWriteControl != null)
			{
				if (mediumWithStream.CurrentMode == EStreamMode.READ_AND_WRITE)
				{
					mediumWithReadWriteControl.FinalizeReadAndWrite();
				}
				else
				{
					throw new InvalidOperationException(
						logger.FormatException(
							GetType(),
							$"THE MEDIUM IS IN AN INVALID MODE: {mediumWithStream.CurrentMode}"));
				}
			}
		}

		protected void EnsureMediumFinalizedForAppend(
			ISerializationCommandContext context)
		{
			IHasReadWriteControl mediumWithReadWriteControl =
				context.SerializationMedium as IHasReadWriteControl;

			IMediumWithStream mediumWithStream = context.SerializationMedium
				as IMediumWithStream;

			if (mediumWithStream != null
				&& mediumWithStream.StreamOpen
				&& mediumWithReadWriteControl != null)
			{
				if (mediumWithStream.CurrentMode == EStreamMode.APPEND)
				{
					mediumWithReadWriteControl.FinalizeAppend();
				}
				else
				{
					throw new InvalidOperationException(
						logger.FormatException(
							GetType(),
							$"THE MEDIUM IS IN AN INVALID MODE: {mediumWithStream.CurrentMode}"));
				}
			}
		}

		protected void EnsureMediumFinalizedForWrite(
			ISerializationCommandContext context)
		{
			IHasReadWriteControl mediumWithReadWriteControl =
				context.SerializationMedium as IHasReadWriteControl;

			IMediumWithStream mediumWithStream =
				context.SerializationMedium as IMediumWithStream;

			if (mediumWithStream != null
				&& mediumWithStream.StreamOpen
				&& mediumWithReadWriteControl != null)
			{
				if (mediumWithStream.CurrentMode == EStreamMode.WRITE)
				{
					mediumWithReadWriteControl.FinalizeWrite();
				}
				else
				{
					throw new InvalidOperationException(
						logger.FormatException(
							GetType(),
							$"THE MEDIUM IS IN AN INVALID MODE: {mediumWithStream.CurrentMode}"));
				}
			}
		}

		#endregion

		#region Try get DTO

		private bool TryGetDTO<TValue>(
			TValue value,
			out object dto)
		{
			dto = null;

			if (context.Visitor == null)
				return false;

			if (!context.Visitor.CanVisit<TValue>(
				value))
				return false;
				
			var saveVisitor = context.Visitor as ISaveVisitor;

			if (saveVisitor == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITOR IS NOT A SAVE VISITOR: {context.Visitor.GetType().Name}");

				return false;
			}

			var visitable = value as IVisitable;

			if (visitable != null)
			{
				if (!visitable.AcceptSave(
					saveVisitor,
					ref dto))
				{
					logger?.LogError(
						GetType(),
						$"VISITABLE ACCEPT SAVE FAILED: {typeof(TValue).Name}");

					return false;
				}

				return true;
			}

			if (!saveVisitor.VisitSave<TValue>(
				ref dto,
				value,
				context.Visitor))
			{
				logger?.LogError(
					GetType(),
					$"VISIT SAVE FAILED: {typeof(TValue).Name}");

				return false;
			}

			return true;
		}

		private bool TryGetDTO(
			Type valueType,
			object valueObject,
			out object dto)
		{
			dto = null;

			if (context.Visitor == null)
				return false;

			if (!context.Visitor.CanVisit(valueType))
				return false;

			var saveVisitor = context.Visitor as ISaveVisitor;

			if (saveVisitor == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITOR IS NOT A SAVE VISITOR: {context.Visitor.GetType().Name}");

				return false;
			}

			var visitable = valueObject as IVisitable;

			if (visitable != null)
			{
				if (!visitable.AcceptSave(
					saveVisitor,
					ref dto))
				{
					logger?.LogError(
						GetType(),
						$"VISITABLE ACCEPT SAVE FAILED: {valueType.Name}");

					return false;
				}

				return true;
			}

			if (!saveVisitor.VisitSave(
				ref dto,
				valueType,
				valueObject,
				context.Visitor))
			{
				logger?.LogError(
					GetType(),
					$"VISIT SAVE FAILED: {valueType.Name}");

				return false;
			}

			return true;
		}

		#endregion

		#region Try get visitor

		private bool TryGetLoadVisitor<TValue>(
			TValue value,
			out ILoadVisitor loadVisitor)
		{
			loadVisitor = null;

			if (context.Visitor == null)
				return false;

			if (!context.Visitor.CanVisit<TValue>(
				value))
				return false;
			
			loadVisitor = context.Visitor as ILoadVisitor;

			if (loadVisitor == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITOR IS NOT A LOAD VISITOR: {context.Visitor.GetType().Name}");

				return false;
			}

			return true;
		}

		private bool TryGetLoadVisitor(
			Type valueType,
			out ILoadVisitor loadVisitor)
		{
			loadVisitor = null;

			if (context.Visitor == null)
				return false;

			if (!context.Visitor.CanVisit(valueType))
				return false;

			loadVisitor = context.Visitor as ILoadVisitor;

			if (loadVisitor == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITOR IS NOT A LOAD VISITOR: {context.Visitor.GetType().Name}");

				return false;
			}

			return true;
		}

		private bool TryGetPopulateVisitor<TValue>(
			TValue value,
			out IPopulateVisitor populateVisitor)
		{
			populateVisitor = null;

			if (context.Visitor == null)
				return false;

			if (!context.Visitor.CanVisit<TValue>(
				value))
				return false;

			populateVisitor = context.Visitor as IPopulateVisitor;

			if (populateVisitor == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITOR IS NOT A POPULATE VISITOR: {context.Visitor.GetType().Name}");

				return false;
			}

			return true;
		}

		private bool TryGetPopulateVisitor(
			Type valueType,
			out IPopulateVisitor populateVisitor)
		{
			populateVisitor = null;

			if (context.Visitor == null)
				return false;

			if (!context.Visitor.CanVisit(valueType))
				return false;

			populateVisitor = context.Visitor as IPopulateVisitor;

			if (populateVisitor == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITOR IS NOT A POPULATE VISITOR: {context.Visitor.GetType().Name}");

				return false;
			}

			return true;
		}

		private bool TryPopulate<TValue>(
			TValue value,
			object dto,
			IPopulateVisitor populateVisitor)
		{
			var visitable = value as IVisitable;

			if (visitable != null)
			{
				if (!visitable.AcceptPopulate(
					populateVisitor,
					dto))
				{
					logger?.LogError(
						GetType(),
						$"VISITABLE ACCEPT SAVE FAILED: {typeof(TValue).Name}");

					return false;
				}

				return true;
			}

			if (!populateVisitor.VisitPopulate<TValue>(
				dto,
				value,
				context.Visitor))
			{
				logger?.LogError(
					GetType(),
					$"VISIT POPULATE FAILED: {typeof(TValue).Name}");

				return false;
			}

			return true;
		}

		private bool TryPopulate(
			Type valueType,
			object valueObject,
			object dto,
			IPopulateVisitor populateVisitor)
		{
			var visitable = valueObject as IVisitable;

			if (visitable != null)
			{
				if (!visitable.AcceptPopulate(
					populateVisitor,
					dto))
				{
					logger?.LogError(
						GetType(),
						$"VISITABLE ACCEPT SAVE FAILED: {valueType.Name}");

					return false;
				}

				return true;
			}

			if (!populateVisitor.VisitPopulate(
				dto,
				valueType,
				valueObject,
				context.Visitor))
			{
				logger?.LogError(
					GetType(),
					$"VISIT POPULATE FAILED: {nameof(valueType)}");

				return false;
			}

			return true;
		}

		#endregion
	}
}