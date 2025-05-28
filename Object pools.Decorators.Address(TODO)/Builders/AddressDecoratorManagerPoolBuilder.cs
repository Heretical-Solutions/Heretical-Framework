using System;

using HereticalSolutions.ObjectPools.Managed.Builders;
using HereticalSolutions.ObjectPools.Decorators.Address.Factories;

using HereticalSolutions.UUID.Generation;

namespace HereticalSolutions.ObjectPools.Decorators.Address.Builders
{
	public static class AddressDecoratorManagedPoolBuilder
	{
		public static ManagedPoolBuilder<T> DecoratedWithAddresses<
			TUUID,
			T,
			TParentContext,
			TChildContext>(
				this ManagedPoolBuilder<T> builder,

				TParentContext parentContext,
				Func<TParentContext, int> getChildrenCount,
				Func<TParentContext, int, TChildContext> getChildContext,
				Func<TChildContext, string> getAddress,
				Func<TChildContext, ManagedPoolBuilder<T>> getChildPoolBuilder,

				AddressDecoratorPoolFactory addressDecoratorPoolFactory,
				AddressDecoratorAllocationCallbackFactory
					addressDecoratorAllocationCallbackFactory,
				AddressDecoratorMetadataFactory
					addressDecoratorMetadataFactory,
				IUUIDAllocationController<TUUID> uuidAllocationController)
		{
			var context = builder.Context;

			context.BuildDependenciesStep = false;

			context.ConcretePoolBuildStep =
				(delegateContext) =>
				{
					context
						.MetadataDescriptorBuilders
						.Add(
							addressDecoratorMetadataFactory
								.BuildAddressMetadataDescriptor<TUUID>);

					var decoratorPool = addressDecoratorPoolFactory.
						BuildAddressDecoratorManagedPool<TUUID, T>(
							uuidAllocationController);

					int childrenCount = getChildrenCount(parentContext);

					for (int i = 0; i < childrenCount; i++)
					{
						var childContext = getChildContext(
							parentContext,
							i);

						var address = getAddress(
							childContext);

						// Build a set address callback
						SetAddressCallback<TUUID, T> setAddressCallback =
							addressDecoratorAllocationCallbackFactory.
								BuildSetAddressCallback<TUUID, T>(
									address);

						context.FacadeAllocationCallbacks.Add(
							setAddressCallback);

						var childPoolBuilder = getChildPoolBuilder(
							childContext);

						foreach (var valueAllocationCallback
							in context.ValueAllocationCallbacks)
						{
							childPoolBuilder.Context.
								ValueAllocationCallbacks.Add(
									valueAllocationCallback);
						}

						foreach (var facadeAllocationCallback
							in context.FacadeAllocationCallbacks)
						{
							childPoolBuilder.Context.
								FacadeAllocationCallbacks.Add(
									facadeAllocationCallback);
						}

						foreach (var metadataDescriptorBuilder
							in context.MetadataDescriptorBuilders)
						{
							childPoolBuilder.Context.
								MetadataDescriptorBuilders.Add(
									metadataDescriptorBuilder);
						}

						var childPool = childPoolBuilder.Build();

						decoratorPool.AddPool(
							address,
							childPool);
					}

					delegateContext.CurrentPool = decoratorPool;
				};

			return builder;
		}
	}
}