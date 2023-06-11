using System;

using HereticalSolutions.Delegates;
using HereticalSolutions.Persistence;
using HereticalSolutions.Repositories;

namespace HereticalSolutions.Time.Timers
{
    public class RuntimeTimer
        : ITimer,
          IRuntimeTimer,
          IRuntimeTimerContext,
          ITimerWithState,
          ITickable,
          IVisitable
    {
        private ITimerStrategy<IRuntimeTimerContext, float> currentStrategy;

        private readonly IReadOnlyRepository<ETimerState, ITimerStrategy<IRuntimeTimerContext, float>> strategyRepository;

        public RuntimeTimer(
            string id,
            float defaultDuration,
            
            IPublisherSingleArgGeneric<IRuntimeTimer> onStartAsPublisher,
            INonAllocSubscribableSingleArgGeneric<IRuntimeTimer> onStartAsSubscribable,
            
            IPublisherSingleArgGeneric<IRuntimeTimer> onFinishAsPublisher,
            INonAllocSubscribableSingleArgGeneric<IRuntimeTimer> onFinishAsSubscribable,
            
            IReadOnlyRepository<ETimerState, ITimerStrategy<IRuntimeTimerContext, float>> strategyRepository)
        {
            ID = id;

            CurrentTimeElapsed = 0f;

            CurrentDuration = DefaultDuration = defaultDuration;
            

            OnStartAsPublisher = onStartAsPublisher;
            
            OnStart = onStartAsSubscribable;
            
            
            OnFinishAsPublisher = onFinishAsPublisher;

            OnFinish = onFinishAsSubscribable;
            
            
            this.strategyRepository = strategyRepository;
            
            SetState(ETimerState.INACTIVE);
        }

        #region IRuntimeTimerContext
        
        #region Variables
        
        public float CurrentTimeElapsed { get; set; }

        #endregion
        
        #region Duration
        
        public float CurrentDuration { get; set; }

        public float DefaultDuration { get; private set; }

        #endregion

        #region Publishers
        
        public IPublisherSingleArgGeneric<IRuntimeTimer> OnStartAsPublisher { get; private set; }
        
        public IPublisherSingleArgGeneric<IRuntimeTimer> OnFinishAsPublisher { get; private set; }

        #endregion
        
        #endregion
        
        #region ITimer
        
        public string ID { get; private set; }
        
        #region Timer state
        
        public ETimerState State { get; private set; }

        #endregion
        
        #region Progress
        
        public float Progress
        {
            get => currentStrategy.GetProgress(this);
        }
        
        #endregion

        #region Controls
        
        public bool Accumulate { get; set; }

        public bool Repeat { get; set; }
        
        public void Reset()
        {
            currentStrategy.Reset(this);
        }

        public void Start()
        {
            currentStrategy.Start(this);
        }

        public void Pause()
        {
            currentStrategy.Pause(this);
        }

        public void Resume()
        {
            currentStrategy.Resume(this);
        }

        public void Abort()
        {
            currentStrategy.Abort(this);
        }

        public void Finish()
        {
            currentStrategy.Finish(this);
        }
        
        #endregion
        
        #endregion

        #region IRuntimeTimer
        
        #region Countdown and Time elapsed
        
        public float TimeElapsed
        {
            get => currentStrategy.GetTimeElapsed(this);
        }
        
        public float Countdown
        {
            get => currentStrategy.GetCountdown(this);
        }
        
        #endregion

        #region Controls
        
        public void Reset(float duration)
        {
            Reset();

            CurrentDuration = duration;
        }

        public void Start(float duration)
        {
            CurrentDuration = duration;
            
            Start();
        }
        
        #endregion
        
        #region Callbacks

        public INonAllocSubscribableSingleArgGeneric<IRuntimeTimer> OnStart { get; private set; }
        
        public INonAllocSubscribableSingleArgGeneric<IRuntimeTimer> OnFinish { get; private set; }

        #endregion
        
        #endregion

        #region ITickable

        public void Tick(float delta)
        {
            currentStrategy.Tick(this, delta);
        }

        #endregion

        #region ITimerWithState

        public void SetState(ETimerState state)
        {
            State = state;
            
            //Strategies should exist for all states in enum therefore Get
            currentStrategy = strategyRepository.Get(state);
        }

        #endregion

        #region IVisitable

        public Type DTOType
        {
            get => typeof(RuntimeTimerDTO);
        }
        
        public bool Accept<TDTO>(ISaveVisitor visitor, out TDTO DTO)
        {
            if (!(typeof(TDTO).Equals(typeof(RuntimeTimerDTO))))
                throw new Exception($"[RuntimeTimer] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(RuntimeTimerDTO).ToString()}\" RECEIVED: \"{typeof(TDTO).ToString()}\"");
            
            var result = visitor.Save<IRuntimeTimer, RuntimeTimerDTO>(this, out RuntimeTimerDTO runtimeTimerDTO);

            //DIRTY HACKS DO NOT REPEAT
            var dtoObject = (object)runtimeTimerDTO;
            
            DTO = (TDTO)dtoObject;

            return result;
        }
        
        public bool Accept(ISaveVisitor visitor, out object DTO)
        {
            var result = visitor.Save<IRuntimeTimer, RuntimeTimerDTO>(this, out RuntimeTimerDTO runtimeTimerDTO);

            DTO = runtimeTimerDTO;

            return result;
        }

        public bool Accept<TDTO>(ILoadVisitor visitor, TDTO DTO)
        {
            if (!(typeof(TDTO).Equals(typeof(RuntimeTimerDTO))))
                throw new Exception($"[RuntimeTimer] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(RuntimeTimerDTO).ToString()}\" RECEIVED: \"{typeof(TDTO).ToString()}\"");
            
            //DIRTY HACKS DO NOT REPEAT
            var dtoObject = (object)DTO;
            
            return visitor.Load<IRuntimeTimer, RuntimeTimerDTO>((RuntimeTimerDTO)dtoObject, this);
        }
        
        public bool Accept(ILoadVisitor visitor, object DTO)
        {
            return visitor.Load<IRuntimeTimer, RuntimeTimerDTO>((RuntimeTimerDTO)DTO, this);
        }
        
        #endregion
    }
}