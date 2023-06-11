using HereticalSolutions.Persistence.Visitors;
using HereticalSolutions.Repositories;

namespace HereticalSolutions.Persistence.Factories
{
    public static partial class PersistenceFactory
    {
        public static CompositeVisitor BuildCompositeVisitor(
            IReadOnlyObjectRepository loadVisitorsRepository,
            IReadOnlyObjectRepository saveVisitorsRepository)
        {
            return new CompositeVisitor(
                loadVisitorsRepository,
                saveVisitorsRepository);
        }
    }
}