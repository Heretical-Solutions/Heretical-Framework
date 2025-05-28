using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	[SerializationMedium]
	public class CachedStringMedium
		: ISerializationMedium,
		  IMediumWithTypeFilter
	{
		private readonly ILogger logger;

		public string Value { get; set; }

		public CachedStringMedium(
			ILogger logger)
		{
			this.logger = logger;

			Value = string.Empty;
		}

		#region ISerializationMedium

		#region Read

		public bool Read<TValue>(
			out TValue value)
		{
			AssertMediumIsValid(
			   typeof(TValue));

			value = Value.CastFromTo<string, TValue>();

			return true;
		}

		public bool Read(
			Type valueType,
			out object value)
		{
			AssertMediumIsValid(
			   valueType);

			value = Value.CastFromTo<string, object>();

			return true;
		}

		#endregion

		#region Write

		public bool Write<TValue>(
			TValue value)
		{
			AssertMediumIsValid(
			   typeof(TValue));

			Value = value.CastFromTo<TValue, string>();

			return true;
		}

		public bool Write(
			Type valueType,
			object value)
		{
			AssertMediumIsValid(
			   valueType);

			Value = value.CastFromTo<object, string>();

			return true;
		}

		#endregion

		#region Append

		public bool Append<TValue>(
			TValue value)
		{
			AssertMediumIsValid(
				typeof(TValue));

			var result = Value;

			result += value.CastFromTo<TValue, string>();

			Value = result;

			return true;
		}

		public bool Append(
			Type valueType,
			object value)
		{
			AssertMediumIsValid(
				valueType);

			var result = Value;

			result += value.CastFromTo<object, string>();

			Value = result;

			return true;
		}

		#endregion

		#endregion

		#region IMediumWithTypeFilter

		public bool AllowsType<TValue>()
		{
			return typeof(TValue) == typeof(string);
		}

		public bool AllowsType(
			Type valueType)
		{
			return valueType == typeof(string);
		}

		#endregion

		private void AssertMediumIsValid(
			Type valueType)
		{
			if (valueType != typeof(string))
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"INVALID VALUE TYPE: {valueType.Name}"));
		}
	}
}