using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.ObjectPools.Managed;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Decorators.Appendable
{
	public class AppendableDecoratorManagedPool<T>
		: ADecoratorManagedPool<T>
	{
		private readonly IAllocationCommand<T> nullAllocationCommand;

		private readonly List<IPoolElementFacade<T>> skippedElements;

		private readonly ILogger logger;

		public AppendableDecoratorManagedPool(
			IManagedPool<T> innerPool,
			List<IPoolElementFacade<T>> skippedElements,
			IAllocationCommand<T> nullAllocationCommand,
			ILogger logger)
			: base(innerPool)
		{
			this.skippedElements = skippedElements;

			this.nullAllocationCommand = nullAllocationCommand;

			this.logger = logger;
		}

		public override IPoolElementFacade<T> Pop(
			IPoolPopArgument[] args)
		{
			#region Append from argument

			if (args.TryGetArgument<AppendToPoolArgument<T>>(
				out var arg))
			{
				logger?.Log(
					GetType(),
					"APPEND ARGUMENT RECEIVED, APPENDING");

				IManagedResizable<T> innerPoolAsResizeable = innerPool
					as IManagedResizable<T>;

				IManagedPoolInternal<T> innerPoolInternal = innerPool
					as IManagedPoolInternal<T>;

				innerPoolAsResizeable.Resize(
					nullAllocationCommand,
					false);

				skippedElements.Clear();

				IPoolElementFacade<T> result = null;

				while (true)
				{
					result = innerPoolInternal.PopFacade();

					if (result.Status == EPoolElementStatus.UNINITIALIZED)
					{
						break;
					}

					skippedElements.Add(result);
				}

				for (int i = 0; i < skippedElements.Count; i++)
				{
					innerPoolInternal.PushFacade(skippedElements[i]);
				}

				skippedElements.Clear();

				//Validate pool

				if (result.Pool == null)
				{
					result.Pool = this;
				}

				//Update facade

				result.Value = arg.Value;

				result.Status = EPoolElementStatus.UNINITIALIZED;

				return result;
			}

			#endregion

			return base.Pop(args);
		}
	}
}