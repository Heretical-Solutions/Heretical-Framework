using System.Collections;
using System.Collections.Generic;

namespace HereticalSolutions
{
	//It is IMPORTANT that the enumerable is struct so that it does create allocations
	public struct LinkedListEnumerable<T> : IEnumerable<T>
	{
		private LinkedListNode<T> first;

		public LinkedListEnumerable(LinkedListNode<T> first)
		{
			this.first = first;
		}

		public LinkedListEnumerator<T> GetEnumerator() =>
			new LinkedListEnumerator<T>(first);
		IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}