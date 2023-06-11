using System;
using System.Threading.Tasks;

using UnityEngine.AddressableAssets;

namespace HereticalSolutions.ResourceManagement
{
    /// <summary>
    /// Represents a storage handle for an addressable resource.
    /// </summary>
    public class AddressableResourceStorageHandle : IResourceStorageHandle
    {
        private bool allocated = false;

        private Object resource;

        private AssetReference assetReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressableResourceStorageHandle"/> class.
        /// </summary>
        /// <param name="assetReference">The asset reference for the resource.</param>
        public AddressableResourceStorageHandle(AssetReference assetReference)
        {
            this.assetReference = assetReference;
        }

        #region IResourceStorageHandle
        
        /// <summary>
        /// Gets a value indicating whether the resource is allocated.
        /// </summary>
        public bool Allocated
        {
            get => allocated;
        }
        
        /// <summary>
        /// Allocates the resource asynchronously.
        /// </summary>
        /// <param name="progress">An optional progress reporter for tracking the allocation progress.</param>
        /// <returns>A task representing the asynchronous allocation operation.</returns>
        public async Task Allocate(IProgress<float> progress = null)
        {
            progress?.Report(0f);
            
            if (allocated)
                throw new Exception("[AddressableResourceStorageHandle] RESOURCE ALREADY ALLOCATED");
            
            resource = await assetReference.LoadAssetAsync<UnityEngine.Object>().Task; //TODO: change to while() loop and report progress from handle

            allocated = true;
            
            progress?.Report(1f);
        }

        /// <summary>
        /// Gets the resource object.
        /// </summary>
        public object Resource
        {
            get
            {
                if (!allocated)
                    return null;

                return resource;
            }
        }
        
        /// <summary>
        /// Gets the resource object of the specified type.
        /// </summary>
        /// <typeparam name="TValue">The type of the resource object.</typeparam>
        /// <returns>The resource object of the specified type, or the default value if not allocated.</returns>
        public TValue GetResource<TValue>()
        {
            if (!allocated)
                return default(TValue);

            return (TValue)resource;
        }

        /// <summary>
        /// Frees the allocated resource asynchronously.
        /// </summary>
        /// <param name="progress">An optional progress reporter for tracking the free operation progress.</param>
        /// <returns>A task representing the asynchronous free operation.</returns>
        public async Task Free(IProgress<float> progress = null)
        {
            progress?.Report(0f);
            
            if (!allocated)
                throw new Exception("[AddressableResourceStorageHandle] RESOURCE ALREADY FREED");

            assetReference.ReleaseAsset();
            
            progress?.Report(1f);
        }
        
        #endregion
    }
}