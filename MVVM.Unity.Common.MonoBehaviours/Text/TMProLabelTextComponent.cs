using UnityEngine;

using TMPro;

namespace HereticalSolutions.MVVM.Unity.Common.MonoBehaviours
{
    public class TMProLabelTextComponent
        : ViewComponent
    {
        [SerializeField]
        private string propertyID;
        
        [SerializeField]
        private TextMeshProUGUI textMeshProUGUI;
        
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
            View = new TMProLabelTextView(
                propertyID,
                textMeshProUGUI);
        }
    }
}