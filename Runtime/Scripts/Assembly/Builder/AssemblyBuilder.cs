using System.Collections.Generic;

namespace HereticalSolutions.Assembly.Factories
{
	public class AssemblyBuilder<T>
	{
		private List<IShop<T>> assemblyLine = new List<IShop<T>>();

		public AssemblyBuilder<T> Add(IShop<T> newShop)
		{
			assemblyLine.Add(newShop);

			return this;
		}

		public AssemblyBuilder<T> Add(
			IShop<T> newShop,
			out IShop<T> shop)
		{
			assemblyLine.Add(newShop);

			shop = newShop;

			return this;
		}

		public IShop<T>[] Build()
		{
			return assemblyLine.ToArray();
		}
	}
}