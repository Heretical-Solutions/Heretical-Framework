using UnityEngine.AddressableAssets;

namespace HereticalSolutions.ResourceManagement.Factories
{
    /// <summary>
    /// Factory class for creating instances related to the runtime resource manager.
    /// </summary>
    public static class RuntimeResourceManagerFactoryUnity
    {
        /// <summary>
        /// Builds a new instance of the <see cref="ScriptableResourceStorageHandle"/> class.
        /// </summary>
        /// <param name="resource">The resource object.</param>
        /// <returns>A new instance of the <see cref="ScriptableResourceStorageHandle"/> class.</returns>
        public static ScriptableResourceStorageHandle BuildScriptableResourceStorageHandle(object resource)
        {
            return new ScriptableResourceStorageHandle(resource);
        }
        
        /// <summary>
        /// Builds a new instance of the <see cref="AddressableResourceStorageHandle"/> class.
        /// </summary>
        /// <param name="assetReference">The asset reference.</param>
        /// <returns>A new instance of the <see cref="AddressableResourceStorageHandle"/> class.</returns>
        public static AddressableResourceStorageHandle BuildAddressableResourceStorageHandle(AssetReference assetReference)
        {
            return new AddressableResourceStorageHandle(assetReference);
        }
    }
}