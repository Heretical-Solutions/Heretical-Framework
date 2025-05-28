using System;

using HereticalSolutions.Persistence;

using HereticalSolutions.TypeConversion;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ProceduralEnums
{
	[Visitor(typeof(ProcEnum<>), typeof(string))]
	public class ProcEnumVisitor<TValue>
		: ISaveVisitor,
		  //ILoadVisitor,
		  IPopulateVisitor
	{
		private readonly ITypeConverter<string> stringConverter;

		private readonly ILogger logger;

		public ProcEnumVisitor(
			ITypeConverter<string> stringConverter,
			ILogger logger)
		{
			this.stringConverter = stringConverter;

			this.logger = logger;
		}

		#region IVisitor

		public bool CanVisit<TVisitable>(
			TVisitable instance)
		{
			return typeof(TVisitable).IsSameOrInheritor(typeof(ProcEnum<>));
		}

		public bool CanVisit(
			Type visitableType,
			object instance)
		{
			return visitableType.IsSameOrInheritor(typeof(ProcEnum<>));
		}

		public Type GetDTOType<TVisitable>(
			TVisitable instance)
		{
			if (!typeof(TVisitable).IsSameOrInheritor(typeof(ProcEnum<>)))
				return null;

			return typeof(string);
		}

		public Type GetDTOType(
			Type visitableType,
			object instance)
		{
			if (!visitableType.IsSameOrInheritor(typeof(ProcEnum<>)))
				return null;

			return typeof(string);
		}

		#endregion

		#region ISaveVisitor

		public bool VisitSave<TVisitable>(
			ref object dto,
			TVisitable visitable,
			IVisitor rootVisitor)
		{
			if (visitable is not TValue value)
			{
				logger?.LogError(
					GetType(),
					$"VISITABLE IS NOT OF TYPE: {typeof(TValue).Name}");

				return false;
			}
			
			if (!stringConverter.ConvertToTargetType(
				value.GetType(),
				value,
				out var stringValue))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO STRING: {value}");

				dto = null;

				return false;
			}
				
			dto = stringValue;

			return true;
		}

		public bool VisitSave(
			ref object dto,
			Type visitableType,
			object visitableObject,
			IVisitor rootVisitor)
		{
			if (visitableObject is not TValue value)
			{
				logger?.LogError(
					GetType(),
					$"VISITABLE IS NOT OF TYPE: {typeof(TValue).Name}");

				return false;
			}

			if (!stringConverter.ConvertToTargetType(
				value.GetType(),
				value,
				out var stringValue))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO STRING: {value}");

				dto = null;

				return false;
			}

			dto = stringValue;

			return true;
		}

		#endregion

		#region IPopulateVisitor

		public bool VisitPopulate<TVisitable>(
			object dto,
			TVisitable visitable,
			IVisitor rootVisitor)
		{
			if (dto is not string castedDTO)
			{
				logger?.LogError(
					GetType(),
					$"DTO IS NOT OF TYPE: {nameof(System.String)}");

				return false;
			}

			if (visitable is not ProcEnum<TValue> procEnum)
			{
				logger?.LogError(
					GetType(),
					$"VISITABLE IS NOT OF TYPE: {typeof(ProcEnum<TValue>).Name}");

				return false;
			}

			if (!stringConverter.ConvertFromTargetType<TValue>(
				castedDTO,
				out var value))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO PROCENUM: {castedDTO}");

				return false;
			}

			procEnum.Value = value;

			return true;
		}

		public bool VisitPopulate(
			object dto,
			Type visitableType,
			object visitableObject,
			IVisitor rootVisitor)
		{
			if (dto is not string castedDTO)
			{
				logger?.LogError(
					GetType(),
					$"DTO IS NOT OF TYPE: {nameof(System.String)}");

				return false;
			}

			if (visitableObject is not ProcEnum<TValue> procEnum)
			{
				logger?.LogError(
					GetType(),
					$"VISITABLE IS NOT OF TYPE: {typeof(ProcEnum<TValue>).Name}");

				return false;
			}

			if (!stringConverter.ConvertFromTargetType<TValue>(
				castedDTO,
				out var value))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO PROCENUM: {castedDTO}");

				return false;
			}

			procEnum.Value = value;

			return true;
		}

		#endregion
	}
}