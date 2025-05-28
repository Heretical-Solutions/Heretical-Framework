using HereticalSolutions.Collections;

using HereticalSolutions.Metadata;

namespace HereticalSolutions.ObjectPools.Managed.Async
{
	public class AsyncPoolElementFacadeWithArrayIndex<T>
		: AsyncPoolElementFacade<T>,
		  IIndexed
	{
		public AsyncPoolElementFacadeWithArrayIndex(
			IStronglyTypedMetadata metadata)
			: base(metadata)
		{
		}

		#region IIndexed

		public int Index { get; set; }

		#endregion

		#region ICleanUppable

		public override void Cleanup()
		{
			Index = -1;

			base.Cleanup();
		}

		#endregion

		#region IDisposable

		public override void Dispose()
		{
			base.Cleanup();
		}

		#endregion
	}
}