using System;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEditor;

namespace HereticalSolutions.Persistence.Unity.Editor
{
	[SerializationMedium]
	public class EditorPrefsMedium
		: ISerializationMedium,
		  IHasIODestination
	{
		private static readonly Type[] allowedValueTypes = new Type[]
		{
			typeof(string)
		};

		private readonly string keyPrefs;

		private readonly ILogger logger;

		public EditorPrefsMedium(
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

			if (EditorPrefs.HasKey(keyPrefs))
			{
				value = EditorPrefs.GetString(keyPrefs).CastFromTo<string, TValue>();

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

			if (EditorPrefs.HasKey(keyPrefs))
			{
				value = EditorPrefs.GetString(keyPrefs);

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

			EditorPrefs.SetString(
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

			EditorPrefs.SetString(
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

		#region IHasIODestination

		public void EnsureIODestinationExists()
		{
			//Do nothing
		}

		public bool IODestinationExists()
		{
			return EditorPrefs.HasKey(keyPrefs);
		}

		public void CreateIODestination()
		{
			//Do nothing
		}

		public void EraseIODestination()
		{
			EditorPrefs.DeleteKey(keyPrefs);
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