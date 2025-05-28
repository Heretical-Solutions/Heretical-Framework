using System.Threading.Tasks;
using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations.Async;

using HereticalSolutions.ObjectPools.Managed;
using HereticalSolutions.ObjectPools.Managed.Async;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ObjectPools.Decorators.Appendable
{
	public class AppendableAsyncDecoratorManagedPool<T>
		: AAsyncDecoratorManagedPool<T>
	{
		private readonly IAsyncAllocationCommand<T> nullAllocationCommand;

		private readonly List<IAsyncPoolElementFacade<T>> skippedElements;

		private readonly ILogger logger;

		public AppendableAsyncDecoratorManagedPool(
			IAsyncManagedPool<T> innerPool,
			List<IAsyncPoolElementFacade<T>> skippedElements,
			IAsyncAllocationCommand<T> nullAllocationCommand,
			ILogger logger)
			: base(innerPool)
		{
			this.skippedElements = skippedElements;

			this.nullAllocationCommand = nullAllocationCommand;

			this.logger = logger;
		}

		public override async Task<IAsyncPoolElementFacade<T>> Pop(
			IPoolPopArgument[] args,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			#region Append from argument

			if (args.TryGetArgument<AppendToPoolArgument<T>>(
				out var arg))
			{
				logger?.Log(
					GetType(),
					"APPEND ARGUMENT RECEIVED, APPENDING");

				IAsyncManagedResizable<T> innerPoolAsResizeable = innerPool
					as IAsyncManagedResizable<T>;

				IAsyncManagedPoolInternal<T> innerPoolInternal = innerPool
					as IAsyncManagedPoolInternal<T>;

				await innerPoolAsResizeable.Resize(
					nullAllocationCommand,
					false,
					
					asyncContext);

				skippedElements.Clear();

				IAsyncPoolElementFacade<T> result = null;

				while (true)
				{
					result = await innerPoolInternal.PopFacade(
						asyncContext);

					if (result.Status == EPoolElementStatus.UNINITIALIZED)
					{
						break;
					}

					skippedElements.Add(result);
				}

				for (int i = 0; i < skippedElements.Count; i++)
				{
					await innerPoolInternal.PushFacade(
						skippedElements[i],
						asyncContext);
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

			return await base.Pop(
				args,
				asyncContext);
		}
	}
}