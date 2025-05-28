using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

namespace HereticalSolutions.SpacePartitioning.Factories
{
	public class SpacePartitioningFactory
	{
		private readonly RepositoryFactory repositoryFactory;

		private readonly NodePoolFactory nodePoolFactory;

		private readonly ILoggerResolver loggerResolver;

		public SpacePartitioningFactory(
			RepositoryFactory repositoryFactory,
			NodePoolFactory nodePoolFactory,
			ILoggerResolver loggerResolver)
		{
			this.repositoryFactory = repositoryFactory;

			this.nodePoolFactory = nodePoolFactory;

			this.loggerResolver = loggerResolver;
		}

		public Quadtree<TValue> BuildQuadtree<TValue>(
			Vector2 min,
			Vector2 max)
		{
			ILogger logger =
				loggerResolver?.GetLogger<Quadtree<TValue>>();

			var result = new Quadtree<TValue>(
				repositoryFactory.BuildDictionaryRepository<TValue, ValueSpaceData<TValue>>(),
				nodePoolFactory.BuildNodePool<TValue>(),
				nodePoolFactory.BuildValueDataPool<TValue>(),
				new DistanceComparer2D<TValue>(),
				new Bounds2D(
					(min + max) / 2f,
					(max - min)),
				logger);

			Node<TValue> root = result.AllocateNode(
				new Bounds2D(
					min.x,
					min.y,
					max.x - min.x,
					max.y - min.y),
				null,
				0);

			result.Root = root;

			return result;
		}
	}
}