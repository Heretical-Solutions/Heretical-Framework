using System;

using HereticalSolutions.Persistence;

using HereticalSolutions.Synchronization.Time.Timers.TickCollection.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Synchronization.Time.Timers.TickCollection
{
	[Visitor(typeof(TickTimer), typeof(TickTimerDTO))]
	public class TickTimerVisitor
		: ISaveVisitor,
		  ILoadVisitor,
		  IPopulateVisitor
	{
		private readonly TickTimerFactory timerFactory;

		private readonly ILogger logger;

		public TickTimerVisitor(
			TickTimerFactory timerFactory,
			ILogger logger)
		{
			this.timerFactory = timerFactory;

			this.logger = logger;
		}

		#region IVisitor

		public bool CanVisit<TVisitable>(
			TVisitable instance)
		{
			return typeof(TVisitable) == typeof(TickTimer);
		}

		public bool CanVisit(
			Type visitableType,
			object instance)
		{
			return visitableType == typeof(TickTimer);
		}

		public Type GetDTOType<TVisitable>(
			TVisitable instance)
		{
			if (typeof(TVisitable) != typeof(TickTimer))
				return null;

			return typeof(TickTimerDTO);
		}

		public Type GetDTOType(
			Type visitableType,
			object instance)
		{
			if (visitableType != typeof(TickTimer))
				return null;

			return typeof(TickTimerDTO);
		}

		#endregion

		#region ISaveVisitor

		public bool VisitSave<TVisitable>(
			ref object dto,
			TVisitable visitable,
			IVisitor rootVisitor)
		{
			TickTimer timer = visitable as TickTimer;

			if (timer == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITABLE IS NOT OF TYPE: {nameof(TickTimer)}");

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
			TickTimer timer = visitableObject as TickTimer;

			if (timer == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITABLE IS NOT OF TYPE: {nameof(TickTimer)}");

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
			TickTimerDTO castedDTO = dto as TickTimerDTO;

			if (castedDTO == null)
			{
				logger?.LogError(
					GetType(),
					$"DTO IS NOT OF TYPE: {nameof(TickTimerDTO)}");

				visitable = default;

				return false;
			}

			var timer = timerFactory.BuildTickTimer();

			timer.Context.DTO = castedDTO;

			timer.Context.SetState(castedDTO.State);

			visitable = timer.CastFromTo<TickTimer, TVisitable>();

			return true;
		}

		public bool VisitLoad(
			object dto,
			Type visitableType,
			out object visitableObject,
			IVisitor rootVisitor)
		{
			TickTimerDTO castedDTO = dto as TickTimerDTO;

			if (castedDTO == null)
			{
				logger?.LogError(
					GetType(),
					$"DTO IS NOT OF TYPE: {nameof(TickTimerDTO)}");

				visitableObject = default;

				return false;
			}

			var timer = timerFactory.BuildTickTimer();

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
			TickTimerDTO castedDTO = dto as TickTimerDTO;

			if (castedDTO == null)
			{
				logger?.LogError(
					GetType(),
					$"DTO IS NOT OF TYPE: {nameof(TickTimerDTO)}");

				return false;
			}

			TickTimer timer = visitable as TickTimer;

			if (timer == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITABLE IS NOT OF TYPE: {nameof(TickTimer)}");

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
			TickTimerDTO castedDTO = dto as TickTimerDTO;

			if (castedDTO == null)
			{
				logger?.LogError(
					GetType(),
					$"DTO IS NOT OF TYPE: {nameof(TickTimerDTO)}");

				return false;
			}

			TickTimer timer = visitableObject as TickTimer;

			if (timer == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITABLE IS NOT OF TYPE: {nameof(TickTimer)}");

				return false;
			}

			timer.Context.DTO = castedDTO;

			timer.Context.SetState(castedDTO.State);

			return true;
		}

		#endregion
	}
}