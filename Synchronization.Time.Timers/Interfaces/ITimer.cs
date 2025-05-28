using HereticalSolutions.Delegates.NonAlloc;

namespace HereticalSolutions.Synchronization.Time.Timers
{
    public interface ITimer
    {
        string ID { get; }

        #region Timer state

        ETimerState State { get; }

        #endregion

        #region Progress

        float ProgressNormal { get; }

        #endregion

        #region Controls

        bool Accumulate { get; set; }

        bool Repeat { get; set; }

        bool FlushTimeElapsedOnRepeat { get; set; }

        bool FireRepeatCallbackOnFinish { get; set; }
        
        void Reset();

        void Start();

        void Pause();

        void Resume();

        void Abort();

        void Finish();

        #endregion

        #region Callbacks

        INonAllocSubscribable OnStart { get; }

        INonAllocSubscribable OnStartRepeated { get; }

        INonAllocSubscribable OnFinish { get; }

        INonAllocSubscribable OnFinishRepeated { get; }

        #endregion
    }
}