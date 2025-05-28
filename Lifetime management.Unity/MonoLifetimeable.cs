using UnityEngine;

using NaughtyAttributes;

namespace HereticalSolutions.LifetimeManagement.Unity
{
    public abstract class MonoLifetimeable
        : MonoBehaviour
    {
        protected abstract IContainsLifetime LifetimeContainer { get; }
        
        [Foldout("Lifetime control")]
        [SerializeField]
        protected EMonoLifetimeStage setUpStage;
        
        [Foldout("Lifetime control")]
        [SerializeField]
        protected EMonoLifetimeStage initializeStage;
        
        [Foldout("Lifetime control")]
        [SerializeField]
        protected EMonoLifetimeStage cleanUpStage;

        [Foldout("Lifetime control")]
        [SerializeField]
        protected EMonoLifetimeStage tearDownStage;
        
        protected virtual void Awake()
        {
            UpdateLifetime(EMonoLifetimeStage.AWAKE);
        }

        protected virtual void Start()
        {
            UpdateLifetime(EMonoLifetimeStage.START);
        }

        protected virtual void OnEnable()
        {
            UpdateLifetime(EMonoLifetimeStage.ON_ENABLE);
        }

        protected virtual void OnDisable()
        {
            UpdateLifetime(EMonoLifetimeStage.ON_DISABLE);
        }

        protected virtual void OnDestroy()
        {
            UpdateLifetime(EMonoLifetimeStage.ON_DESTROY);
        }

        protected void UpdateLifetime(
            EMonoLifetimeStage stage)
        {
            var lifetimeContainer = LifetimeContainer;
            
            if (lifetimeContainer == null)
                return;
            
            if (!lifetimeContainer.Lifetime.IsSetUp
                && lifetimeContainer.Lifetime is ISetUppable setUppable
                && setUpStage == stage)
            {
                setUppable.SetUp();
            }
            
            if (!lifetimeContainer.Lifetime.IsInitialized
                && lifetimeContainer.Lifetime is IInitializable initializable
                && initializeStage == stage)
            {
                initializable.Initialize();
            }
            
            if (lifetimeContainer.Lifetime.IsInitialized
                && lifetimeContainer.Lifetime is ICleanuppable cleanuppable
                && cleanUpStage == stage)
            {
                cleanuppable.Cleanup();
            }
            
            if (lifetimeContainer.Lifetime.IsSetUp
                && lifetimeContainer.Lifetime is ITearDownable tearDownable
                && tearDownStage == stage)
            {
                tearDownable.TearDown();
            }
        }
    }
}