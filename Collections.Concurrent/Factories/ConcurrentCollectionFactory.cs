using System.Threading;

using HereticalSolutions.Collections.Factories;

namespace HereticalSolutions.Collections.Concurrent.Factories
{
	public class ConcurrentCollectionFactory
	{
		private readonly CollectionFactory collectionFactory;

		public ConcurrentCollectionFactory(
			CollectionFactory collectionFactory)
		{
			this.collectionFactory = collectionFactory;
		}

		#region B+ trees

		public ConcurrentBPlusTree<T> BuildConcurrentBPlusTree<T>()
		{
			return new ConcurrentBPlusTree<T>(
				collectionFactory.BuildBPlusTree<T>(
					collectionFactory.BPlusTreeDegree),
				new SemaphoreSlim(1, 1));
		}

		public ConcurrentBPlusTree<T> BuildConcurrentBPlusTree<T>(
			int degree)
		{
			return new ConcurrentBPlusTree<T>(
				collectionFactory.BuildBPlusTree<T>(degree),
				new SemaphoreSlim(1, 1));
		}

		public ConcurrentBPlusTreeMap<TKey, TValue>
			BuildConcurrentBPlusTreeMap<TKey, TValue>()
		{
			return new ConcurrentBPlusTreeMap<TKey, TValue>(
				collectionFactory.BuildBPlusTreeMap<TKey, TValue>(
					collectionFactory.BPlusTreeDegree),
				new SemaphoreSlim(1, 1));
		}

		public ConcurrentBPlusTreeMap<TKey, TValue>
			BuildConcurrentBPlusTreeMap<TKey, TValue>(
			int degree)
		{
			return new ConcurrentBPlusTreeMap<TKey, TValue>(
				collectionFactory.BuildBPlusTreeMap<TKey, TValue>(
					degree),
				new SemaphoreSlim(1, 1));
		}

		#endregion

		#region Circular buffers

		public MPMCCircularBuffer<T> BuildMPMCCircularBuffer<T>()
		{
			var buffer = new T[collectionFactory.CircularBufferCapacity];

			for (int i = 0; i < collectionFactory.CircularBufferCapacity; i++)
			{
				buffer[i] = default;
			}

			var states =
				new AtomicElementState[collectionFactory.CircularBufferCapacity];

			for (int i = 0; i < collectionFactory.CircularBufferCapacity; i++)
			{
				states[i] = new AtomicElementState(
					EBufferElementState.CONSUMED);
			}

			return new MPMCCircularBuffer<T>(
				buffer,
				states);
		}

		#endregion
	}
}