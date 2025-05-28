using HereticalSolutions.LifetimeManagement;
using HereticalSolutions.LifetimeManagement.Unity;
using HereticalSolutions.LifetimeManagement.Factories;

using UnityEngine;

namespace HereticalSolutions.MVVM.Unity
{
    public abstract class ViewComponent
        : MonoLifetimeable
    {
        [field: SerializeField]
        public ViewModelComponent BaseVMComponent { get; set; }

        public IView View { get; protected set; }

        protected override IContainsLifetime LifetimeContainer
        {
            get
            {
                return View as IContainsLifetime;
            }
        }
        
        protected virtual void Reset()
        {
            setUpStage = EMonoLifetimeStage.NONE;
            
            initializeStage = EMonoLifetimeStage.NONE;
            
            cleanUpStage = EMonoLifetimeStage.NONE;
            
            tearDownStage = EMonoLifetimeStage.NONE;
        }
        
        protected override void Awake()
        {
            UpdateViewModelIfMissing();
            
             base.Awake();
        }

        protected override void Start()
        {
            UpdateViewModelIfMissing();
            
            base.Start();
        }

        protected override void OnEnable()
        {
            UpdateViewModelIfMissing();
            
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            UpdateViewModelIfMissing();
            
            base.OnDisable();
        }

        protected override void OnDestroy()
        {
            UpdateViewModelIfMissing();
            
            base.OnDestroy();
        }
        
        protected virtual void Update()
        {
            UpdateViewModelIfMissing();
        }
        
        protected void CreateViewLifetime()
        {
            if (View == null)
                return;
            
            ILifetimeable parentLifetime = null;

            if (BaseVMComponent != null
                && BaseVMComponent.ViewModel != null
                && BaseVMComponent.ViewModel is IContainsLifetime viewModelAsLifetimeContainer)
            {
                parentLifetime = viewModelAsLifetimeContainer.Lifetime;
            }

            LifetimeFactory.BuildHierarchicalLifetime(
                parentLifetime,
                View);
        }
        
        protected void UpdateViewModelIfMissing()
        {
            if (View == null)
                return;
            
            if (View.ViewModel != null)
                return;
            
            if (BaseVMComponent == null)
                return;
            
            if (BaseVMComponent.ViewModel == null)
                return;
            
            View.ViewModel = BaseVMComponent.ViewModel;

            if (View is IContainsLifetime lifetimeContainer
                && lifetimeContainer.Lifetime != null)
            {
                if (lifetimeContainer.Lifetime is ITearDownable tearDownable)
                {
                    tearDownable.TearDown();
                }

                lifetimeContainer.Lifetime = null;
            }

            CreateViewLifetime();
        }
    }
}