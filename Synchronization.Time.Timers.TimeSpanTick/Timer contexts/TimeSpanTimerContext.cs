using System;

using HereticalSolutions.Delegates;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Synchronization.Time.Timers.TimeSpanTick
{
	public class TimeSpanTimerContext
		: ATimerContext<ITimeSpanTimer, ITimeSpanTimerState, TimeSpanTimerDTO>,
		  ITimeSpanTimerContext
	{
		public TimeSpanTimerContext(
			TimeSpanTimerDTO timerDTO,

			IReadOnlyRepository<ETimerState, ITimeSpanTimerState> stateRepository,

			IPublisherSingleArgGeneric<ITimeSpanTimer> onStart,
			IPublisherSingleArgGeneric<ITimeSpanTimer> onStartRepeated,
			IPublisherSingleArgGeneric<ITimeSpanTimer> onFinish,
			IPublisherSingleArgGeneric<ITimeSpanTimer> onFinishRepeated)
			: base(
				timerDTO,

				stateRepository,

				onStart,
				onStartRepeated,
				onFinish,
				onFinishRepeated)
		{
		}

		#region IPersistentTimerContext

		#region ITimerContext

		public override ETimerState CurrentState
		{
			get => timerDTO.State;
			set => timerDTO.State = value;
		}

		#region Controls

		public override bool Accumulate
		{
			get => timerDTO.Accumulate;
			set => timerDTO.Accumulate = value;
		}

		public override bool Repeat
		{
			get => timerDTO.Repeat;
			set => timerDTO.Repeat = value;
		}

		public override bool FlushTimeElapsedOnRepeat
		{
			get => timerDTO.FlushTimeElapsedOnRepeat;
			set => timerDTO.FlushTimeElapsedOnRepeat = value;
		}

		public override bool FireRepeatCallbackOnFinish
		{
			get => timerDTO.FireRepeatCallbackOnFinish;
			set => timerDTO.FireRepeatCallbackOnFinish = value;
		}

		#endregion

		#endregion

		public DateTime StartTime
		{
			get => timerDTO.StartTime;
			set => timerDTO.StartTime = value;
		}

		public DateTime EstimatedFinishTime
		{
			get => timerDTO.EstimatedFinishTime;
			set => timerDTO.EstimatedFinishTime = value;
		}

		public TimeSpan SavedProgress
		{
			get => timerDTO.SavedProgress;
			set => timerDTO.SavedProgress = value;
		}

		public TimeSpan CurrentDurationSpan
		{
			get => timerDTO.CurrentDurationSpan;
			set => timerDTO.CurrentDurationSpan = value;
		}

		public TimeSpan DefaultDurationSpan
		{
			get => timerDTO.DefaultDurationSpan;
			set => timerDTO.DefaultDurationSpan = value;
		}

		#endregion
	}
}