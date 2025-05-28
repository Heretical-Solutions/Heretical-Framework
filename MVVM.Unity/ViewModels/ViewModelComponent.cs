using HereticalSolutions.LifetimeManagement;
using HereticalSolutions.LifetimeManagement.Unity;
using HereticalSolutions.LifetimeManagement.Factories;

using UnityEngine;

namespace HereticalSolutions.MVVM.Unity
{
    public abstract class ViewModelComponent
        : MonoLifetimeable
    {
        [field: SerializeField]
        public ViewModelComponent ParentVMComponent { get; set; }

        public IViewModel ViewModel { get; protected set; }

        protected override IContainsLifetime LifetimeContainer
        {
            get
            {
                return ViewModel as IContainsLifetime;
            }
        }
        
        protected void CreateViewModelLifetime()
        {
            if (ViewModel == null)
                return;
            
            ILifetimeable parentLifetime = null;
            
            if (ParentVMComponent != null
                && ParentVMComponent.ViewModel != null
                && ParentVMComponent.ViewModel is IContainsLifetime viewModelAsLifetimeContainer)
            
                parentLifetime = viewModelAsLifetimeContainer.Lifetime;
            
            LifetimeFactory.BuildHierarchicalLifetime(
                parentLifetime,
                ViewModel);
        }
        
        protected virtual void Reset()
        {
            setUpStage = EMonoLifetimeStage.AWAKE;
            
            initializeStage = EMonoLifetimeStage.ON_ENABLE;
            
            cleanUpStage = EMonoLifetimeStage.ON_DISABLE;
            
            tearDownStage = EMonoLifetimeStage.ON_DESTROY;
        }
    }
}