using System;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Synchronization.Time.Timers.TimeSpanTick
{
	public class TimeSpanTimerMetadata
		: IContainsTimeSpanTimer,
		  ICleanuppable,
		  IDisposable
	{
		public TimeSpanTimerMetadata()
		{
			Timer = null;
		}

		#region IContainsTimeSpanTimer

		public ITimeSpanTimer Timer { get; set; }

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