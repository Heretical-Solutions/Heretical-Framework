using System;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.GameEntities
{
    public class InitializeViewModelComponentSystem<TEntityID, TEntityIDComponent> : ISystem<Entity>
    {
        private readonly Func<TEntityIDComponent, TEntityID> getEntityIDFromIDComponentDelegate;

        private readonly IEntityManager<World, TEntityID, Entity> entityManager;
        
        public InitializeViewModelComponentSystem(
            IEntityManager<World, TEntityID, Entity> entityManager)
        {
            this.entityManager = entityManager;
        }

        public bool IsEnabled { get; set; } = true;

        void ISystem<Entity>.Update(Entity entity)
        {
            if (!IsEnabled)
                return;

            if (!entity.Has<ViewModelComponent>())
                return;

            ref ViewModelComponent viewModelComponent = ref entity.Get<ViewModelComponent>();


            //var guid = entity.Get<GUIDComponent>().GUID;
            var entityID = getEntityIDFromIDComponentDelegate.Invoke(
                entity.Get<TEntityIDComponent>());
            
            var registryEntity = entityManager.GetRegistryEntity(entityID);


            //TODO: add world ID to component type repository and add preferred world ID to component to ensure we can pick any entity as a model for a VM
            if (registryEntity.Has<SimulationEntityComponent>())
            {
                var simulationEntity = registryEntity.Get<SimulationEntityComponent>().SimulationEntity;

                viewModelComponent.SourceEntity = simulationEntity;
            }
        }

        /// <summary>
        /// Disposes the system.
        /// </summary>
        public void Dispose()
        {
        }
    }
}