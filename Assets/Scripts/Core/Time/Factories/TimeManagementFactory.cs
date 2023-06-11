using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Time.Factories
{
    public static partial class TimeFactory
    {
        public static TimeManager BuildTimeManager()
        {
            return new TimeManager(
                RepositoriesFactory.BuildDictionaryRepository<string, ISynchronizable>());
        }

        public static SynchronizationContext BuildSynchronizationContext(
            string id,
            bool canBeToggled,
            bool canScale)
        {
            var broadcaster = DelegatesFactory.BuildNonAllocBroadcasterGeneric<float>();

            return new SynchronizationContext(
                new SynchronizationContextDescriptor(
                    id,
                    canBeToggled,
                    canScale),
                broadcaster,
                broadcaster);
        }
    }
}