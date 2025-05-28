using System;

using System.Collections.Generic;
using System.Threading;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Bags.NonAlloc.Concurrent
{
	public class ConcurrentNonAllocLinkedListBag<T>
		: IBag<T>,
		  ICleanuppable,
		  IDisposable
	{
		private readonly NonAllocLinkedListBag<T> bag;

		private readonly SemaphoreSlim semaphoreSlim;

		public ConcurrentNonAllocLinkedListBag(
			NonAllocLinkedListBag<T> bag,
			SemaphoreSlim semaphoreSlim)
		{
			this.bag = bag;

			this.semaphoreSlim = semaphoreSlim;
		}

		#region IBag

		public bool Push(
			T instance)
		{
			semaphoreSlim.Wait();

			try
			{
				return bag.Push(instance);
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}

		public bool Pop(
			T instance)
		{
			semaphoreSlim.Wait();

			try
			{
				return bag.Pop(instance);
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}

		public bool Peek(
			out T instance)
		{
			instance = default(T);

			semaphoreSlim.Wait();

			try
			{
				if (!bag.Peek(out instance))
				{
					return false;
				}
			}
			finally
			{
				semaphoreSlim.Release();
			}

			return true;
		}

		public int Count
		{
			get
			{
				semaphoreSlim.Wait();

				try
				{
					return bag.Count;
				}
				finally
				{
					semaphoreSlim.Release();
				}
			}
		}

		public LinkedListEnumerable<T> All //IEnumerable<T> All
		{
			get
			{
				semaphoreSlim.Wait();

				try
				{
					return bag.All;
				}
				finally
				{
					semaphoreSlim.Release();
				}
			}
		}

		public void Clear()
		{
			semaphoreSlim.Wait();

			try
			{
				bag.Clear();
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			semaphoreSlim.Wait();

			try
			{
				bag.Clear();
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			semaphoreSlim.Wait();

			try
			{
				bag.Clear();
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}

		#endregion
	}
}