using System;

using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.ObjectPools;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Concurrent.Factories
{
	public class ConcurrentBroadcasterFactory
	{
		private readonly BroadcasterFactory broadcasterFactory;

		private readonly ILoggerResolver loggerResolver;

		public ConcurrentBroadcasterFactory(
			BroadcasterFactory broadcasterFactory,
			ILoggerResolver loggerResolver)
		{
			this.broadcasterFactory = broadcasterFactory;

			this.loggerResolver = loggerResolver;
		}

		#region Concurrent broadcaster generic

		public ConcurrentBroadcasterGeneric<T>
			BuildConcurrentBroadcasterGeneric<T>()
		{
			var contextPool = broadcasterFactory.
				BuildContextPool<BroadcasterInvocationContext<T>>();

			return BuildConcurrentBroadcasterGeneric<T>(
				contextPool);
		}

		public ConcurrentBroadcasterGeneric<T>
			BuildConcurrentBroadcasterGeneric<T>(
				IPool<BroadcasterInvocationContext<T>> contextPool)
		{
			ILogger logger =
				loggerResolver?.GetLogger<ConcurrentBroadcasterGeneric<T>>();

			return new ConcurrentBroadcasterGeneric<T>(
				contextPool,
				new object(),
				logger);
		}

		#endregion

		#region Concurrent broadcaster multiple args

		public ConcurrentBroadcasterMultipleArgs
			BuildConcurrentBroadcasterMultipleArgs()
		{
			return new ConcurrentBroadcasterMultipleArgs(
				BuildConcurrentBroadcasterGeneric<object[]>());
		}

		public ConcurrentBroadcasterMultipleArgs
			BuildConcurrentBroadcasterMultipleArgs(
				IPool<BroadcasterInvocationContext<object[]>> contextPool)
		{
			return new ConcurrentBroadcasterMultipleArgs(
				BuildConcurrentBroadcasterGeneric<object[]>(
					contextPool));
		}

		#endregion

		#region Concurrent broadcaster with repository

		public ConcurrentBroadcasterWithRepository
			BuildConcurrentBroadcasterWithRepository(
				IRepository<Type, object> broadcasterRepository)
		{
			ILogger logger =
				loggerResolver?.GetLogger<ConcurrentBroadcasterWithRepository>();

			return new ConcurrentBroadcasterWithRepository(
				broadcasterRepository,
				new object(),
				logger);
		}

		#endregion
	}
}