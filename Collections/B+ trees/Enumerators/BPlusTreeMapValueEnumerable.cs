using System.Collections;
using System.Collections.Generic;

namespace HereticalSolutions.Collections
{
	public struct BPlusTreeMapValueEnumerable<TKey, TValue> : IEnumerable<TValue>
	{
		private readonly BPlusTreeMapNode<TKey, TValue> root;

		public BPlusTreeMapValueEnumerable(BPlusTreeMapNode<TKey, TValue> root)
		{
			this.root = root;
		}

		public BPlusTreeMapValueEnumerator<TKey, TValue> GetEnumerator() => new BPlusTreeMapValueEnumerator<TKey, TValue>(root);

		IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}