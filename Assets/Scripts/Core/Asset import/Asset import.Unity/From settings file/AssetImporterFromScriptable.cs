using System;
using System.Threading.Tasks;

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.AssetImport
{
    public class AssetImporterFromScriptable : AAssetImporter
    {
        private ResourcesSettings settings;

        public AssetImporterFromScriptable(
            IRuntimeResourceManager runtimeResourceManager,
            ILoggerResolver loggerResolver = null,
            ILogger logger = null)
            : base(
                runtimeResourceManager,
                loggerResolver,
                logger)
        {
        }

        public void Initialize(
            ResourcesSettings settings)
        {
            this.settings = settings;
        }

        public override async Task<IResourceVariantData> Import(
            IProgress<float> progress = null)
        {
            int resourcesLoaded = 0;

            int totalResources = 0;

            foreach (var resourceDataSettings in settings.Resources)
                totalResources += resourceDataSettings.Variants.Length;

            progress?.Report(0f);

            foreach (var resourceDataSettings in settings.Resources)
            {
                string resourceID = resourceDataSettings.ResourceID;

                foreach (var resourceVariantDataSettings in resourceDataSettings.Variants)
                {
                    string variantID = resourceVariantDataSettings.VariantID;

                    logger?.Log<AssetImporterFromScriptable>(
                        $"IMPORTING {resourceID} INITIATED");

                    IProgress<float> localProgress = progress.CreateLocalProgress(
                        0f,
                        1f,
                        resourcesLoaded,
                        totalResources);

                    var localResult = await AddAssetAsResourceVariant(
                        await GetOrCreateResourceData(
                            resourceID)
                            .ThrowExceptions<IResourceData, AssetImporterFromScriptable>(logger),
                        new ResourceVariantDescriptor()
                        {
                            VariantID = variantID,
                            VariantIDHash = variantID.AddressToHash(),
                            Priority = 0,
                            Source = EResourceSources.LOCAL_STORAGE,
                            Storage = EResourceStorages.RAM,
                            ResourceType = resourceVariantDataSettings.Resource.GetType(),
                        },
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
                        ResourceManagementFactory.BuildConcurrentPreallocatedResourceStorageHandle<UnityEngine.Object>(
                            resourceVariantDataSettings.Resource,
                            runtimeResourceManager,
                            loggerResolver),
#else
                        ResourceManagementFactory.BuildPreallocatedResourceStorageHandle<UnityEngine.Object>(
                            resourceVariantDataSettings.Resource,
                            runtimeResourceManager,
                            loggerResolver),
#endif
                        true,
                        localProgress)
                        .ThrowExceptions<IResourceVariantData, AssetImporterFromScriptable>(logger);

                    logger?.Log<AssetImporterFromScriptable>(
                        $"IMPORTING {resourceID} FINISHED");

                    resourcesLoaded++;

                    progress?.Report((float)resourcesLoaded / (float)totalResources);

                    await Task.Yield();
                }
            }

            progress?.Report(1f);

            return null;
        }

        public override void Cleanup()
        {
            settings = null;
        }
    }
}