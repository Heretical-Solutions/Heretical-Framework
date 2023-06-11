using System;
using System.Threading.Tasks;

using HereticalSolutions.MVVM;

using HereticalSolutions.ResourceManagement.Factories;

using UnityEngine;

namespace HereticalSolutions.ResourceManagement.Importers
{
    /// <summary>
    /// Imports resources from Addressables.
    /// </summary>
    public class ResourceImporterFromAddressables
    {
        /// <summary>
        /// Loads resources from Addressables.
        /// </summary>
        /// <param name="runtimeResourceManager">The runtime resource manager.</param>
        /// <param name="settings">The addressable resources settings.</param>
        /// <param name="currentResourceProgress">Optional: Progress tracker for the current resource.</param>
        /// <param name="currentResource">Optional: Observable property for the current resource.</param>
        /// <param name="totalProgress">Optional: Progress tracker for the overall loading progress.</param>
        /// <returns>The task representing the loading process.</returns>
        public async Task Load(
            IRuntimeResourceManager runtimeResourceManager,
            AddressableResourcesSettings settings,
            IProgress<float> currentResourceProgress = null,
            IObservableProperty<string> currentResource = null,
            IProgress<float> totalProgress = null)
        {
            // Count the total number of resources to be loaded
            int totalResources = 0;
            foreach (var resourceDataSettings in settings.Resources)
            {
                totalResources += resourceDataSettings.Variants.Length;
            }

            int resourcesLoaded = 0;
            currentResourceProgress?.Report(0f);

            if (currentResource != null)
                currentResource.Value = string.Empty;

            totalProgress?.Report(0f);

            await Task.Yield();

            foreach (var resourceDataSettings in settings.Resources)
            {
                IResourceData resourceData = null;

                if (runtimeResourceManager.HasResource(resourceDataSettings.ResourceID))
                {
                    resourceData = (IResourceData)runtimeResourceManager.GetResource(resourceDataSettings.ResourceID);
                }
                else
                {
                    resourceData = RuntimeResourceManagerFactory.BuildResourceData(
                        new ResourceDescriptor()
                        {
                            ID = resourceDataSettings.ResourceID,
                            IDHash = resourceDataSettings.ResourceID.AddressToHash()
                        });

                    runtimeResourceManager.AddResource((IReadOnlyResourceData)resourceData);
                }

                foreach (var resourceVariantDataSettings in resourceDataSettings.Variants)
                {
                    var resourceAssetReference = resourceVariantDataSettings.AssetReference;

                    if (currentResource != null)
                        currentResource.Value = resourceVariantDataSettings.VariantID;

                    if (!resourceAssetReference.RuntimeKeyIsValid())
                    {
                        // Log an error if the runtime key is invalid for the asset
                        Debug.LogError($"[ResourceImporterFromAddressables] RUNTIME KEY IS INVALID FOR ASSET {resourceDataSettings.ResourceID} VARIANT {resourceVariantDataSettings.VariantID}");
                        
                        continue;
                    }

                    var variantData = RuntimeResourceManagerFactory.BuildResourceVariantData(
                        new ResourceVariantDescriptor()
                        {
                            VariantID = resourceVariantDataSettings.VariantID,
                            VariantIDHash = resourceVariantDataSettings.VariantID.AddressToHash(),
                            Priority = resourceVariantDataSettings.Priority,
                            Source = EResourceSources.CLOUD_STORAGE,
                            ResourceType = typeof(UnityEngine.Object) // TODO: Find a better way
                        },
                        RuntimeResourceManagerFactoryUnity.BuildAddressableResourceStorageHandle(
                            resourceVariantDataSettings.AssetReference));

                    await resourceData.AddVariant(
                        variantData,
                        currentResourceProgress);

                    resourcesLoaded++;

                    totalProgress?.Report((float)resourcesLoaded / (float)totalResources);

                    await Task.Yield();
                }
            }

            if (currentResource != null)
                currentResource.Value = string.Empty;

            totalProgress?.Report(1f);
        }
        
        /// <summary>
        /// Unloads resources from the runtime resource manager.
        /// </summary>
        /// <param name="runtimeResourceManager">The runtime resource manager.</param>
        /// <param name="settings">The addressable resources settings.</param>
        /// <param name="currentResourceProgress">Optional: Progress tracker for the current resource.</param>
        /// <param name="currentResource">Optional: Observable property for the current resource.</param>
        /// <param name="totalProgress">Optional: Progress tracker for the overall unloading progress.</param>
        /// <returns>The task representing the unloading process.</returns>
        public async Task Unload(
            IRuntimeResourceManager runtimeResourceManager,
            AddressableResourcesSettings settings,
            IProgress<float> currentResourceProgress = null,
            IObservableProperty<string> currentResource = null,
            IProgress<float> totalProgress = null)
        {
            // Count the total number of resources to be unloaded
            int totalResources = 0;
            foreach (var resourceDataSettings in settings.Resources)
            {
                totalResources += resourceDataSettings.Variants.Length;
            }

            int resourcesUnloaded = 0;
            currentResourceProgress?.Report(0f);

            if (currentResource != null)
                currentResource.Value = string.Empty;

            totalProgress?.Report(0f);

            await Task.Yield();

            foreach (var resourceDataSettings in settings.Resources)
            {
                IResourceData resourceData = null;

                if (runtimeResourceManager.HasResource(resourceDataSettings.ResourceID))
                {
                    resourceData = (IResourceData)runtimeResourceManager.GetResource(resourceDataSettings.ResourceID);
                }
                else
                {
                    // Skip unloading if the resource is not found in the runtime resource manager
                    continue;
                }

                foreach (var resourceVariantDataSettings in resourceDataSettings.Variants)
                {
                    var resourceAssetReference = resourceVariantDataSettings.AssetReference;

                    if (currentResource != null)
                        currentResource.Value = resourceVariantDataSettings.VariantID;

                    if (!resourceAssetReference.RuntimeKeyIsValid())
                    {
                        // Log an error if the runtime key is invalid for the asset
                        Debug.LogError($"[ResourceImporterFromAddressables] RUNTIME KEY IS INVALID FOR ASSET {resourceDataSettings.ResourceID} VARIANT {resourceVariantDataSettings.VariantID}");

                        continue;
                    }

                    await resourceData.RemoveVariant(
                        resourceVariantDataSettings.VariantID.AddressToHash(),
                        currentResourceProgress);

                    resourcesUnloaded++;

                    totalProgress?.Report((float)resourcesUnloaded / (float)totalResources);

                    await Task.Yield();
                }
                
                if (resourceData.DefaultVariant == null)
                    runtimeResourceManager.RemoveResource(resourceDataSettings.ResourceID);
            }

            if (currentResource != null)
                currentResource.Value = string.Empty;

            totalProgress?.Report(1f);
        }
    }
}