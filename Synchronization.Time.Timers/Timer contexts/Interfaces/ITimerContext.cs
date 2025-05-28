using HereticalSolutions.Delegates;

namespace HereticalSolutions.Synchronization.Time.Timers
{
	public interface ITimerContext<TTimer, TTimerState, TDTO>
		where TTimer : ITimer
	{
		TTimer Timer { get; }

		TDTO DTO { get; set; }

		#region State

		ETimerState CurrentState { get; }

		TTimerState CurrentStateInstance { get; }

		void SetState(
			ETimerState state);

		#endregion

		#region Controls

		bool Accumulate { get; set; }

		bool Repeat { get; set; }

		bool FlushTimeElapsedOnRepeat { get; set; }

		bool FireRepeatCallbackOnFinish { get; set; }

		#endregion

		#region Publishers

		IPublisherSingleArgGeneric<TTimer> OnStart { get; }

		IPublisherSingleArgGeneric<TTimer> OnStartRepeated { get; }

		IPublisherSingleArgGeneric<TTimer> OnFinish { get; }

		IPublisherSingleArgGeneric<TTimer> OnFinishRepeated { get; }

		#endregion
	}
}