using System;

namespace HereticalSolutions.ProceduralEnums
{
	[AttributeUsage(
		AttributeTargets.Field,
		AllowMultiple = false,
		Inherited = false)] //true)]
	public abstract class AProcEnumAttribute : Attribute
	{
		public int Order { get; private set; } = 0;

		protected AProcEnumAttribute()
		{
		}

		protected AProcEnumAttribute(
			int order)
		{
			Order = order;
		}
	}
}