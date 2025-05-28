using System.Threading.Tasks;

using System.IO;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Persistence
{
	public interface IMediumWithStream
		: IAsyncSerializationMedium,
		  IBlockSerializationMedium,
		  IAsyncBlockSerializationMedium,
		  IHasReadWriteControl
	{
		EStreamMode CurrentMode { get; }

		Stream Stream { get; }

		bool StreamOpen { get; }


		#region Flush

		bool FlushAutomatically { get; }

		void Flush();

		Task FlushAsync(

			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion


		#region Seek

		long Position { get; }

		bool CanSeek { get; }

		bool Seek(
			long offset,
			out long position);

		bool SeekFromStart(
			long offset,
			out long position);

		bool SeekFromFinish(
			long offset,
			out long position);

		#endregion
	}
}