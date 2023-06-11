using System;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.AllocationCallbacks;
using HereticalSolutions.Pools.Allocations;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Services.Settings;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Services.Factories
{
    public static partial class ServicesFactory
    {
        public static INonAllocDecoratedPool<GameObject> BuildGameObjectPool(
            DiContainer container,
            GameObjectPoolSettings settings,
            Transform parentTransform = null)
        {
            #region Builders

            var poolWithAddressBuilder = new PoolWithAddressBuilder<GameObject>();
            
            var poolWithVariantsBuilder = new PoolWithVariantsBuilder<GameObject>();

            var resizablePoolBuilder = new ResizablePoolBuilder<GameObject>();
            
            #endregion

            #region Callbacks
            
            PushToDecoratedPoolCallback<GameObject> pushCallback =
                PoolsFactory.BuildPushToDecoratedPoolCallback<GameObject>(
                    PoolsFactory.BuildDeferredCallbackQueue<GameObject>());
            
            #endregion
            
            #region Metadata descriptor builders

            var metadataDescriptorBuilders = new Func<MetadataAllocationDescriptor>[]
            {
                PoolsFactory.BuildIndexedMetadataDescriptor,
                AddressDecoratorsPoolsFactory.BuildAddressMetadataDescriptor,
                VariantsDecoratorsPoolsFactory.BuildVariantMetadataDescriptor
            };

            #endregion

            poolWithAddressBuilder.Initialize();
            
            foreach (var element in settings.Elements)
            {
                #region Address
                
                string fullAddress = element.GameObjectAddress;

                int[] addressHashes = fullAddress.AddressToHashes();

                SetAddressCallback<GameObject> setAddressCallback = AddressDecoratorsPoolsFactory.BuildSetAddressCallback<GameObject>(
                    fullAddress,
                    addressHashes);

                #endregion
                
                poolWithVariantsBuilder.Initialize();

                for (int i = 0; i < element.Variants.Length; i++)
                {
                    #region Variant
                    
                    var currentVariant = element.Variants[i];
                    
                    string currentVariantName = $"{fullAddress} (Variant {i.ToString()})";

                    SetVariantCallback<GameObject> setVariantCallback =
                        VariantsDecoratorsPoolsFactory.BuildSetVariantCallback<GameObject>(i);

                    RenameByStringAndIndexCallback renameCallback =
                        UnityDecoratorsPoolsFactory.BuildRenameByStringAndIndexCallback(currentVariantName);

                    #endregion

                    #region Allocation callbacks initialization

                    var callbacks = new IAllocationCallback<GameObject>[]
                    {
                        renameCallback,
                        setAddressCallback,
                        setVariantCallback,
                        pushCallback
                    };

                    #endregion

                    #region Value allocation delegate initialization

                    var prefab = currentVariant.Prefab;

                    Func<GameObject> valueAllocationDelegate =
                        () => UnityZenjectAllocationsFactory.DIResolveOrInstantiateAllocationDelegate(
                            container,
                            prefab);

                    #endregion

                    //TODO: replace with SupplyAndMerge pool
                    resizablePoolBuilder.Initialize(
                        valueAllocationDelegate,
                        metadataDescriptorBuilders,
                        currentVariant.Initial,
                        currentVariant.Additional,
                        callbacks);

                    var resizablePool = resizablePoolBuilder.Build();

                    var gameObjectPool = UnityDecoratorsPoolsFactory.BuildNonAllocGameObjectPool(
                        resizablePool,
                        parentTransform);

                    var prefabInstancePool = UnityDecoratorsPoolsFactory.BuildNonAllocPrefabInstancePool(
                        gameObjectPool,
                        prefab);

                    poolWithVariantsBuilder.AddVariant(
                        i,
                        currentVariant.Chance,
                        prefabInstancePool);
                }

                var variantPool = poolWithVariantsBuilder.Build();

                poolWithAddressBuilder.Parse(
                    fullAddress,
                    variantPool);
            }

            var poolWithAddress = poolWithAddressBuilder.Build();

            var poolWithID = IDDecoratorsPoolsFactory.BuildNonAllocPoolWithID(
                poolWithAddress,
                settings.PoolID);

            pushCallback.Root = poolWithID;

            return poolWithID;
        }
    }
}