using System.Collections;
using System.Collections.Generic;

namespace HereticalSolutions
{
	//It is IMPORTANT that the enumerator is struct so that it does create allocations
	public struct LinkedListEnumerator<T> : IEnumerator<T>
	{
		private LinkedListNode<T> first;
		private LinkedListNode<T> current;
		private bool started;

		public LinkedListEnumerator(LinkedListNode<T> first)
		{
			this.first = first;
			current = null;
			started = false;
		}

		public T Current => current.Value;
		object IEnumerator.Current => Current;

		public void Dispose() { }

		public bool MoveNext()
		{
			if (!started)
			{
				current = first;
				started = true;
			}
			else if (current != null)
			{
				current = current.Next;
			}

			return current != null;
		}

		public void Reset()
		{
			current = null;
			started = false;
		}
	}
}