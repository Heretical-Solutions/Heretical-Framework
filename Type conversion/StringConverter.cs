using System;

using HereticalSolutions.Repositories;

using HereticalSolutions.Persistence;

using HereticalSolutions.Logging;

namespace HereticalSolutions.TypeConversion
{
	public class StringConverter
		: ATypeConverter<string>
	{
		private readonly ISerializer stringSerializer;

		private readonly CachedStringMedium cachedStringMedium;

		public StringConverter(
			ISerializer stringSerializer,
			CachedStringMedium cachedStringMedium,

			IReadOnlyRepository<Type, Delegate> convertFromTargetTypeDelegateRepository,
			IReadOnlyRepository<Type, Delegate> convertToTargetTypeDelegateRepository,
			
			ILogger logger)
			: base(
				convertFromTargetTypeDelegateRepository,
				convertToTargetTypeDelegateRepository,
				logger)
		{
			this.stringSerializer = stringSerializer;
			
			this.cachedStringMedium = cachedStringMedium;
		}

		protected override bool FallbackConvertFromTargetType<TValue>(
			string @string,
			out TValue value)
		{
			if (typeof(TValue) == typeof(string))
			{
				value = @string.CastFromTo<string, TValue>();

				return true;
			}

			if (typeof(TValue).IsPrimitive)
			{
				value = (TValue)Convert.ChangeType(
					@string,
					typeof(TValue));

				return true;
			}

			cachedStringMedium.Value = @string;

			if (stringSerializer.Deserialize<TValue>(
				out value))
			{
				return true;
			}

			logger?.LogError(
				GetType(),
				$"UNSUPPORTED TYPE: {typeof(TValue).Name}");

			value = default;

			return false;
		}

		protected override bool FallbackConvertFromTargetType(
			Type valueType,
			string @string,
			out object value)
		{
			if (valueType == typeof(string))
			{
				value = @string.CastFromTo<string, object>();

				return true;
			}

			if (valueType.IsPrimitive)
			{
				value = Convert.ChangeType(
					@string,
					valueType);

				return true;
			}

			cachedStringMedium.Value = @string;

			if (stringSerializer.Deserialize(
				valueType,
				out value ))
			{
				return true;
			}

			logger?.LogError(
				GetType(),
				$"UNSUPPORTED TYPE: {valueType.Name}");

			value = default;

			return false;
		}

		protected override bool FallbackConvertToTargetType<TValue>(
			TValue value,
			out string @string)
		{
			if (typeof(TValue) == typeof(string))
			{
				@string = value.CastFromTo<TValue, string>();

				return true;
			}

			if (typeof(TValue).IsPrimitive)
			{
				@string = value.ToString();

				return true;
			}

			if (stringSerializer.Serialize<TValue>(
				value))
			{
				@string = cachedStringMedium.Value;

				return true;
			}

			logger?.LogError(
				GetType(),
				$"UNSUPPORTED TYPE: {typeof(TValue).Name}");

			@string = default;

			return false;
		}

		protected override bool FallbackConvertToTargetType(
			Type valueType,
			object valueObject,
			out string @string)
		{
			if (valueType == typeof(string))
			{
				@string = valueObject.CastFromTo<object, string>();

				return true;
			}

			if (valueType.IsPrimitive)
			{
				@string = valueObject.ToString();

				return true;
			}

			if (stringSerializer.Serialize(
				valueType,
				valueObject))
			{
				@string = cachedStringMedium.Value;

				return true;
			}

			logger?.LogError(
				GetType(),
				$"UNSUPPORTED TYPE: {valueType.Name}");

			@string = default;

			return false;
		}
	}
}