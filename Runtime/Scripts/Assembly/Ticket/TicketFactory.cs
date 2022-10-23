using System;
using System.Collections.Generic;
using HereticalSolutions.Repositories;

namespace HereticalSolutions.Assembly.Factories
{
	public static class TicketFactory
	{
		public static AssemblyTicket<TValue> BuildTicket<TValue>(
			IShop<TValue>[] assemblyLine)
		{
			return new AssemblyTicket<TValue>(
				new DictionaryRepository<string, object>(
					new Dictionary<string, object>()),
				assemblyLine);
		}

		public static AssemblyTicket<TValue> BuildNextShopTicket<TValue>(
			AssemblyTicket<TValue> currentShopTicket)
		{
			var result = new AssemblyTicket<TValue>(
				new DictionaryRepository<string, object>(
					new Dictionary<string, object>()),
				currentShopTicket.AssemblyLine);

			foreach (var key in currentShopTicket.Arguments.Keys)
				result.Arguments.Add(
					key,
					currentShopTicket.Arguments.Get(key));

			return result;
		}
	}
}