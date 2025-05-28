using System;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.ObjectPools;

using HereticalSolutions.Synchronization.Time.Timers;
using HereticalSolutions.Synchronization.Time.Timers.FloatDelta;
using HereticalSolutions.Synchronization.Time.TimeUpdaters;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Synchronization.Time.TimerManagers
{
	public class TimerManager
		: ITimerManager,
		  ICleanuppable,
		  IDisposable
	{
		private readonly string timerManagerID;

		private readonly ITimeUpdater timeUpdater;

		private readonly IPool<IFloatTimer> timerPool;

		private readonly IPool<AllocatedTimerContext> contextPool;

		private readonly ILogger logger;
		
		public TimerManager(
			string timerManagerID,
			ITimeUpdater timeUpdater,
			IPool<IFloatTimer> timerPool,
			IPool<AllocatedTimerContext> contextPool,
			ILogger logger)
		{
			this.timerManagerID = timerManagerID;

			this.timeUpdater = timeUpdater;

			this.timerPool = timerPool;

			this.contextPool = contextPool;

			this.logger = logger;
		}

		#region ITimerManager

		public string ID { get => timerManagerID;}

		public ITimeUpdater TimeUpdater => timeUpdater;

		public bool TryAllocateTimer(
			out AllocatedTimerContext context)
		{
			var timer = timerPool.Pop();

			timer.OnStart.UnsubscribeAll();
			timer.OnFinish.UnsubscribeAll();
			timer.OnStartRepeated.UnsubscribeAll();
			timer.OnFinishRepeated.UnsubscribeAll();

			timer.Accumulate = false;
			timer.Repeat = false;
			timer.FlushTimeElapsedOnRepeat = false;
			timer.FireRepeatCallbackOnFinish = true;

			timer.Context.SetState(
				ETimerState.INACTIVE);

			context = contextPool.Pop();

			context.Initialize(
				timer,
				this);

			return true;
		}

		public bool TryFreeTimer(
			AllocatedTimerContext context)
		{
			if (context == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"CONTEXT IS NULL"));
			}

			var timer = context.Timer;

			if (timer == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"TIMER IS NULL"));
			}

			timer.OnStart.UnsubscribeAll();
			timer.OnFinish.UnsubscribeAll();
			timer.OnStartRepeated.UnsubscribeAll();
			timer.OnFinishRepeated.UnsubscribeAll();

			timer.Accumulate = false;
			timer.Repeat = false;
			timer.FlushTimeElapsedOnRepeat = false;
			timer.FireRepeatCallbackOnFinish = true;

			timer.Context.SetState(
				ETimerState.INACTIVE);

			timerPool.Push(
				timer);


			context.Cleanup();

			contextPool.Push(
				context);

			return true;
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			if (timeUpdater is ICleanuppable)
				(timeUpdater as ICleanuppable).Cleanup();

			if (timerPool is ICleanuppable)
				(timerPool as ICleanuppable).Cleanup();

			if (contextPool is ICleanuppable)
				(contextPool as ICleanuppable).Cleanup();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (timeUpdater is IDisposable)
				(timeUpdater as IDisposable).Dispose();

			if (timerPool is IDisposable)
				(timerPool as IDisposable).Dispose();

			if (contextPool is IDisposable)
				(contextPool as IDisposable).Dispose();
		}

		#endregion
	}
}