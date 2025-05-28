using System;

namespace HereticalSolutions.Persistence
{
	public interface IVisitor
	{
		bool CanVisit<TVisitable>(
			TVisitable instance);

		bool CanVisit(
			Type visitableType,
			object instance);

		Type GetDTOType<TVisitable>(
			TVisitable instance);

		Type GetDTOType(
			Type visitableType,
			object instance);
	}
}