using System.Collections.Generic;

namespace HereticalSolutions.Bags.Factories
{
	public class LinkedListBagFactory
	{
		#region Linked list bag

		public LinkedListBag<T> BuildLinkedListBag<T>()
		{
			var linkedList = new LinkedList<T>();

			return new LinkedListBag<T>(
				linkedList);
		}

		#endregion
	}
}