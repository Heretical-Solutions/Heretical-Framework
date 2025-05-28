using System;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Synchronization.Time.Timers.TickCollection
{
	public class TickTimerMetadata
		: IContainsTickTimer,
		  ICleanuppable,
		  IDisposable
	{
		public TickTimerMetadata()
		{
			Timer = null;
		}

		#region IContainsTickTimer

		public ITickTimer Timer { get; set; }

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