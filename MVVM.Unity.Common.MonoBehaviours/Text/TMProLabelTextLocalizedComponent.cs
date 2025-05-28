using HereticalSolutions.Localization;

using UnityEngine;

using TMPro;

using Zenject;

namespace HereticalSolutions.MVVM.Unity.Common.MonoBehaviours
{
    public class TMProLabelTextLocalizedComponent
        : ViewComponent
    {
        [Inject]
        private ILocalizationManager localizationManager;
        
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
            View = new TMProLabelTextLocalizedView(
                propertyID,
                textMeshProUGUI,
                localizationManager);
        }
    }
}