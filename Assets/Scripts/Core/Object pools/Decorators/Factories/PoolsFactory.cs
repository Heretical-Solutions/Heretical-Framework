namespace HereticalSolutions.Pools.Factories
{
	public static partial class PoolsFactory
	{
		#region Decorator pools

		public static DecoratorPool<T> BuildDecoratorPool<T>(IPool<T> innerPool)
		{
			return new DecoratorPool<T>(innerPool);
		}

		#endregion

		#region Non alloc decorator pools

		#endregion
	}
}