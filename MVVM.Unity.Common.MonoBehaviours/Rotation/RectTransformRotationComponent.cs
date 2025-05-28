using UnityEngine;

namespace HereticalSolutions.MVVM.Unity.Common.MonoBehaviours
{
    public class RectTransformRotationComponent
        : ViewComponent
    {
        [SerializeField]
        private string propertyID;
        
        [SerializeField]
        private RectTransform targetRectTransform;
        
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
            View = new RectTransformRotationView(
                propertyID,
                targetRectTransform);
        }
    }
}