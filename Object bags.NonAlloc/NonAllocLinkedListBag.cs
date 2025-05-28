using System;

using System.Collections.Generic;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.ObjectPools;

namespace HereticalSolutions.Bags.NonAlloc
{
	public class NonAllocLinkedListBag<T>
		: IBag<T>,
		  ICleanuppable,
		  IDisposable
	{
		private readonly LinkedList<T> bag;

		private readonly IPool<LinkedListNode<T>> nodePool;

		public NonAllocLinkedListBag(
			LinkedList<T> bag,
			IPool<LinkedListNode<T>> nodePool)
		{
			this.bag = bag;

			this.nodePool = nodePool;
		}

		#region IBag

		public bool Push(
			T instance)
		{
			var node = nodePool.Pop();

			node.Value = instance;

			bag.AddLast(node);

			return true;
		}

		public bool Pop(
			T instance)
		{
			var node = bag.Find(instance);

			if (node == null)
				return false;

			bag.Remove(node);

			node.Value = default(T);

			nodePool.Push(node);

			return true;
		}

		public bool Peek(
			out T instance)
		{
			if (bag.Count > 0)
			{
				instance = bag.First.Value;

				return true;
			}

			instance = default(T);

			return false;
		}

		public int Count { get => bag.Count; }

		public LinkedListEnumerable<T> All //IEnumerable<T> All
		{
			get
			{
				// Non-allocating version. WARNING! Casting struct to an interface (IEnumerable) still provides allocations (~48B), but that's the best can do without specifying concrete type of enumerable
				return new LinkedListEnumerable<T>(
					bag.First);

				//Allocating version
				/*
				var current = bag.First;

				while (current != null)
				{
					yield return current.Value;

					current = current.Next;
				}
				*/
			}
		}

		public void Clear()
		{
			while (bag.Count > 0)
			{
				Pop(bag.First.Value);
			}

			bag.Clear();
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			foreach (var item in bag)
				if (item is ICleanuppable)
					(item as ICleanuppable).Cleanup();

			bag.Clear();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			foreach (var item in bag)
				if (item is IDisposable)
					(item as IDisposable).Dispose();

			bag.Clear();
		}

		#endregion
	}
}