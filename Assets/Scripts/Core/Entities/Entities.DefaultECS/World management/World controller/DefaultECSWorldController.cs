using System;

using DefaultEcs;
using DefaultEcs.System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.GameEntities
{
    public class DefaultECSWorldController
        <TEntityID,
        TEntityIDComponent,
        TWorldIdentityComponent,
        TResolveWorldIdentityComponent>
        : IWorldController<World, ISystem<Entity>, Entity>,
          IPrototypeCompliantWorldController<World, Entity>,
          IEntityIDCompliantWorldController<TEntityID, Entity>,
          IRegistryCompliantWorldController<Entity>
    {
        #region Delegates

        #region ID component

        private readonly Func<TEntityIDComponent, TEntityID> getEntityIDFromIDComponentDelegate;

        private readonly Func<TEntityID, TEntityIDComponent> createIDComponentDelegate;

        #endregion

        #region World identity component

        private readonly Func<TWorldIdentityComponent, Entity> getEntityFromWorldIdentityComponentDelegate;

        private readonly Func<TWorldIdentityComponent, string> getPrototypeIDFromWorldIdentityComponentDelegate;

        private readonly Func<string, Entity, TWorldIdentityComponent> createWorldIdentityComponentDelegate;

        #endregion

        #region Resolve world identity component

        private readonly Func<object, TResolveWorldIdentityComponent> createResolveWorldIdentityComponentDelegate;

        #endregion

        #endregion

        private readonly IPrototypesRepository<World, Entity> prototypeRepository;

        #region Systems

        private ISystem<Entity> resolveSystems;

        private ISystem<Entity> initializationSystems;

        private ISystem<Entity> deinitializationSystems;

        #endregion

        private readonly ILogger logger;
        

        public DefaultECSWorldController(
            World world,

            Func<TEntityIDComponent, TEntityID> getEntityIDFromIDComponentDelegate,
            Func<TEntityID, TEntityIDComponent> createIDComponentDelegate,

            Func<TWorldIdentityComponent, Entity> getEntityFromWorldIdentityComponentDelegate,
            Func<TWorldIdentityComponent, string> getPrototypeIDFromWorldIdentityComponentDelegate,
            Func<string, Entity, TWorldIdentityComponent> createWorldIdentityComponentDelegate,

            Func<object, TResolveWorldIdentityComponent> createResolveWorldIdentityComponentDelegate,

            IPrototypesRepository<World, Entity> prototypeRepository,
            ILogger logger = null)
        {
            World = world;


            this.getEntityIDFromIDComponentDelegate = getEntityIDFromIDComponentDelegate;

            this.createIDComponentDelegate = createIDComponentDelegate;


            this.getEntityFromWorldIdentityComponentDelegate = getEntityFromWorldIdentityComponentDelegate;

            this.getPrototypeIDFromWorldIdentityComponentDelegate = getPrototypeIDFromWorldIdentityComponentDelegate;

            this.createWorldIdentityComponentDelegate = createWorldIdentityComponentDelegate;


            this.createResolveWorldIdentityComponentDelegate = createResolveWorldIdentityComponentDelegate;


            this.prototypeRepository = prototypeRepository;

            this.logger = logger;


            resolveSystems = null;

            initializationSystems = null;

            deinitializationSystems = null;
        }

        public void Initialize(
            ISystem<Entity> resolveSystems,
            ISystem<Entity> initializationSystems,
            ISystem<Entity> deinitializationSystems)
        {
            this.resolveSystems = resolveSystems;

            this.initializationSystems = initializationSystems;

            this.deinitializationSystems = deinitializationSystems;
        }

        #region IWorldController

        public World World { get; private set; }


        //Entity systems
        public ISystem<Entity> EntityResolveSystems { get => resolveSystems; }

        public ISystem<Entity> EntityInitializationSystems { get => initializationSystems; }

        public ISystem<Entity> EntityDeinitializationSystems { get => deinitializationSystems; }


        public bool TrySpawnEntity(
            out Entity entity)
        {
            entity = World.CreateEntity();

            //Process freshly spawned entity with initialization systems
            initializationSystems?.Update(entity);

            return true;
        }

        public bool TrySpawnAndResolveEntity(
            object source,
            out Entity entity)
        {
            entity = World.CreateEntity();

            //Mark entity as in need of resolving and provide a source as a payload to the component
            entity.Set<TResolveWorldIdentityComponent>(
                createResolveWorldIdentityComponentDelegate.Invoke(source));

            //Process freshly spawned entity with resolve systems
            resolveSystems?.Update(entity);

            //Don't need it anymore. Bye!
            entity.Remove<TResolveWorldIdentityComponent>();

            //Process freshly resolved entity with initialization systems
            initializationSystems?.Update(entity);

            return true;
        }

        public void DespawnEntity(
            Entity entity)
        {
            if (entity == default)
                return;

            if (entity.World != World)
                logger?.LogError<DefaultECSWorldController<TEntityID, TEntityIDComponent, TWorldIdentityComponent, TResolveWorldIdentityComponent>>(
                    $"ATTEMPT TO DESPAWN ENTITY FROM THE WRONG WORLD");

            if (entity.Has<DespawnComponent>())
                return;

            //Mark the entity for despawn
            entity.Set<DespawnComponent>();

            //Process the entity on its way to be despawned with deinitialization systems
            deinitializationSystems?.Update(entity);
        }

        #endregion

        #region IPrototypeCompliantWorldController

        public IPrototypesRepository<World, Entity> PrototypeRepository { get => prototypeRepository; }

        public bool TrySpawnEntityFromPrototype(
            string prototypeID,
            out Entity entity)
        {
            if (!TryClonePrototypeEntityToWorld(
                prototypeID,
                out entity))
            {
                return false;
            }

            //Process freshly spawned entity with initialization systems
            initializationSystems?.Update(entity);

            return true;
        }

        public bool TrySpawnAndResolveEntityFromPrototype(
            string prototypeID,
            object source,
            out Entity entity)
        {
            if (!TryClonePrototypeEntityToWorld(
                prototypeID,
                out entity))
            {
                return false;
            }

            //Mark entity as in need of resolving and provide a source as a payload to the component
            entity.Set<TResolveWorldIdentityComponent>(
                createResolveWorldIdentityComponentDelegate.Invoke(source));

            //Process freshly spawned entity with resolve systems
            resolveSystems?.Update(entity);

            //Don't need it anymore. Bye!
            entity.Remove<TResolveWorldIdentityComponent>();

            //Process freshly resolved entity with initialization systems
            initializationSystems?.Update(entity);

            return true;
        }

        #endregion

        #region IEntituyIDCompliantWorldController

        public bool TrySpawnEntityWithIDFromPrototype(
            string prototypeID,
            TEntityID entityID,
            out Entity entity)
        {
            if (!TrySpawnEntityFromPrototype(
                prototypeID,
                out entity))
            {
                return false;
            }

            //Give the entity its ID

            //ref GUIDComponent guidComponent = ref entity.Get<GUIDComponent>();
            //
            //guidComponent.GUID = guid;

            entity.Set<TEntityIDComponent>(
                createIDComponentDelegate.Invoke(entityID));

            return true;
        }

        public bool TrySpawnAndResolveEntityWithIDFromPrototype(
            string prototypeID,
            TEntityID entityID,
            object source,
            out Entity entity)
        {
            if (!TrySpawnAndResolveEntityFromPrototype(
                prototypeID,
                source,
                out entity))
            {
                return false;
            }

            //Give the entity its GUID

            //ref GUIDComponent guidComponent = ref entity.Get<GUIDComponent>();
            //
            //guidComponent.GUID = guid;

            entity.Set<TEntityIDComponent>(
                createIDComponentDelegate.Invoke(entityID));

            return true;
        }

        #endregion

        #region IRegistryCompliantWorldController

        public bool TrySpawnEntityFromRegistry(
            Entity registryEntity,
            out Entity localEntity)
        {
            localEntity = default;

            if (!registryEntity.Has<TWorldIdentityComponent>())
            {
                return false;
            }

            //Get the target ID from the registry entity
            //var guid = registryEntity.Get<GUIDComponent>().GUID;

            var entityID = getEntityIDFromIDComponentDelegate.Invoke(
                registryEntity.Get<TEntityIDComponent>());


            //Get the prototype ID from the registry entity
            var entityIdentityComponent = registryEntity.Get<TWorldIdentityComponent>();

            var prototypeID = getPrototypeIDFromWorldIdentityComponentDelegate.Invoke(entityIdentityComponent);


            if (!TrySpawnEntityWithIDFromPrototype(
                prototypeID,
                entityID,
                out localEntity))
            {
                return false;
            }

            //And now let's link registry entity to the one we just created
            registryEntity.Set<TWorldIdentityComponent>(
                createWorldIdentityComponentDelegate.Invoke(
                    prototypeID,
                    localEntity));

            return true;
        }

        public bool TrySpawnAndResolveEntityFromRegistry(
            Entity registryEntity,
            object source,
            out Entity localEntity)
        {
            localEntity = default;

            if (!registryEntity.Has<TWorldIdentityComponent>())
            {
                return false;
            }

            //Get the target ID from the registry entity
            //var guid = registryEntity.Get<GUIDComponent>().GUID;

            var entityID = getEntityIDFromIDComponentDelegate.Invoke(
                registryEntity.Get<TEntityIDComponent>());


            //Get the prototype ID from the registry entity
            var entityIdentityComponent = registryEntity.Get<TWorldIdentityComponent>();

            var prototypeID = getPrototypeIDFromWorldIdentityComponentDelegate.Invoke(entityIdentityComponent);


            if (!TrySpawnAndResolveEntityWithIDFromPrototype(
                prototypeID,
                entityID,
                source,
                out localEntity))
            {
                return false;
            }

            //And now let's link registry entity to the one we just created
            registryEntity.Set<TWorldIdentityComponent>(
                createWorldIdentityComponentDelegate.Invoke(
                    prototypeID,
                    localEntity));

            return true;
        }

        public bool TrySpawnEntityFromPrototypeAndLinkToRegistry(
            Entity registryEntity,
            string prototypeID,
            out Entity localEntity)
        {
            localEntity = default;

            //If there's already an entity of this world linked to the registry entity, we're done here
            if (registryEntity.Has<TWorldIdentityComponent>())
            {
                return false;
            }

            //Get the target ID from the registry entity

            //var guid = registryEntity.Get<GUIDComponent>().GUID;

            var entityID = getEntityIDFromIDComponentDelegate.Invoke(
                registryEntity.Get<TEntityIDComponent>());

            if (!TrySpawnEntityWithIDFromPrototype(
                prototypeID,
                entityID,
                out localEntity))
            {
                return false;
            }

            //And now let's link registry entity to the one we just created
            registryEntity.Set<TWorldIdentityComponent>(
                createWorldIdentityComponentDelegate.Invoke(
                    prototypeID,
                    localEntity));

            return true;
        }

        public bool TrySpawnAndResolveEntityFromPrototypeAndLinkToRegistry(
            Entity registryEntity,
            string prototypeID,
            object source,
            out Entity localEntity)
        {
            localEntity = default;

            //If there's already an entity of this world linked to the registry entity, we're done here
            if (registryEntity.Has<TWorldIdentityComponent>())
            {
                return false;
            }

            //Get the target ID from the registry entity
            //var guid = registryEntity.Get<GUIDComponent>().GUID;

            var entityID = getEntityIDFromIDComponentDelegate.Invoke(
                registryEntity.Get<TEntityIDComponent>());


            if (!TrySpawnAndResolveEntityWithIDFromPrototype(
                prototypeID,
                entityID,
                source,
                out localEntity))
            {
                return false;
            }

            //And now let's link registry entity to the one we just created
            registryEntity.Set<TWorldIdentityComponent>(
                createWorldIdentityComponentDelegate.Invoke(
                    prototypeID,
                    localEntity));

            return true;
        }

        public void DespawnEntityAndUnlinkFromRegistry(
            Entity registryEntity)
        {
            if (!registryEntity.Has<TWorldIdentityComponent>())
                return;

            ref var entityIdentityComponent = ref registryEntity.Get<TWorldIdentityComponent>();

            var localEntity = getEntityFromWorldIdentityComponentDelegate.Invoke(entityIdentityComponent);

            DespawnEntity(localEntity);

            registryEntity.Remove<TWorldIdentityComponent>();
        }

        public bool TryReplaceEntityFromPrototypeAndUpdateRegistry(
            Entity registryEntity,
            string prototypeID,
            out Entity localEntity)
        {
            bool alreadyHasIdentityComponent = registryEntity.Has<TWorldIdentityComponent>();

            if (alreadyHasIdentityComponent)
            {
                ref var entityIdentityComponent = ref registryEntity.Get<TWorldIdentityComponent>();

                var previousEntity = getEntityFromWorldIdentityComponentDelegate.Invoke(entityIdentityComponent);

                DespawnEntity(previousEntity);
            }

            //Get the target ID from the registry entity
            //var guid = registryEntity.Get<GUIDComponent>().GUID;

            var entityID = getEntityIDFromIDComponentDelegate.Invoke(
                registryEntity.Get<TEntityIDComponent>());

            if (!TrySpawnEntityWithIDFromPrototype(
                prototypeID,
                entityID,
                out localEntity))
            {
                registryEntity.Remove<TWorldIdentityComponent>();

                return false;
            }

            //And now let's link registry entity to the one we just created
            registryEntity.Set<TWorldIdentityComponent>(
                createWorldIdentityComponentDelegate.Invoke(
                    prototypeID,
                    localEntity));

            return true;
        }

        public bool TryReplaceAndResolveEntityFromPrototypeAndUpdateRegistry(
            Entity registryEntity,
            string prototypeID,
            object source,
            out Entity localEntity)
        {
            bool alreadyHasIdentityComponent = registryEntity.Has<TWorldIdentityComponent>();

            if (alreadyHasIdentityComponent)
            {
                ref var entityIdentityComponent = ref registryEntity.Get<TWorldIdentityComponent>();

                var previousEntity = getEntityFromWorldIdentityComponentDelegate.Invoke(entityIdentityComponent);

                DespawnEntity(previousEntity);
            }

            //Get the target ID from the registry entity
            //var guid = registryEntity.Get<GUIDComponent>().GUID;

            var entityID = getEntityIDFromIDComponentDelegate.Invoke(
                registryEntity.Get<TEntityIDComponent>());

            if (!TrySpawnAndResolveEntityWithIDFromPrototype(
                prototypeID,
                entityID,
                source,
                out localEntity))
            {
                registryEntity.Remove<TWorldIdentityComponent>();

                return false;
            }

            //And now let's link registry entity to the one we just created
            registryEntity.Set<TWorldIdentityComponent>(
                createWorldIdentityComponentDelegate.Invoke(
                    prototypeID,
                    localEntity));

            return true;
        }

        #endregion

        private bool TryClonePrototypeEntityToWorld(
            string prototypeID,
            out Entity entity)
        {
            entity = default(Entity);

            if (string.IsNullOrEmpty(prototypeID))
            {
                logger?.LogError(
                    GetType(),
                    $"INVALID PROTOTYPE ID");

                return false;
            }

            if (!prototypeRepository.TryGetPrototype(
                prototypeID,
                out var prototypeEntity))
            {
                logger?.LogError(
                    GetType(),
                    $"NO PROTOTYPE REGISTERED BY ID {prototypeID}");

                return false;
            }

            entity = prototypeEntity.CopyTo(World);

            return true;
        }
    }
}