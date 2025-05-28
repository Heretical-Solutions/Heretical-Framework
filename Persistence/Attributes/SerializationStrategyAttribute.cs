namespace HereticalSolutions.Persistence
{
	[System.AttributeUsage(
		System.AttributeTargets.Class)]
	public class SerializationMediumAttribute : System.Attribute
	{
		public SerializationMediumAttribute()
		{
		}
	}
}