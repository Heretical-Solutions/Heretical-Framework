using System;

namespace HereticalSolutions.Persistence
{
    public interface ISaveVisitor
        : IVisitor
    {
        bool VisitSave<TVisitable>(
            ref object dto,
            TVisitable visitable,
            IVisitor rootVisitor);

        bool VisitSave(
            ref object dto,
            Type visitableType,
            object visitableObject,
            IVisitor rootVisitor);
    }
}