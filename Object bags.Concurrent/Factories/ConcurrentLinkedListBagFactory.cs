using System.Threading;

using HereticalSolutions.Bags.Factories;

namespace HereticalSolutions.Bags.Concurrent.Factories
{
	public class ConcurrentLinkedListBagFactory
	{
		private readonly LinkedListBagFactory linkedListBagFactory;

		public ConcurrentLinkedListBagFactory(
			LinkedListBagFactory linkedListBagFactory)
		{
			this.linkedListBagFactory = linkedListBagFactory;
		}

		#region Concurrent linked list bag

		public ConcurrentLinkedListBag<T> BuildConcurrentLinkedListBag<T>()
		{
			return new ConcurrentLinkedListBag<T>(
				linkedListBagFactory.BuildLinkedListBag<T>(),
				new SemaphoreSlim(1, 1));
		}

		#endregion
	}
}