using UnityEngine;
using UnityEngine.UI;

namespace HereticalSolutions.MVVM.Unity.Common.MonoBehaviours
{
    public class ProgressBarImageComponent
        : ViewComponent
    {
        [SerializeField]
        private string propertyID;
        
        [SerializeField]
        private Image progressBar;
        
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
            View = new ProgressBarImageView(
                propertyID,
                progressBar);
        }
    }
}