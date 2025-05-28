using HereticalSolutions.Delegates;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Synchronization.Time.Timers
{
	public abstract class ATimerContext<TTimer, TTimerState, TDTO>
		: ITimerContext<TTimer, TTimerState, TDTO>,
		  ITimerDependencyProvideable<TTimer>
		  where TTimer : ITimer
	{
		protected readonly
			IReadOnlyRepository<ETimerState, TTimerState>
				stateRepository;

		protected readonly IPublisherSingleArgGeneric<TTimer> onStart;

		protected readonly IPublisherSingleArgGeneric<TTimer> onStartRepeated;

		protected readonly IPublisherSingleArgGeneric<TTimer> onFinish;

		protected readonly IPublisherSingleArgGeneric<TTimer> onFinishRepeated;

		protected TDTO timerDTO;

		protected TTimerState currentStateInstance;

		public ATimerContext(
			TDTO timerDTO,

			IReadOnlyRepository<ETimerState, TTimerState> stateRepository,

			IPublisherSingleArgGeneric<TTimer> onStart,
			IPublisherSingleArgGeneric<TTimer> onStartRepeated,
			IPublisherSingleArgGeneric<TTimer> onFinish,
			IPublisherSingleArgGeneric<TTimer> onFinishRepeated)
		{
			this.timerDTO = timerDTO;

			this.stateRepository = stateRepository;

			this.onStart = onStart;
			this.onStartRepeated = onStartRepeated;
			this.onFinish = onFinish;
			this.onFinishRepeated = onFinishRepeated;

			SetState(
				ETimerState.INACTIVE);
		}

		#region ITimerContext

		public TTimer Timer { get; set; }

		public TDTO DTO
		{
			get => timerDTO;
			set => timerDTO = value;
		}

		#region State

		public abstract ETimerState CurrentState { get; set; }

		public TTimerState CurrentStateInstance => currentStateInstance;

		public void SetState(
			ETimerState state)
		{
			CurrentState = state;

			currentStateInstance = stateRepository.Get(state);
		}

		#endregion

		#region Controls

		public abstract bool Accumulate { get; set; }

		public abstract bool Repeat { get; set; }

		public abstract bool FlushTimeElapsedOnRepeat { get; set; }

		public abstract bool FireRepeatCallbackOnFinish { get; set; }

		#endregion

		#region Publishers

		public IPublisherSingleArgGeneric<TTimer> OnStart => onStart;

		public IPublisherSingleArgGeneric<TTimer> OnStartRepeated => onStartRepeated;

		public IPublisherSingleArgGeneric<TTimer> OnFinish => onFinish;

		public IPublisherSingleArgGeneric<TTimer> OnFinishRepeated => onFinishRepeated;

		#endregion

		#endregion
	}
}