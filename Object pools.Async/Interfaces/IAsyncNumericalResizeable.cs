using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.ObjectPools.Async
{
	public interface IAsyncNumericalResizable
	{
		Task Resize(
			int additionalAmount,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}