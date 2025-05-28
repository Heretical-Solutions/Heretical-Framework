using System.Collections;
using System.Collections.Generic;

namespace HereticalSolutions.Collections
{
	public struct BPlusTreeEnumerable<T> : IEnumerable<T>
	{
		private readonly BPlusTreeNode<T> root;

		public BPlusTreeEnumerable(BPlusTreeNode<T> root)
		{
			this.root = root;
		}

		public BPlusTreeEnumerator<T> GetEnumerator() => new BPlusTreeEnumerator<T>(root);
		IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}