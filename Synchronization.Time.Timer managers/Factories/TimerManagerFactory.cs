using System;

using HereticalSolutions.Allocations;

using HereticalSolutions.Delegates.NonAlloc.Factories;

using HereticalSolutions.ObjectPools.Configurable.Factories;

using HereticalSolutions.Synchronization.Time.Timers.FloatDelta;
using HereticalSolutions.Synchronization.Time.Timers.FloatDelta.Factories;
using HereticalSolutions.Synchronization.Time.TimeUpdaters;
using HereticalSolutions.Synchronization.Time.TimeUpdaters.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Synchronization.Time.TimerManagers.Factories
{
	public class TimerManagerFactory
	{
		private const string TIME_UPDATER_POSTFIX = "TimeUpdater";

		private readonly TimeUpdaterFactory timeUpdaterFactory;

		private readonly FloatTimerFactory floatTimerFactory;

		private readonly NonAllocSubscriptionFactory nonAllocSubscriptionFactory;

		private readonly ConfigurableStackPoolFactory
			stackPoolFactory;

		private readonly ILoggerResolver loggerResolver;

		#region Timer subscriptions pool

		private const int
			DEFAULT_TIMER_INITIAL_ALLOCATION_AMOUNT = 128;

		private const int
			DEFAULT_TIMER_ADDITIONAL_ALLOCATION_AMOUNT = 128;

		protected AllocationCommandDescriptor
			defaultTimerPoolInitialAllocationDescriptor =
				new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = DEFAULT_TIMER_INITIAL_ALLOCATION_AMOUNT
				};

		protected AllocationCommandDescriptor
			defaultTimerPoolAdditionalAllocationDescriptor =
				new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = DEFAULT_TIMER_ADDITIONAL_ALLOCATION_AMOUNT
				};

		#endregion

		public TimerManagerFactory(
			TimeUpdaterFactory timeUpdaterFactory,
			FloatTimerFactory floatTimerFactory,
			NonAllocSubscriptionFactory nonAllocSubscriptionFactory,	
			ConfigurableStackPoolFactory stackPoolFactory,
			ILoggerResolver loggerResolver)
		{
			this.timeUpdaterFactory = timeUpdaterFactory;

			this.floatTimerFactory = floatTimerFactory;

			this.nonAllocSubscriptionFactory = nonAllocSubscriptionFactory;

			this.stackPoolFactory = stackPoolFactory;

			this.loggerResolver = loggerResolver;

			defaultTimerPoolInitialAllocationDescriptor =
				new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = DEFAULT_TIMER_INITIAL_ALLOCATION_AMOUNT
				};

			defaultTimerPoolAdditionalAllocationDescriptor =
				new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = DEFAULT_TIMER_ADDITIONAL_ALLOCATION_AMOUNT
				};
		}

		public TimerManagerFactory(
			TimeUpdaterFactory timerUpdaterFactory,
			FloatTimerFactory floatTimerFactory,
			NonAllocSubscriptionFactory nonAllocSubscriptionFactory,
			ConfigurableStackPoolFactory stackPoolFactory,

			AllocationCommandDescriptor
				defaultTimerPoolInitialAllocationDescriptor,
			AllocationCommandDescriptor
				defaultTimerPoolAdditionalAllocationDescriptor,

			ILoggerResolver loggerResolver)
		{
			this.timeUpdaterFactory = timerUpdaterFactory;

			this.floatTimerFactory = floatTimerFactory;

			this.nonAllocSubscriptionFactory = nonAllocSubscriptionFactory;

			this.stackPoolFactory = stackPoolFactory;

			this.defaultTimerPoolInitialAllocationDescriptor =
				defaultTimerPoolInitialAllocationDescriptor;

			this.defaultTimerPoolAdditionalAllocationDescriptor =
				defaultTimerPoolAdditionalAllocationDescriptor;


			this.loggerResolver = loggerResolver;
		}

		public TimerManager BuildTimerManager(
			string timerManagerID)
		{
			var timeUpdater = timeUpdaterFactory
				.BuildTimeUpdater($"{timerManagerID} {TIME_UPDATER_POSTFIX}");

			return BuildTimerManager(
				timerManagerID,
				timeUpdater);
		}

		public TimerManager BuildTimerManager(
			string timerManagerID,
			ITimeUpdater timeUpdater)
		{
			ILogger logger = loggerResolver
				.GetLogger<TimerManagerFactory>();

			Func<IFloatTimer> timerAllocationDelegate = floatTimerFactory.BuildFloatTimer;

			var timerPool = stackPoolFactory
				.BuildConfigurableStackPool<IFloatTimer>(
					new AllocationCommand<IFloatTimer>(
						defaultTimerPoolInitialAllocationDescriptor,
						timerAllocationDelegate,
						null),
					new AllocationCommand<IFloatTimer>(
						defaultTimerPoolAdditionalAllocationDescriptor,
						timerAllocationDelegate,
						null));

			Func<AllocatedTimerContext> contextAllocationDelegate =
				() => new AllocatedTimerContext(
					nonAllocSubscriptionFactory);

			var contextPool = stackPoolFactory.
				BuildConfigurableStackPool<AllocatedTimerContext>(
					new AllocationCommand<AllocatedTimerContext>(
						defaultTimerPoolInitialAllocationDescriptor,
						contextAllocationDelegate,
						null),
					new AllocationCommand<AllocatedTimerContext>(
						defaultTimerPoolAdditionalAllocationDescriptor,
						contextAllocationDelegate,
						null));

			return new TimerManager(
				timerManagerID,
				timeUpdater,
				timerPool,
				contextPool,
				logger);
		}
	}
}