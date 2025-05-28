using System;

namespace HereticalSolutions.Persistence
{
    public interface ILoadVisitor
        : IVisitor
    {
        bool VisitLoad<TVisitable>(
            object dto,
            out TVisitable visitable,
            IVisitor rootVisitor);

        bool VisitLoad(
            object dto,
            Type visitableType,
            out object visitableObject,
            IVisitor rootVisitor);
    }
}