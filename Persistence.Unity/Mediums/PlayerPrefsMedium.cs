using System;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

namespace HereticalSolutions.Persistence.Unity
{
	[SerializationMedium]
	public class PlayerPrefsMedium
		: ISerializationMedium,
		  IMediumWithTypeFilter,
		  IHasIODestination
	{
		private static readonly Type[] allowedValueTypes = new Type[]
		{
			typeof(string)
		};

		private readonly string keyPrefs;

		private readonly ILogger logger;

		public PlayerPrefsMedium(
			string keyPrefs,
			ILogger logger)
		{
			this.keyPrefs = keyPrefs;

			this.logger = logger;
		}

		#region ISerializationMedium

		public Type[] AllowedValueTypes { get => allowedValueTypes; }

		#region Read

		public bool Read<TValue>(
			out TValue value)
		{
			AssertMediumIsValid(
				typeof(TValue));

			if (PlayerPrefs.HasKey(keyPrefs))
			{
				value = PlayerPrefs.GetString(keyPrefs).CastFromTo<string, TValue>();

				return true;
			}

			value = default;

			return false;
		}

		public bool Read(
			Type valueType,
			out object value)
		{
			AssertMediumIsValid(
				valueType);

			if (PlayerPrefs.HasKey(keyPrefs))
			{
				value = PlayerPrefs.GetString(keyPrefs);

				return true;
			}

			value = default;

			return false;
		}

		#endregion

		#region Write

		public bool Write<TValue>(
			TValue value)
		{
			AssertMediumIsValid(
				typeof(TValue));

			PlayerPrefs.SetString(
				keyPrefs,
				value.CastFromTo<TValue, string>());

			return true;
		}

		public bool Write(
			Type valueType,
			object value)
		{
			AssertMediumIsValid(
				valueType);

			PlayerPrefs.SetString(
				keyPrefs,
				value.CastFromTo<object, string>());

			return true;
		}

		#endregion

		#region Append

		public bool Append<TValue>(
			TValue value)
		{
			throw new NotSupportedException();
		}

		public bool Append(
			Type valueType,
			object value)
		{
			throw new NotSupportedException();
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

		#region IHasIODestination

		public void EnsureIODestinationExists()
		{
			//Do nothing
		}

		public bool IODestinationExists()
		{
			return PlayerPrefs.HasKey(keyPrefs);
		}

		public void CreateIODestination()
		{
			//Do nothing
		}

		public void EraseIODestination()
		{
			PlayerPrefs.DeleteKey(keyPrefs);
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