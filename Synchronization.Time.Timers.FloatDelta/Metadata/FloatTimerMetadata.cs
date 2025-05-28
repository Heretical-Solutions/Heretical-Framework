using System;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Synchronization.Time.Timers.FloatDelta
{
	public class FloatTimerMetadata
		: IContainsFloatTimer,
		  ICleanuppable,
		  IDisposable
	{
		public FloatTimerMetadata()
		{
			Timer = null;
		}

		#region IContainsFloatTimer

		public IFloatTimer Timer { get; set; }

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			if (Timer is ICleanuppable)
				(Timer as ICleanuppable).Cleanup();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (Timer is IDisposable)
				(Timer as IDisposable).Dispose();
		}

		#endregion
	}
}