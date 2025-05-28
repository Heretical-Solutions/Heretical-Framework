using HereticalSolutions.Synchronization.Time.TimeUpdaters;

namespace HereticalSolutions.Synchronization.Unity
{
	public interface ISynchronizationManager
	{
		ITickUpdater FixedUpdateTick { get; }
		ITimeUpdater FixedUpdateTimer { get; }

		ITickUpdater UpdateTick { get; }
		ITimeUpdater UpdateTimer { get; }

		ITickUpdater LateUpdateTick { get; }
		ITimeUpdater LateUpdateTimer { get; }

	}
}