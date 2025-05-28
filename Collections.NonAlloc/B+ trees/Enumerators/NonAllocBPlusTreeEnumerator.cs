using System.Collections;
using System.Collections.Generic;

namespace HereticalSolutions.Collections.NonAlloc
{
	public struct NonAllocBPlusTreeEnumerator<T> : IEnumerator<T>
	{
		private readonly NonAllocBPlusTreeNode<T> root;

		private NonAllocBPlusTreeNode<T> currentNode;

		private int currentKeyIndex;

		private bool started;

		public NonAllocBPlusTreeEnumerator(
			NonAllocBPlusTreeNode<T> root)
		{
			this.root = root;

			currentNode = null;

			currentKeyIndex = -1;

			started = false;
		}

		public T Current => currentNode.Keys[currentKeyIndex];

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

				currentKeyIndex = -1;
			}

			// Try to move to the next key in the current node
			currentKeyIndex++;

			// If we've processed all keys in this node
			if (currentKeyIndex >= currentNode.Keys.Length)
			{
				// Try to move to the next leaf node
				currentNode = currentNode.Next;

				currentKeyIndex = 0;

				// If no more nodes, we're done
				if (currentNode == null)
					return false;
			}

			return true;
		}

		public void Reset()
		{
			currentNode = null;

			currentKeyIndex = -1;

			started = false;
		}
	}
}