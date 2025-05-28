using System.Collections.Generic;

namespace HereticalSolutions.Bags
{
	public interface IBag<T>
	{
		bool Push(
			T instance);

		bool Pop(
			T instance);

		bool Peek(
			out T instance);

		int Count { get; }

		LinkedListEnumerable<T> All { get; }
		//IEnumerable<T> All { get; }

		void Clear();
	}
}