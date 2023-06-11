using System;
using System.Threading.Tasks;

using HereticalSolutions.MVVM;

using HereticalSolutions.ResourceManagement.Factories;

namespace HereticalSolutions.ResourceManagement.Importers
{
    public class ResourceImporterFromSettingsFile
    {
        public async Task Load(
            IRuntimeResourceManager runtimeResourceManager,
            ResourcesSettings settings,
            IProgress<float> currentResourceProgress = null,
            IObservableProperty<string> currentResource = null,
            IProgress<float> totalProgress = null)
        {
            int resourcesLoaded = 0;

            int totalResources = 0;

            foreach (var resourceDataSettings in settings.Resources)
                totalResources += resourceDataSettings.Variants.Length;

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
                    if (currentResource != null)
                        currentResource.Value = resourceVariantDataSettings.VariantID;

                    var variantData = RuntimeResourceManagerFactory.BuildResourceVariantData(
                        new ResourceVariantDescriptor()
                        {
                            VariantID = resourceVariantDataSettings.VariantID,
                            VariantIDHash = resourceVariantDataSettings.VariantID.AddressToHash(),
                            Priority = resourceVariantDataSettings.Priority,
                            Source = EResourceSources.SCRIPTABLE_SETTINGS_FILE,
                            ResourceType = resourceVariantDataSettings.Resource.GetType()
                        },
                        RuntimeResourceManagerFactoryUnity.BuildScriptableResourceStorageHandle(
                            resourceVariantDataSettings.Resource));

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
        
        public async Task Unload(
            IRuntimeResourceManager runtimeResourceManager,
            ResourcesSettings settings,
            IProgress<float> currentResourceProgress = null,
            IObservableProperty<string> currentResource = null,
            IProgress<float> totalProgress = null)
        {
            int resourcesUnloaded = 0;

            int totalResources = 0;

            foreach (var resourceDataSettings in settings.Resources)
                totalResources += resourceDataSettings.Variants.Length;

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
                    continue;
                }

                foreach (var resourceVariantDataSettings in resourceDataSettings.Variants)
                {
                    if (currentResource != null)
                        currentResource.Value = resourceVariantDataSettings.VariantID;

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