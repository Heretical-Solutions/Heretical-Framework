using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public abstract class AMedium
		: ISerializationMedium
	{
		protected readonly ILogger logger;

		public AMedium(
			ILogger logger)
		{
			this.logger = logger;
		}

		#region ISerializationMedium

		#region Read

		public abstract bool Read<TValue>(
			out TValue value);

		public abstract bool Read(
			Type valueType,
			out object value);

		#endregion

		#region Write

		public abstract bool Write<TValue>(
			TValue value);

		public abstract bool Write(
			Type valueType,
			object value);

		#endregion

		#region Append

		public abstract bool Append<TValue>(
			TValue value);

		public abstract bool Append(
			Type valueType,
			object value);

		#endregion

		#endregion

		protected void AssertValueType(
			Type valueType,
			Type expectedType)
		{
			if (valueType != expectedType)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"INVALID VALUE TYPE: {valueType.Name}"));
		}
	}
}