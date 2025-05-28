using System.Collections;
using System.Collections.Generic;

namespace HereticalSolutions.Collections.NonAlloc
{
	public struct NonAllocBPlusTreeEnumerable<T> : IEnumerable<T>
	{
		private readonly NonAllocBPlusTreeNode<T> root;

		public NonAllocBPlusTreeEnumerable(NonAllocBPlusTreeNode<T> root)
		{
			this.root = root;
		}

		public NonAllocBPlusTreeEnumerator<T> GetEnumerator() => new NonAllocBPlusTreeEnumerator<T>(root);
		IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}