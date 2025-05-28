using System.Collections;
using System.Collections.Generic;

namespace HereticalSolutions.Collections
{
	public struct BPlusTreeMapValueEnumerator<TKey, TValue> : IEnumerator<TValue>
	{
		private readonly BPlusTreeMapNode<TKey, TValue> root;
		private BPlusTreeMapNode<TKey, TValue> currentNode;
		private int currentIndex;
		private bool started;

		public BPlusTreeMapValueEnumerator(BPlusTreeMapNode<TKey, TValue> root)
		{
			this.root = root;

			currentNode = null;

			currentIndex = -1;

			started = false;
		}

		public TValue Current => currentNode.Values[currentIndex];
		object IEnumerator.Current => Current;

		public void Dispose() { }

		public bool MoveNext()
		{
			if (!started)
			{
				// First call to MoveNext - find the leftmost leaf node
				started = true;

				if (root == null)
					return false;

				currentNode = root;

				// Traverse down to the first leaf node
				while (!currentNode.IsLeaf)
				{
					currentNode = currentNode.Children[0];
				}

				currentIndex = -1;
			}

			// Try to move to the next value in the current node
			currentIndex++;

			// If we've processed all values in this node
			if (currentIndex >= currentNode.KeysCount)
			{
				// Try to move to the next leaf node
				currentNode = currentNode.Next;
				currentIndex = 0;

				// If no more nodes, we're done
				if (currentNode == null)
					return false;
			}

			return true;
		}

		public void Reset()
		{
			currentNode = null;

			currentIndex = -1;
			
			started = false;
		}
	}
}