using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public class DispatchVisitor
		: ISaveVisitor,
		  ILoadVisitor,
		  IPopulateVisitor
	{
		private readonly IReadOnlyRepository<Type, IEnumerable<IVisitor>> 
			concreteVisitorRepository;

		private readonly IEnumerable<IVisitor> fallbackVisitors;

		private readonly ILogger logger;

		public DispatchVisitor(
			IReadOnlyRepository<Type, IEnumerable<IVisitor>> concreteVisitorRepository,
			IEnumerable<IVisitor> fallbackVisitors,
			ILogger logger)
		{
			this.concreteVisitorRepository = concreteVisitorRepository;

			this.fallbackVisitors = fallbackVisitors;

			this.logger = logger;
		}

		#region IVisitor

		public bool CanVisit<TVisitable>(
			TVisitable instance)
		{
			return GetConcreteVisitor<TVisitable>(
				instance,
				out _);
		}

		public bool CanVisit(
			Type visitableType,
			object instance)
		{
			return GetConcreteVisitor(
				visitableType,
				instance,
				out _);
		}

		public Type GetDTOType<TVisitable>(
			TVisitable instance)
		{
			if (!GetConcreteVisitor<TVisitable>(
				instance,
				out var concreteVisitor))
			{
				return null;
			}

			return concreteVisitor.GetDTOType<TVisitable>(
				instance);
		}

		public Type GetDTOType(
			Type visitableType,
			object instance)
		{
			if (!GetConcreteVisitor(
				visitableType,
				instance,
				out var concreteVisitor))
			{
				return null;
			}

			return concreteVisitor.GetDTOType(
				visitableType,
				instance);
		}

		#endregion

		#region ISaveVisitor

		public bool VisitSave<TVisitable>(
			ref object dto,
			TVisitable visitable,
			IVisitor rootVisitor)
		{
			if (!GetConcreteVisitor<TVisitable>(
				visitable,
				out var concreteVisitor))
			{
				logger?.LogError(
					GetType(),
					$"NO VISITOR REGISTERED FOR VISITABLE TYPE: {typeof(TVisitable).Name}");

				dto = null;

				return false;
			}
			
			if (concreteVisitor is ISaveVisitor concreteSaveVisitor)
			{
				if (concreteSaveVisitor.VisitSave<TVisitable>(
					ref dto,
					visitable,
					(rootVisitor ?? this)))
				{
					return true;
				}
				
				logger?.LogError(
					GetType(),
					$"FAILED TO SAVE VISITABLE TYPE: {typeof(TVisitable).Name}");

				dto = null;

				return false;
			}

			logger?.LogError(
				GetType(),
				$"NO CONCRETE SAVE VISITOR REGISTERED FOR VISITABLE TYPE: {typeof(TVisitable).Name}");

			dto = default;

			return false;
		}

		public bool VisitSave(
			ref object dto,
			Type visitableType,
			object visitableObject,
			IVisitor rootVisitor)
		{
			if (!GetConcreteVisitor(
				visitableType,
				visitableObject,
				out var concreteVisitor))
			{
				logger?.LogError(
					GetType(),
					$"NO VISITOR REGISTERED FOR VISITABLE TYPE: {visitableType}");

				dto = null;

				return false;
			}

			if (concreteVisitor is ISaveVisitor concreteSaveVisitor)
			{
				if (concreteSaveVisitor.VisitSave(
					ref dto,
					visitableType,
					visitableObject,
					(rootVisitor ?? this)))
				{
					return true;
				}

				logger?.LogError(
					GetType(),
					$"FAILED TO SAVE VISITABLE TYPE: {visitableType.Name}");

				dto = null;

				return false;
			}

			logger?.LogError(
				GetType(),
				$"NO CONCRETE SAVE VISITOR REGISTERED FOR VISITABLE TYPE: {visitableType.Name}");

			dto = default;

			return false;
		}

		#endregion

		#region ILoadVisitor

		public bool VisitLoad<TVisitable>(
			object dto,
			out TVisitable visitable,
			IVisitor rootVisitor)
		{
			if (!GetConcreteVisitor<TVisitable>(
				default,
				out var concreteVisitor))
			{
				logger?.LogError(
					GetType(),
					$"NO VISITOR REGISTERED FOR VISITABLE TYPE: {typeof(TVisitable).Name}");

				visitable = default;

				return false;
			}

			if (concreteVisitor is ILoadVisitor concreteLoadVisitor)
			{
				if (concreteLoadVisitor.VisitLoad<TVisitable>(
					dto,
					out var nonCastedVisitable,
					(rootVisitor ?? this)))
				{
					if (nonCastedVisitable is TVisitable castedVisitable)
					{
						visitable = castedVisitable;

						return true;
					}
					
					logger?.LogError(
						GetType(),
						$"FAILED TO CAST VISITABLE {nonCastedVisitable.GetType().Name} TO TYPE: {typeof(TVisitable).Name}");

					visitable = default;

					return false;
				}
				
				logger?.LogError(
					GetType(),
					$"FAILED TO LOAD VISITABLE TYPE: {typeof(TVisitable).Name}");

				visitable = default;

				return false;
			}

			logger?.LogError(
				GetType(),
				$"NO CONCRETE LOAD VISITOR REGISTERED FOR VISITABLE TYPE: {typeof(TVisitable).Name}");

			visitable = default;

			return false;
		}

		public bool VisitLoad(
			object dto,
			Type visitableType,
			out object visitableObject,
			IVisitor rootVisitor)
		{
			if (!GetConcreteVisitor(
				visitableType,
				default,
				out var concreteVisitor))
			{
				logger?.LogError(
					GetType(),
					$"NO VISITOR REGISTERED FOR VISITABLE TYPE: {visitableType.Name}");

				visitableObject = null;

				return false;
			}

			if (concreteVisitor is ILoadVisitor concreteLoadVisitor)
			{
				if (concreteLoadVisitor.VisitLoad(
					dto,
					visitableType,
					out visitableObject,
					(rootVisitor ?? this)))
				{
					return true;
				}

				logger?.LogError(
					GetType(),
					$"FAILED TO LOAD VISITABLE TYPE: {visitableType.Name}");

				visitableObject = default;

				return false;
			}

			logger?.LogError(
				GetType(),
				$"NO CONCRETE LOAD VISITOR REGISTERED FOR VISITABLE TYPE: {visitableType.Name}");

			visitableObject = default;

			return false;
		}

		#endregion

		#region IPopulateVisitor

		public bool VisitPopulate<TVisitable>(
			object dto,
			TVisitable visitable,
			IVisitor rootVisitor)
		{
			if (!GetConcreteVisitor<TVisitable>(
				visitable,
				out var concreteVisitor))
			{
				logger?.LogError(
					GetType(),
					$"NO VISITOR REGISTERED FOR VISITABLE TYPE: {typeof(TVisitable).Name}");

				return false;
			}

			IVisitable visitableDowncasted = visitable as IVisitable;

			if (concreteVisitor is IPopulateVisitor concretePopulateVisitor)
			{
				if (concretePopulateVisitor.VisitPopulate(
					dto,
					visitableDowncasted,
					(rootVisitor ?? this)))
				{
					return true;
				}
				
				logger?.LogError(
					GetType(),
					$"FAILED TO POPULATE VISITABLE TYPE: {typeof(TVisitable).Name}");

				return false;
			}

			logger?.LogError(
				GetType(),
				$"NO CONCRETE POPULATE VISITOR REGISTERED FOR VISITABLE TYPE: {typeof(TVisitable).Name}");

			return false;
		}

		public bool VisitPopulate(
			object dto,
			Type visitableType,
			object visitableObject,
			IVisitor rootVisitor)
		{
			if (!GetConcreteVisitor(
				visitableType,
				visitableObject,
				out var concreteVisitor))
			{
				logger?.LogError(
					GetType(),
					$"NO VISITOR REGISTERED FOR VISITABLE TYPE: {visitableType.Name}");

				return false;
			}

			if (concreteVisitor is IPopulateVisitor concretePopulateVisitor)
			{
				if (concretePopulateVisitor.VisitPopulate(
					dto,
					visitableObject,
					(rootVisitor ?? this)))
				{
					return true;
				}

				logger?.LogError(
					GetType(),
					$"FAILED TO POPULATE VISITABLE TYPE: {visitableType.Name}");

				return false;
			}

			logger?.LogError(
				GetType(),
				$"NO CONCRETE POPULATE VISITOR REGISTERED FOR VISITABLE TYPE: {visitableType.Name}");

			return false;
		}

		#endregion

		private bool GetConcreteVisitor<TVisitable>(
			TVisitable instance,
			out IVisitor visitor)
		{
			visitor = default;

			if (!concreteVisitorRepository.TryGetByTypeOrInheritor<IEnumerable<IVisitor>>(
				typeof(TVisitable),
				out IEnumerable<IVisitor> concreteVisitors))
			{
				return false;
			}

			foreach (IVisitor concreteVisitor in concreteVisitors)
			{
				if (concreteVisitor is ISaveVisitor concreteSaveVisitor)
				{
					if (concreteSaveVisitor.CanVisit<TVisitable>(
						instance))
					{
						visitor = concreteVisitor;

						return true;
					}
				}
			}

			return false;
		}

		private bool GetConcreteVisitor(
			Type visitableType,
			object instance,
			out IVisitor visitor)
		{
			visitor = default;

			if (concreteVisitorRepository.TryGetByTypeOrInheritor<IEnumerable<IVisitor>>(
				visitableType,
				out IEnumerable<IVisitor> concreteVisitors))
			{
				foreach (IVisitor concreteVisitor in concreteVisitors)
				{
					if (concreteVisitor is ISaveVisitor concreteSaveVisitor)
					{
						if (concreteSaveVisitor.CanVisit(
							visitableType,
							instance))
						{
							visitor = concreteVisitor;
	
							return true;
						}
					}
				}
			}

			foreach (IVisitor concreteVisitor in fallbackVisitors)
			{
				if (concreteVisitor is ISaveVisitor concreteSaveVisitor)
				{
					if (concreteSaveVisitor.CanVisit(
						visitableType,
						instance))
					{
						visitor = concreteVisitor;

						return true;
					}
				}
			}

			return false;
		}
	}
}