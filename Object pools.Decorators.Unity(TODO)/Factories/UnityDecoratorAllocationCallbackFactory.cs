using HereticalSolutions.Logging;

using UnityEngine;

namespace HereticalSolutions.ObjectPools.Decorators.Unity.Factories
{
    public class UnityDecoratorAllocationCallbackFactory
    {
        private readonly ILoggerResolver loggerResolver;

        public UnityDecoratorAllocationCallbackFactory(
            ILoggerResolver loggerResolver)
        {
            this.loggerResolver = loggerResolver;
        }

        public RenameByStringAndIndexCallback
            BuildRenameByStringAndIndexCallback(
                string name)
        {
            return new RenameByStringAndIndexCallback(name);
        }

        public SetPrefabAllocationCallback
            BuildSetPrefabAllocationCallback(
                GameObject prefab)
        {
            var logger = loggerResolver.GetLogger<SetPrefabAllocationCallback>();

            return new SetPrefabAllocationCallback(
                prefab,
                logger);
        }
    }
}