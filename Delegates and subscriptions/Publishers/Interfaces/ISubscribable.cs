using System.Collections.Generic;

namespace HereticalSolutions.Delegates
{
	public interface ISubscribable
	{
		IEnumerable<object> AllSubscriptions { get; }

		void UnsubscribeAll();
	}
}