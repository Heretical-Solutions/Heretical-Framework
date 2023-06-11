namespace HereticalSolutions.Time
{
    public interface ITimer
    {
        string ID { get; }

        #region Timer state

        ETimerState State { get; }

        #endregion

        #region Progress

        float Progress { get; }

        #endregion

        #region Controls

        /// <summary>
        /// If set to true, instead of checking the elapsed time against the duration the timer works only as an accumulative counter of time
        /// </summary>
        bool Accumulate { get; set; }

        /// <summary>
        /// If set to true, instead of translating into finished state on timeout the timer starts another cycle and fires OnFinish/OnStart callbacks
        /// </summary>
        bool Repeat { get; set; }

        /// <summary>
        /// Resets the timer. State becomes INACTIVE, time elapsed becomes zero, countdown is reset to current duration, current duration is set to default duration, start and estimated finish times are reset to default
        /// </summary>
        void Reset();

        /// <summary>
        /// Starts the timer. State becomes STARTED, time elapsed becomes zero, countdown is reset to current duration, start and estimated finish times are calculated
        /// </summary>
        void Start();

        /// <summary>
        /// Pauses the timer. State becomes PAUSED, start and estimated finish times are reset to default
        /// </summary>
        void Pause();

        /// <summary>
        /// Resumes the timer. State becomes STARTED, start and estimated finish times are calculated
        /// </summary>
        void Resume();
        
        /// <summary>
        /// Aborts the timer. State becomes INACTIVE, time elapsed becomes zero, countdown is reset to current duration, start and estimated finish times are reset to default
        /// </summary>
        void Abort();

        /// <summary>
        /// Finishes the timer operation prematurely. State becomes FINISHED, time elapsed becomes zero, countdown is reset to current duration, start and estimated finish times are reset to default
        /// </summary>
        void Finish();

        #endregion
    }
}