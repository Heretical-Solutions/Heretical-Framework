using UnityEngine;
using UnityEngine.UI;

namespace HereticalSolutions.MVVM.Unity.Common.MonoBehaviours
{
    public class ButtonComponent
        : ViewComponent
    {
        [SerializeField]
        private string commandID;
        
        [SerializeField]
        private Button button;

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
            View = new ButtonView(
                commandID,
                button);
        }
    }
}