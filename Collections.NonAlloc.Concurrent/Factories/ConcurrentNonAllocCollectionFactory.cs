using System.Threading;

using HereticalSolutions.ObjectPools;

using HereticalSolutions.Collections.Factories;
using HereticalSolutions.Collections.NonAlloc.Factories;

namespace HereticalSolutions.Collections.NonAlloc.Concurrent.Factories
{
	public class ConcurrentNonAllocCollectionFactory
	{
		private readonly CollectionFactory collectionFactory;

		private readonly NonAllocCollectionFactory nonAllocCollectionFactory;

		public ConcurrentNonAllocCollectionFactory(
			CollectionFactory collectionFactory,
			NonAllocCollectionFactory nonAllocCollectionFactory)
		{
			this.collectionFactory = collectionFactory;

			this.nonAllocCollectionFactory = nonAllocCollectionFactory;
		}

		#region B+ trees

		public ConcurrentNonAllocBPlusTree<T>
			BuildConcurrentNonAllocBPlusTree<T>()
		{
			var nodePool = nonAllocCollectionFactory.
				BuildNonAllocBPlusTreeNodePool<T>();

			return new ConcurrentNonAllocBPlusTree<T>(
				nonAllocCollectionFactory.BuildNonAllocBPlusTree<T>(
					nodePool,
					collectionFactory.BPlusTreeDegree),
				new SemaphoreSlim(1, 1));
		}

		public ConcurrentNonAllocBPlusTree<T>
			BuildConcurrentNonAllocBPlusTree<T>(
				IPool<NonAllocBPlusTreeNode<T>> nodePool,
				int degree)
		{
			return new ConcurrentNonAllocBPlusTree<T>(
				nonAllocCollectionFactory.BuildNonAllocBPlusTree<T>(
					nodePool,
					degree),
				new SemaphoreSlim(1, 1));
		}

		public ConcurrentNonAllocBPlusTreeMap<TKey, TValue>
			BuildConcurrentNonAllocBPlusTreeMap<TKey, TValue>()
		{
			var nodePool = nonAllocCollectionFactory.
				BuildNonAllocBPlusTreeMapNodePool<TKey, TValue>();

			return new ConcurrentNonAllocBPlusTreeMap<TKey, TValue>(
				nonAllocCollectionFactory.BuildNonAllocBPlusTreeMap<TKey, TValue>(
					nodePool,
					collectionFactory.BPlusTreeDegree),
				new SemaphoreSlim(1, 1));
		}

		public ConcurrentNonAllocBPlusTreeMap<TKey, TValue>
			BuildConcurrentNonAllocBPlusTreeMap<TKey, TValue>(
				IPool<NonAllocBPlusTreeMapNode<TKey, TValue>> nodePool,
				int degree)
		{
			return new ConcurrentNonAllocBPlusTreeMap<TKey, TValue>(
				nonAllocCollectionFactory.BuildNonAllocBPlusTreeMap<TKey, TValue>(
					nodePool,
					degree),
				new SemaphoreSlim(1, 1));
		}

		#endregion
	}
}