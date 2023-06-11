using System;

using HereticalSolutions.Persistence.Visitors;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Time;
using HereticalSolutions.Time.Visitors;

namespace HereticalSolutions.Persistence.Factories
{
    public static partial class TimeFactory
    {
        public static CompositeVisitor BuildSimpleCompositeVisitorWithTimerVisitors()
        {
            #region Load visitors repository
            
            IRepository<Type, object> loadVisitorsDatabase = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            loadVisitorsDatabase.Add(typeof(IPersistentTimer), new PersistentTimerVisitor());
            loadVisitorsDatabase.Add(typeof(IRuntimeTimer), new RuntimeTimerVisitor());
            
            IReadOnlyObjectRepository loadVisitorsRepository = RepositoriesFactory.BuildDictionaryObjectRepository(loadVisitorsDatabase);
            
            #endregion
            
            #region Save visitors repository
            
            IRepository<Type, object> saveVisitorsDatabase = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            saveVisitorsDatabase.Add(typeof(IPersistentTimer), new PersistentTimerVisitor());
            saveVisitorsDatabase.Add(typeof(IRuntimeTimer), new RuntimeTimerVisitor());
            
            IReadOnlyObjectRepository saveVisitorsRepository = RepositoriesFactory.BuildDictionaryObjectRepository(saveVisitorsDatabase);
            
            #endregion

            return PersistenceFactory.BuildCompositeVisitor(
                loadVisitorsRepository,
                saveVisitorsRepository);
        }
    }
}