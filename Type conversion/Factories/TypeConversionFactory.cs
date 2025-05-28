using System;
using System.Text;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Persistence.Builders;
using HereticalSolutions.Persistence;

#if JSON_SUPPORT
using HereticalSolutions.Persistence.JSON.Builders;
using HereticalSolutions.Persistence.JSON.Factories;
#endif

using HereticalSolutions.Logging;

namespace HereticalSolutions.TypeConversion.Factories
{
	public class TypeConversionFactory
	{
		private readonly RepositoryFactory repositoryFactory;

		private readonly ILoggerResolver loggerResolver;

		public TypeConversionFactory(
			RepositoryFactory repositoryFactory,
			ILoggerResolver loggerResolver)
		{
			this.repositoryFactory = repositoryFactory;

			this.loggerResolver = loggerResolver;
		}

		public ByteArrayConverter BuildByteArrayConverter(
			TypeDelegatePair[] convertFromBytesDelegates,
			TypeDelegatePair[] convertToBytesDelegates)
		{
			ILogger logger = loggerResolver?.GetLogger<ByteArrayConverter>();

			IRepository<Type, Delegate> convertFromBytesDelegateRepository =
				repositoryFactory.BuildDictionaryRepository<Type, Delegate>();

			IRepository<Type, Delegate> convertToBytesDelegateRepository =
				repositoryFactory.BuildDictionaryRepository<Type, Delegate>();

			AddByteArrayConverterDefaultDelegates(
				convertFromBytesDelegateRepository,
				convertToBytesDelegateRepository);

			if (convertFromBytesDelegates != null)
				foreach (TypeDelegatePair pair in convertFromBytesDelegates)
				{
					convertFromBytesDelegateRepository.Add(
						pair.Type,
						pair.Delegate);
				}

			if (convertToBytesDelegates != null)
				foreach (TypeDelegatePair pair in convertToBytesDelegates)
				{
					convertToBytesDelegateRepository.Add(
						pair.Type,
						pair.Delegate);
				}

			return new ByteArrayConverter(
				convertFromBytesDelegateRepository,
				convertToBytesDelegateRepository,
				logger);
		}

		private void AddByteArrayConverterDefaultDelegates(
			IRepository<Type, Delegate> convertFromBytesDelegateRepository,
			IRepository<Type, Delegate> convertToBytesDelegateRepository)
		{
			convertFromBytesDelegateRepository.Add(
				typeof(string),
				(Func<byte[], string>)Encoding.UTF8.GetString);

			convertToBytesDelegateRepository.Add(
				typeof(string),
				(Func<string, byte[]>)Encoding.UTF8.GetBytes);
		}

		public StringConverter BuildStringConverter(
			SerializerBuilder serializerBuilder,

#if JSON_SUPPORT
			JSONPersistenceFactory jsonPersistenceFactory,
#endif

			TypeDelegatePair[] convertFromStringDelegates,
			TypeDelegatePair[] convertToStringDelegates)
		{
			ILogger logger = loggerResolver?.GetLogger<StringConverter>();

			IRepository<Type, Delegate> convertFromStringDelegateRepository =
				repositoryFactory.BuildDictionaryRepository<Type, Delegate>();

			IRepository<Type, Delegate> convertToStringDelegateRepository =
				repositoryFactory.BuildDictionaryRepository<Type, Delegate>();

			AddStringConverterDefaultDelegates(
				convertFromStringDelegateRepository,
				convertToStringDelegateRepository);

			if (convertFromStringDelegates != null)
				foreach (TypeDelegatePair pair in convertFromStringDelegates)
				{
					convertFromStringDelegateRepository.Add(
						pair.Type,
						pair.Delegate);
				}

			if (convertToStringDelegates != null)
				foreach (TypeDelegatePair pair in convertToStringDelegates)
				{
					convertToStringDelegateRepository.Add(
						pair.Type,
						pair.Delegate);
				}

			var stringSerializer = serializerBuilder
				.NewSerializer()
#if JSON_SUPPORT
				.ToJSON(
					jsonPersistenceFactory)
#else
				.ToObject()
#endif
				.AsCachedString()
				.BuildSerializer();

			var cachedStringMedium = stringSerializer
				.Context
				.SerializationMedium
				as CachedStringMedium;

			return new StringConverter(
				stringSerializer,
				cachedStringMedium,

				convertFromStringDelegateRepository,
				convertToStringDelegateRepository,

				logger);
		}

		private void AddStringConverterDefaultDelegates(
			IRepository<Type, Delegate> convertFromStringDelegateRepository,
			IRepository<Type, Delegate> convertToStringDelegateRepository)
		{
			convertFromStringDelegateRepository.Add(
				typeof(byte[]),
				(Func<string, byte[]>)Encoding.UTF8.GetBytes);

			convertToStringDelegateRepository.Add(
				typeof(byte[]),
				(Func<byte[], string>)Encoding.UTF8.GetString);
		}
	}
}