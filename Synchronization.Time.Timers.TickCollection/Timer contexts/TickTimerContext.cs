using HereticalSolutions.Delegates;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Synchronization.Time.Timers.TickCollection
{
	public class TickTimerContext
		: ATimerContext<ITickTimer, ITickTimerState, TickTimerDTO>,
		  ITickTimerContext
	{
		public TickTimerContext(
			TickTimerDTO timerDTO,

			IReadOnlyRepository<ETimerState, ITickTimerState> stateRepository,

			IPublisherSingleArgGeneric<ITickTimer> onStart,
			IPublisherSingleArgGeneric<ITickTimer> onStartRepeated,
			IPublisherSingleArgGeneric<ITickTimer> onFinish,
			IPublisherSingleArgGeneric<ITickTimer> onFinishRepeated)
			: base(
				timerDTO,

				stateRepository,

				onStart,
				onStartRepeated,
				onFinish,
				onFinishRepeated)
		{
		}

		#region IRuntimeTimerContext

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

		public uint CurrentTimeElapsed
		{
			get => timerDTO.CurrentTimeElapsed;
			set => timerDTO.CurrentTimeElapsed = value;
		}

		public uint CurrentDuration
		{
			get => timerDTO.CurrentDuration;
			set => timerDTO.CurrentDuration = value;
		}

		public uint DefaultDuration => timerDTO.DefaultDuration;

		#endregion
	}
}