/*
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.ObjectPools;

namespace HereticalSolutions.AssetImport
{
	public interface IAssetImporterPool
	{
		Task<IPoolElementFacade<AAssetImporter>> PopImporter<TImporter>(

			//Async tail
			AsyncExecutionContext asyncContext)
			where TImporter : AAssetImporter;

		Task PushImporter(
			IPoolElementFacade<AAssetImporter> pooledImporter,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}
*/