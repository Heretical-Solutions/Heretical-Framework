using UnityEngine;

namespace HereticalSolutions.MVVM.Unity.Common.MonoBehaviours
{
    public class RectTransformPositionComponent
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
            View = new RectTransformPositionView(
                propertyID,
                targetRectTransform);
        }
    }
}