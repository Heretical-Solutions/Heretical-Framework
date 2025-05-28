using System;

using HereticalSolutions.Persistence;

using HereticalSolutions.Synchronization.Time.Timers.FloatDelta.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Synchronization.Time.Timers.FloatDelta
{
    [Visitor(typeof(FloatTimer), typeof(FloatTimerDTO))]
    public class FloatTimerVisitor
        : ISaveVisitor,
          ILoadVisitor,
          IPopulateVisitor
    {
        private readonly FloatTimerFactory timerFactory;

        private readonly ILogger logger;

        public FloatTimerVisitor(
            FloatTimerFactory timerFactory,
            ILogger logger)
        {
            this.timerFactory = timerFactory;

            this.logger = logger;
        }

        #region IVisitor

        public bool CanVisit<TVisitable>(
            TVisitable instance)
        {
            return typeof(TVisitable) == typeof(FloatTimer);
        }

        public bool CanVisit(
            Type visitableType,
            object isntance)
        {
            return visitableType == typeof(FloatTimer);
        }

        public Type GetDTOType<TVisitable>(
            TVisitable instance)
        {
            if (typeof(TVisitable) != typeof(FloatTimer))
                return null;

            return typeof(FloatTimerDTO);
        }

        public Type GetDTOType(
            Type visitableType,
            object isntance)
        {
            if (visitableType != typeof(FloatTimer))
                return null;

            return typeof(FloatTimerDTO);
        }

        #endregion

        #region ISaveVisitor

        public bool VisitSave<TVisitable>(
            ref object dto,
            TVisitable visitable,
            IVisitor rootVisitor)
        {
            FloatTimer timer = visitable as FloatTimer;

            if (timer == null)
            {
                logger?.LogError(
                    GetType(),
                    $"VISITABLE IS NOT OF TYPE: {nameof(FloatTimer)}");

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
            FloatTimer timer = visitableObject as FloatTimer;

            if (timer == null)
            {
                logger?.LogError(
                    GetType(),
                    $"VISITABLE IS NOT OF TYPE: {nameof(FloatTimer)}");

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
            FloatTimerDTO castedDTO = dto as FloatTimerDTO;

            if (castedDTO == null)
            {
                logger?.LogError(
                    GetType(),
                    $"DTO IS NOT OF TYPE: {nameof(FloatTimerDTO)}");

                visitable = default;

                return false;
            }

            var timer = timerFactory.BuildFloatTimer();

            timer.Context.DTO = castedDTO;

            timer.Context.SetState(castedDTO.State);

            visitable = timer.CastFromTo<FloatTimer, TVisitable>();

            return true;
        }

        public bool VisitLoad(
            object dto,
            Type visitableType,
            out object visitableObject,
            IVisitor rootVisitor)
        {
            FloatTimerDTO castedDTO = dto as FloatTimerDTO;

            if (castedDTO == null)
            {
                logger?.LogError(
                    GetType(),
                    $"DTO IS NOT OF TYPE: {nameof(FloatTimerDTO)}");

                visitableObject = default;

                return false;
            }

            var timer = timerFactory.BuildFloatTimer();

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
            FloatTimerDTO castedDTO = dto as FloatTimerDTO;

            if (castedDTO == null)
            {
                logger?.LogError(
                    GetType(),
                    $"DTO IS NOT OF TYPE: {nameof(FloatTimerDTO)}");

                return false;
            }

            FloatTimer timer = visitable as FloatTimer;

            if (timer == null)
            {
                logger?.LogError(
                    GetType(),
                    $"VISITABLE IS NOT OF TYPE: {nameof(FloatTimer)}");

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
            FloatTimerDTO castedDTO = dto as FloatTimerDTO;

            if (castedDTO == null)
            {
                logger?.LogError(
                    GetType(),
                    $"DTO IS NOT OF TYPE: {nameof(FloatTimerDTO)}");

                return false;
            }

            FloatTimer timer = visitableObject as FloatTimer;

            if (timer == null)
            {
                logger?.LogError(
                    GetType(),
                    $"VISITABLE IS NOT OF TYPE: {nameof(FloatTimer)}");

                return false;
            }

            timer.Context.DTO = castedDTO;

            timer.Context.SetState(castedDTO.State);

            return true;
        }

        #endregion
    }
}