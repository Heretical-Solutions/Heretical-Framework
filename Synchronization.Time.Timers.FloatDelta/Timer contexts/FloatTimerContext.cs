using HereticalSolutions.Delegates;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Synchronization.Time.Timers.FloatDelta
{
	public class FloatTimerContext
		: ATimerContext<IFloatTimer, IFloatTimerState, FloatTimerDTO>,
		  IFloatTimerContext
	{
		public FloatTimerContext(
			FloatTimerDTO timerDTO,

			IReadOnlyRepository<ETimerState, IFloatTimerState> stateRepository,

			IPublisherSingleArgGeneric<IFloatTimer> onStart,
			IPublisherSingleArgGeneric<IFloatTimer> onStartRepeated,
			IPublisherSingleArgGeneric<IFloatTimer> onFinish,
			IPublisherSingleArgGeneric<IFloatTimer> onFinishRepeated)
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

		public float CurrentTimeElapsed
		{
			get => timerDTO.CurrentTimeElapsed;
			set	=> timerDTO.CurrentTimeElapsed = value;
		}

		public float CurrentDuration
		{
			get => timerDTO.CurrentDuration;
			set => timerDTO.CurrentDuration = value;
		}

		public float DefaultDuration => timerDTO.DefaultDuration;

		#endregion
	}
}