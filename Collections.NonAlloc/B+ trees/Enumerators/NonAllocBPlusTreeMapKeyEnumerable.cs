using System.Collections;
using System.Collections.Generic;

namespace HereticalSolutions.Collections.NonAlloc
{
	public struct NonAllocBPlusTreeMapKeyEnumerable<TKey, TValue> : IEnumerable<TKey>
	{
		private readonly NonAllocBPlusTreeMapNode<TKey, TValue> root;

		public NonAllocBPlusTreeMapKeyEnumerable(NonAllocBPlusTreeMapNode<TKey, TValue> root)
		{
			this.root = root;
		}

		public NonAllocBPlusTreeMapKeyEnumerator<TKey, TValue> GetEnumerator() => new NonAllocBPlusTreeMapKeyEnumerator<TKey, TValue>(root);
		IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() => GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}