using System.Collections;
using System.Collections.Generic;

namespace HereticalSolutions.Collections
{
	public struct BPlusTreeMapKeyEnumerable<TKey, TValue> : IEnumerable<TKey>
	{
		private readonly BPlusTreeMapNode<TKey, TValue> root;

		public BPlusTreeMapKeyEnumerable(BPlusTreeMapNode<TKey, TValue> root)
		{
			this.root = root;
		}

		public BPlusTreeMapKeyEnumerator<TKey, TValue> GetEnumerator() => new BPlusTreeMapKeyEnumerator<TKey, TValue>(root);
		IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() => GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}