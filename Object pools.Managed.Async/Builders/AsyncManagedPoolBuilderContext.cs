using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Async;

using HereticalSolutions.Metadata.Allocations;

namespace HereticalSolutions.ObjectPools.Managed.Async.Builders
{
	public delegate Task BuildStepAsync<T>(
		AsyncManagedPoolBuilderContext<T> context,

		//Async tail
		AsyncExecutionContext asyncContext);

	public class AsyncManagedPoolBuilderContext<T>
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

		#region Facade allocation command

		public List<IAsyncAllocationCallback<IAsyncPoolElementFacade<T>>> 
			FacadeAllocationCallbacks;

		public IAsyncAllocationCallback<IAsyncPoolElementFacade<T>> 
			ResultFacadeAllocationCallback;

		#endregion

		#region Metadata allocation descriptors

		public List<Func<MetadataAllocationDescriptor>> MetadataDescriptorBuilders;

		public MetadataAllocationDescriptor[] MetadataDescriptors;

		#endregion

		#region Building

		public List<BuildStepAsync<T>> InitialBuildSteps;

		public BuildStepAsync<T> ConcretePoolBuildStep;

		public List<BuildStepAsync<T>> FinalBuildSteps;

		#endregion

		public IAsyncManagedPool<T> CurrentPool;
	}
}