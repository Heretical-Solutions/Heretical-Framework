using System;

namespace HereticalSolutions.Persistence
{
	public interface IPopulateVisitor
		: IVisitor
	{
		bool VisitPopulate<TVisitable>(
			object dto,
			TVisitable visitable,
			IVisitor rootVisitor);

		bool VisitPopulate(
			object dto,
			Type visitableType,
			object visitableObject,
			IVisitor rootVisitor);
	}
}