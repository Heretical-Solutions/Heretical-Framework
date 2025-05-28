using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Async;

namespace HereticalSolutions.ObjectPools.Async.Builders
{
	public delegate Task BuildStepAsync<T>(
		AsyncPoolBuilderContext<T> context,

		//Async tail
		AsyncExecutionContext asyncContext);

	public class AsyncPoolBuilderContext<T>
	{
		#region Allocation descriptors

		public AllocationCommandDescriptor InitialAllocation;

		public AllocationCommandDescriptor AdditionalAllocation;

		#endregion

		#region Value allocation command

		public Func<Task<T>> ValueAllocationDelegate;

		public List<IAsyncAllocationCallback<T>> ValueAllocationCallbacks;

		public IAsyncAllocationCallback<T> ResultValueAllocationCallback;


		public IAsyncAllocationCommand<T> InitialValueAllocationCommand;

		public IAsyncAllocationCommand<T> AdditionalValueAllocationCommand;

		#endregion

		#region Building

		public List<BuildStepAsync<T>> InitialBuildSteps;

		public BuildStepAsync<T> ConcretePoolBuildStep;

		public List<BuildStepAsync<T>> FinalBuildSteps;

		#endregion

		public IAsyncPool<T> CurrentPool;
	}
}