namespace HereticalSolutions.Assembly
{
	public interface IShop<TValue>
	{
		TValue Assemble(AssemblyTicket<TValue> ticket, int level);
	}
}