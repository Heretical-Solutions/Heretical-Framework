using System.Collections.Generic;

using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Pools.Arguments;

namespace HereticalSolutions.Pools.Factories
{
	public class ArgumentBuilder
	{
		private readonly List<IPoolDecoratorArgument> argumentChain = new List<IPoolDecoratorArgument>();

		public ArgumentBuilder Add<TArgument>(out TArgument instance) where TArgument : IPoolDecoratorArgument
		{
			instance = AllocationsFactory.ActivatorAllocationDelegate<TArgument>();

			argumentChain.Add(instance);

			return this;
		}

		public IPoolDecoratorArgument[] Build()
		{
			return argumentChain.ToArray();
		}
	}
}