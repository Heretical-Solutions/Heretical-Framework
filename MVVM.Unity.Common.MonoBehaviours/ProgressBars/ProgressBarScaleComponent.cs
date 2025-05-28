using UnityEngine;

namespace HereticalSolutions.MVVM.Unity.Common.MonoBehaviours
{
    public class ProgressBarScaleComponent
        : ViewComponent
    {
        [SerializeField]
        private string propertyID;
        
        [SerializeField]
        private Transform scaleAnchor;
        
        protected override void Awake()
        {
            BuildView();
            
            if (BaseVMComponent == null
                || BaseVMComponent.ViewModel == null)
                CreateViewLifetime();
            
            base.Awake();
        }
        
        private void BuildView()
        {
            View = new ProgressBarScaleView(
                propertyID,
                scaleAnchor);
        }
    }
}