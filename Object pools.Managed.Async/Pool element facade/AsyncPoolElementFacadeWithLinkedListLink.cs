using HereticalSolutions.Collections;

using HereticalSolutions.Metadata;

namespace HereticalSolutions.ObjectPools.Managed.Async
{
	public class AsyncPoolElementFacadeWithLinkedListLink<T>
		: AsyncPoolElementFacade<T>,
		  ILinkedListLink<T>
	{
		public AsyncPoolElementFacadeWithLinkedListLink(
			IStronglyTypedMetadata metadata)
			: base(metadata)
		{
		}

		#region ILinkedList

		public ILinkedListLink<T> Previous { get; set; }

		public ILinkedListLink<T> Next { get; set; }

		#endregion

		#region ICleanUppable

		public override void Cleanup()
		{
			Previous = null;

			Next = null;

			base.Cleanup();
		}

		#endregion

		#region IDisposable

		public override void Dispose()
		{
			Previous = null;

			Next = null;

			base.Cleanup();
		}

		#endregion
	}
}