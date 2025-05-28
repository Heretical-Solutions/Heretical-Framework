using System;

using System.Collections.Generic;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Bags
{
	public class LinkedListBag<T>
		: IBag<T>,
		  ICleanuppable,
		  IDisposable
	{
		private readonly LinkedList<T> bag;

		public LinkedListBag(
			LinkedList<T> bag)
		{
			this.bag = bag;
		}

		#region IBag

		public bool Push(
			T instance)
		{
			bag.AddLast(instance);

			return true;
		}

		public bool Pop(
			T instance)
		{
			return bag.Remove(instance);
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
				//Non-allocating version
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