using System;

using HereticalSolutions.Persistence;

using HereticalSolutions.Synchronization.Time.Timers.TimeSpanTick.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Synchronization.Time.Timers.TimeSpanTick
{
    [Visitor(typeof(TimeSpanTimer), typeof(TimeSpanTimerDTO))]
    public class TimeSpanTimerVisitor
        : ISaveVisitor,
          ILoadVisitor,
          IPopulateVisitor
    {
        private readonly TimeSpanTimerFactory timerFactory;

        private readonly ILogger logger;

        public TimeSpanTimerVisitor(
            TimeSpanTimerFactory timerFactory,
            ILogger logger)
        {
            this.timerFactory = timerFactory;

            this.logger = logger;
        }

        #region IVisitor

        public bool CanVisit<TVisitable>(
            TVisitable instance)
        {
            return typeof(TVisitable) == typeof(TimeSpanTimer);
        }

        public bool CanVisit(
            Type visitableType,
            object instance)
        {
            return visitableType == typeof(TimeSpanTimer);
        }

        public Type GetDTOType<TVisitable>(
            TVisitable instance)
        {
            if (typeof(TVisitable) != typeof(TimeSpanTimer))
                return null;

            return typeof(TimeSpanTimerDTO);
        }

        public Type GetDTOType(
            Type visitableType,
            object instance)
        {
            if (visitableType != typeof(TimeSpanTimer))
                return null;

            return typeof(TimeSpanTimerDTO);
        }

        #endregion

        #region ISaveVisitor

        public bool VisitSave<TVisitable>(
            ref object dto,
            TVisitable visitable,
            IVisitor rootVisitor)
        {
            TimeSpanTimer timer = visitable as TimeSpanTimer;

            if (timer == null)
            {
                logger?.LogError(
                    GetType(),
                    $"VISITABLE IS NOT OF TYPE: {nameof(TimeSpanTimer)}");

                dto = null;

                return false;
            }

            dto = timer.Context.DTO;

            return true;
        }

        public bool VisitSave(
            ref object dto,
            Type visitableType,
            object visitableObject,
            IVisitor rootVisitor)
        {
            TimeSpanTimer timer = visitableObject as TimeSpanTimer;

            if (timer == null)
            {
                logger?.LogError(
                    GetType(),
                    $"VISITABLE IS NOT OF TYPE: {nameof(TimeSpanTimer)}");

                dto = null;

                return false;
            }

            dto = timer.Context.DTO;

            return true;
        }

        #endregion

        #region ILoadVisitor

        public bool VisitLoad<TVisitable>(
            object dto,
            out TVisitable visitable,
            IVisitor rootVisitor)
        {
            TimeSpanTimerDTO castedDTO = dto as TimeSpanTimerDTO;

            if (castedDTO == null)
            {
                logger?.LogError(
                    GetType(),
                    $"DTO IS NOT OF TYPE: {nameof(TimeSpanTimerDTO)}");

                visitable = default;

                return false;
            }

            var timer = timerFactory.BuildTimeSpanTimer();

            timer.Context.DTO = castedDTO;

            timer.Context.SetState(castedDTO.State);

            visitable = timer.CastFromTo<TimeSpanTimer, TVisitable>();

            return true;
        }

        public bool VisitLoad(
            object dto,
            Type visitableType,
            out object visitableObject,
            IVisitor rootVisitor)
        {
            TimeSpanTimerDTO castedDTO = dto as TimeSpanTimerDTO;

            if (castedDTO == null)
            {
                logger?.LogError(
                    GetType(),
                    $"DTO IS NOT OF TYPE: {nameof(TimeSpanTimerDTO)}");

                visitableObject = default;

                return false;
            }

            var timer = timerFactory.BuildTimeSpanTimer();

            timer.Context.DTO = castedDTO;

            timer.Context.SetState(castedDTO.State);

            visitableObject = timer;

            return true;
        }

        #endregion

        #region IPopulateVisitor

        public bool VisitPopulate<TVisitable>(
            object dto,
            TVisitable visitable,
            IVisitor rootVisitor)
        {
            TimeSpanTimerDTO castedDTO = dto as TimeSpanTimerDTO;

            if (castedDTO == null)
            {
                logger?.LogError(
                    GetType(),
                    $"DTO IS NOT OF TYPE: {nameof(TimeSpanTimerDTO)}");

                return false;
            }

            TimeSpanTimer timer = visitable as TimeSpanTimer;

            if (timer == null)
            {
                logger?.LogError(
                    GetType(),
                    $"VISITABLE IS NOT OF TYPE: {nameof(TimeSpanTimer)}");

                return false;
            }

            timer.Context.DTO = castedDTO;

            timer.Context.SetState(castedDTO.State);

            return true;
        }

        public bool VisitPopulate(
            object dto,
            Type visitableType,
            object visitableObject,
            IVisitor rootVisitor)
        {
            TimeSpanTimerDTO castedDTO = dto as TimeSpanTimerDTO;

            if (castedDTO == null)
            {
                logger?.LogError(
                    GetType(),
                    $"DTO IS NOT OF TYPE: {nameof(TimeSpanTimerDTO)}");

                return false;
            }

            TimeSpanTimer timer = visitableObject as TimeSpanTimer;

            if (timer == null)
            {
                logger?.LogError(
                    GetType(),
                    $"VISITABLE IS NOT OF TYPE: {nameof(TimeSpanTimer)}");

                return false;
            }

            timer.Context.DTO = castedDTO;

            timer.Context.SetState(castedDTO.State);

            return true;
        }

        #endregion
    }
}