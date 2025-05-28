/*
using System;
using System.Collections.Generic;
using System.Threading;

using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement.Factories
{
    /// <summary>
    /// Factory class for creating instances related to the runtime resource manager
    /// </summary>
    public static class ResourceManagementFactory
    {
        /// <summary>
        /// Builds a new instance of the <see cref="RuntimeResourceManager"/> class
        /// </summary>
        /// <returns>A new instance of the <see cref="RuntimeResourceManager"/> class.</returns>
        public static RuntimeResourceManager BuildRuntimeResourceManager(
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<RuntimeResourceManager>();

            return new RuntimeResourceManager(
                RepositoryFactory.BuildDictionaryRepository<int, string>(),
                RepositoryFactory.BuildDictionaryRepository<int, IReadOnlyResourceData>(),
                logger);
        }

        public static ConcurrentRuntimeResourceManager BuildConcurrentRuntimeResourceManager(
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ConcurrentRuntimeResourceManager>();

            return new ConcurrentRuntimeResourceManager(
                RepositoryFactory.BuildConcurrentDictionaryRepository<int, string>(),
                RepositoryFactory.BuildConcurrentDictionaryRepository<int, IReadOnlyResourceData>(),
                NotifierFactory.BuildAsyncNotifierSingleArgGeneric<int, IReadOnlyResourceData>(loggerResolver),
                new SemaphoreSlim(1, 1),
                logger);
        }

        public static ResourceData BuildResourceData(
            ResourceDescriptor descriptor,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ResourceData>();

            return new ResourceData(
                descriptor,
                RepositoryFactory.BuildDictionaryRepository<int, string>(),
                RepositoryFactory.BuildDictionaryRepository<int, IResourceVariantData>(),
                RepositoryFactory.BuildDictionaryRepository<int, string>(),
                RepositoryFactory.BuildDictionaryRepository<int, IReadOnlyResourceData>(),
                logger);
        }

        public static ConcurrentResourceData BuildConcurrentResourceData(
            ResourceDescriptor descriptor,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ConcurrentResourceData>();

            return new ConcurrentResourceData(
                descriptor,
                RepositoryFactory.BuildConcurrentDictionaryRepository<int, string>(),
                RepositoryFactory.BuildConcurrentDictionaryRepository<int, IResourceVariantData>(),
                NotifierFactory.BuildAsyncNotifierSingleArgGeneric<int, IResourceVariantData>(loggerResolver),
                RepositoryFactory.BuildConcurrentDictionaryRepository<int, string>(),
                RepositoryFactory.BuildConcurrentDictionaryRepository<int, IReadOnlyResourceData>(),
                NotifierFactory.BuildAsyncNotifierSingleArgGeneric<int, IReadOnlyResourceData>(loggerResolver),
                new SemaphoreSlim(1, 1),
                logger);
        }

        /// <summary>
        /// Builds a new instance of the <see cref="ResourceVariantData"/> class
        /// </summary>
        /// <param name="descriptor">The descriptor of the resource variant data.</param>
        /// <param name="storageHandle">The storage handle of the resource variant data.</param>
        /// <returns>A new instance of the <see cref="ResourceVariantData"/> class.</returns>
        public static ResourceVariantData BuildResourceVariantData(
            ResourceVariantDescriptor descriptor,
            IReadOnlyResourceStorageHandle storageHandle,
            IReadOnlyResourceData resource)
        {
            return new ResourceVariantData(
                descriptor,
                storageHandle,
                resource);
        }

        public static PreallocatedResourceStorageHandle<TResource> BuildPreallocatedResourceStorageHandle<TResource>(
            TResource resource,
            IRuntimeResourceManager runtimeResourceManager,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<PreallocatedResourceStorageHandle<TResource>>();

            return new PreallocatedResourceStorageHandle<TResource>(
                resource,
                runtimeResourceManager,
                logger);
        }

        public static ConcurrentPreallocatedResourceStorageHandle<TResource> BuildConcurrentPreallocatedResourceStorageHandle<TResource>(
            TResource resource,
            IRuntimeResourceManager runtimeResourceManager,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ConcurrentPreallocatedResourceStorageHandle<TResource>>();

            return new ConcurrentPreallocatedResourceStorageHandle<TResource>(
                resource,
                new SemaphoreSlim(1, 1),
                runtimeResourceManager,
                logger);
        }

        public static ReadWriteResourceStorageHandle<TResource> BuildReadWriteResourceStorageHandle<TResource>(
            TResource resource,
            IRuntimeResourceManager runtimeResourceManager,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ReadWriteResourceStorageHandle<TResource>>();

            return new ReadWriteResourceStorageHandle<TResource>(
                resource,
                runtimeResourceManager,
                logger);
        }

        public static ConcurrentReadWriteResourceStorageHandle<TResource> BuildConcurrentReadWriteResourceStorageHandle<TResource>(
            TResource resource,
            IRuntimeResourceManager runtimeResourceManager,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ConcurrentReadWriteResourceStorageHandle<TResource>>();

            return new ConcurrentReadWriteResourceStorageHandle<TResource>(
                resource,
                new SemaphoreSlim(1, 1),
                runtimeResourceManager,
                logger);
        }

        public static ManagedTypeResourceManager<TResource, THandle> BuildManagedTypeResourceManager<TResource, THandle>(
            Func<THandle, THandle> newHandleAllocationDelegate,
            Func<TResource> newResourceAllocationDelegate,
            THandle uninitializedHandle = default,
            ILoggerResolver loggerResolver)
        {
            return new ManagedTypeResourceManager<TResource, THandle>(
                RepositoryFactory.BuildDictionaryRepository<THandle, TResource>(),
                new Queue<THandle>(),
                newHandleAllocationDelegate,
                newResourceAllocationDelegate,
                uninitializedHandle,
                loggerResolver?.GetLogger<ManagedTypeResourceManager<TResource, THandle>>());
        }
    }
}
*/