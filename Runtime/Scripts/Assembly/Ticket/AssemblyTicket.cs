using HereticalSolutions.Repositories;

namespace HereticalSolutions.Assembly
{
	public class AssemblyTicket<TValue>
	{
		public IRepository<string, object> Arguments { get; private set; }

		public IShop<TValue>[] AssemblyLine { get; private set; }

		public AssemblyTicket(
			IRepository<string, object> arguments,
			IShop<TValue>[] assemblyLine)
		{
			Arguments = arguments;

			AssemblyLine = assemblyLine;
		}
	}
}