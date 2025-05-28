namespace HereticalSolutions.Builders
{
	public abstract class ABuilder<TContext>
	{
		protected TContext context;

		public TContext Context => context;

		public void Cleanup()
		{
			context = default;
		}
	}
}