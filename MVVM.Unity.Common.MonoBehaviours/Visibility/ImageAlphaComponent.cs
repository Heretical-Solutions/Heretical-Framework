using UnityEngine;
using UnityEngine.UI;

namespace HereticalSolutions.MVVM.Unity.Common.MonoBehaviours
{
    public class ImageAlphaComponent
        : ViewComponent
    {
        [SerializeField]
        private string propertyID;
        
        [SerializeField]
        private Image image;
        
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
            View = new ImageAlphaView(
                propertyID,
                image);
        }
    }
}