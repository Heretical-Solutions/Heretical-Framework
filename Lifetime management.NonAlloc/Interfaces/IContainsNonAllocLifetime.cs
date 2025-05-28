namespace HereticalSolutions.LifetimeManagement.NonAlloc
{
	public interface IContainsNonAllocLifetime
	{
		INonAllocLifetimeable Lifetime { get; set; }
	}
}