using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.ObjectPools.Async
{
	public interface IAsyncResizable
	{
		Task Resize(

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}