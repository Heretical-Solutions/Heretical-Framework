using System.Collections;
using System.Collections.Generic;

namespace HereticalSolutions.Collections.NonAlloc
{
	public struct NonAllocBPlusTreeMapValueEnumerable<TKey, TValue> : IEnumerable<TValue>
	{
		private readonly NonAllocBPlusTreeMapNode<TKey, TValue> root;

		public NonAllocBPlusTreeMapValueEnumerable(NonAllocBPlusTreeMapNode<TKey, TValue> root)
		{
			this.root = root;
		}

		public NonAllocBPlusTreeMapValueEnumerator<TKey, TValue> GetEnumerator() => new NonAllocBPlusTreeMapValueEnumerator<TKey, TValue>(root);

		IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}