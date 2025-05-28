using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations;

namespace HereticalSolutions.ObjectPools.Builders
{
	public class PoolBuilderContext<T>
	{
		#region Allocation descriptors

		public AllocationCommandDescriptor InitialAllocation;

		public AllocationCommandDescriptor AdditionalAllocation;

		#endregion

		#region Value allocation command

		public Func<T> ValueAllocationDelegate;

		public List<IAllocationCallback<T>> ValueAllocationCallbacks;

		public IAllocationCallback<T> ResultValueAllocationCallback;


		public IAllocationCommand<T> InitialValueAllocationCommand;

		public IAllocationCommand<T> AdditionalValueAllocationCommand;

		#endregion

		#region Building

		public List<Action<PoolBuilderContext<T>>> InitialBuildSteps;

		public Action<PoolBuilderContext<T>> ConcretePoolBuildStep;

		public List<Action<PoolBuilderContext<T>>> FinalBuildSteps;

		#endregion

		public IPool<T> CurrentPool;
	}
}