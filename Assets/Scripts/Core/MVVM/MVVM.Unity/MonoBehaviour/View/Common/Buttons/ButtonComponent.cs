using UnityEngine;
using UnityEngine.UI;

using ILogger = HereticalSolutions.Logging.ILogger;

using Zenject;

namespace HereticalSolutions.MVVM.Mono
{
    public class ButtonComponent : AViewComponent
    {
        [Inject]
        private ILogger logger;

        [SerializeField]
        protected string commandID;

        [SerializeField]
        private Button button;

        protected override void Awake()
        {
            view = new ButtonView(
                baseViewModel.ViewModel,
                commandID,
                button,
                logger);
            
            base.Awake();
        }
    }
}