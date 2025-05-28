using HereticalSolutions.Delegates.NonAlloc;

namespace HereticalSolutions.LifetimeManagement.NonAlloc
{
	public interface INonAllocLifetimeable
	{
		#region Set up

		bool IsSetUp { get; }

		INonAllocSubscribable OnSetUp { get; }

		ESynchronizationFlags SetUpFlags { get; }

		#endregion

		#region Initialize

		bool IsInitialized { get; }

		INonAllocSubscribable OnInitialized { get; }

		ESynchronizationFlags InitializeFlags { get; }

		#endregion

		#region Cleanup

		INonAllocSubscribable OnCleanedUp { get; }

		#endregion

		#region Tear down

		INonAllocSubscribable OnTornDown { get; }

		#endregion
	}
}