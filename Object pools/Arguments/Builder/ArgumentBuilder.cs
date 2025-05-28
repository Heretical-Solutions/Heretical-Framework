using System.Collections.Generic;

using HereticalSolutions.Allocations.Factories;

namespace HereticalSolutions.ObjectPools.Builders
{
    public class ArgumentBuilder
    {
        private readonly List<IPoolPopArgument> argumentChain;

        public ArgumentBuilder(
            List<IPoolPopArgument> argumentChain)
        {
            this.argumentChain = argumentChain;
        }

        public ArgumentBuilder Add<TArgument>(
            out TArgument instance)
            where TArgument : IPoolPopArgument
        {
            instance = AllocationFactory.ActivatorAllocationDelegate<TArgument>();

            argumentChain.Add(
                instance);

            return this;
        }

        public IPoolPopArgument[] Build()
        {
            var result = argumentChain.ToArray();

            argumentChain.Clear();

            return result;
        }
    }
}