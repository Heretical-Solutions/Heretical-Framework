using System;
using System.Threading.Tasks;

namespace HereticalSolutions.Allocations.Async.Factories
{
	public static class AsyncAllocationFactory
	{
		public static async Task<T> AsyncNullAllocationDelegate<T>()
		{
			return default(T);
		}

		public static async Task<T> AsyncInstanceAllocationDelegate<T>(
			T instance)
		{
			return instance;
		}

		public static async Task<T> AsyncFuncAllocationDelegate<T>(
			Func<T> allocationDelegate)
		{
			return (allocationDelegate != null)
				? allocationDelegate.Invoke()
				: default(T);
		}

		public static async Task<TResult> AsyncFuncAllocationDelegate<TResult, TValue>(
			Func<TValue> allocationDelegate)
			where TValue : TResult
		{
			TValue result = (allocationDelegate != null)
				? allocationDelegate.Invoke()
				: default(TValue);

			return (TResult)result;
		}

		public static async Task<T> AsyncActivatorAllocationDelegate<T>()
		{
			return (T)Activator.CreateInstance(typeof(T));
		}

		public static async Task<TResult> AsyncActivatorAllocationDelegate<TResult, TValue>()
		{
			return (TResult)Activator.CreateInstance(typeof(TValue));
		}

		public static async Task<object> AsyncActivatorAllocationDelegate(
			Type valueType)
		{
			return Activator.CreateInstance(valueType);
		}

		public static async Task<T> AsyncActivatorAllocationDelegate<T>(
			object[] arguments)
		{
			return (T)Activator.CreateInstance(typeof(T), arguments);
		}

		public static async Task<TResult> AsyncActivatorAllocationDelegate<TResult, TValue>(
			object[] arguments)
		{
			return (TResult)Activator.CreateInstance(typeof(TValue), arguments);
		}

		public static async Task<object> AsyncActivatorAllocationDelegate(
			Type valueType,
			object[] arguments)
		{
			return Activator.CreateInstance(valueType, arguments);
		}
	}
}