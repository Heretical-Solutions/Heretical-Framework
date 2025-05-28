using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.Metadata.Allocations;

namespace HereticalSolutions.ObjectPools.Managed.Builders
{
	public class ManagedPoolBuilderContext<T>
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

		#region Facade allocation command

		public List<IAllocationCallback<IPoolElementFacade<T>>> FacadeAllocationCallbacks;

		public IAllocationCallback<IPoolElementFacade<T>> ResultFacadeAllocationCallback;

		#endregion

		#region Metadata allocation descriptors

		public List<Func<MetadataAllocationDescriptor>> MetadataDescriptorBuilders;

		public MetadataAllocationDescriptor[] MetadataDescriptors;

		#endregion

		#region Building

		public bool BuildDependenciesStep = true;

		public List<Action<ManagedPoolBuilderContext<T>>> InitialBuildSteps;

		public Action<ManagedPoolBuilderContext<T>> ConcretePoolBuildStep;

		public List<Action<ManagedPoolBuilderContext<T>>> FinalBuildSteps;

		#endregion

		public IManagedPool<T> CurrentPool;
	}
}