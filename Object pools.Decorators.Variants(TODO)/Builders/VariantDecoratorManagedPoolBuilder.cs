using System;

using HereticalSolutions.ObjectPools.Managed.Builders;
using HereticalSolutions.ObjectPools.Decorators.Variants.Factories;

namespace HereticalSolutions.ObjectPools.Decorators.Variants.Builders
{
	public static class VariantDecoratorManagedPoolBuilder
	{
		public static ManagedPoolBuilder<T> DecoratedWithVariants<
			T,
			TParentContext,
			TChildContext>(
				this ManagedPoolBuilder<T> builder,
				
				TParentContext parentContext,
				Func<TParentContext, int> getChildrenCount,
				Func<TParentContext, int, TChildContext> getChildContext,
				Func<TChildContext, float> getChance,
				Func<TChildContext, ManagedPoolBuilder<T>> getChildPoolBuilder,

				VariantDecoratorPoolFactory variantDecoratorPoolFactory,
				VariantDecoratorAllocationCallbackFactory 
					variantDecoratorAllocationCallbackFactory,
				VariantDecoratorMetadataFactory
					variantDecoratorMetadataFactory)
		{
			var context = builder.Context;

			context.BuildDependenciesStep = false;

			context.ConcretePoolBuildStep =
				(delegateContext) =>
				{
					context
						.MetadataDescriptorBuilders
						.Add(
							variantDecoratorMetadataFactory
								.BuildVariantMetadataDescriptor);

					var decoratorPool = variantDecoratorPoolFactory.
						BuildVariantDecoratorManagedPool<T>();

					int childrenCount = getChildrenCount(parentContext);
	
					for (int i = 0; i < childrenCount; i++)
					{
						var childContext = getChildContext(
							parentContext,
							i);

						var chance = getChance(
							childContext);
						
						// Build a set variant callback
						SetVariantCallback<T> setVariantCallback =
							variantDecoratorAllocationCallbackFactory.
								BuildSetVariantCallback<T>(i);
	
						context.FacadeAllocationCallbacks.Add(
							setVariantCallback);
	
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

						// Add the variant to the pool with variants builder
						decoratorPool.AddVariant(
							i,
							chance,
							childPool);
					}
	
					delegateContext.CurrentPool = decoratorPool;
				};

			return builder;
		}
	}
}