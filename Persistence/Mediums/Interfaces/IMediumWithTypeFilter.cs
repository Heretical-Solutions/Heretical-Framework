using System;

namespace HereticalSolutions.Persistence
{
	public interface IMediumWithTypeFilter
	{
		bool AllowsType<TValue>();

		bool AllowsType(
			Type valueType);
	}
}