using System;

using HereticalSolutions.LifetimeManagement;

using UnityEngine.UI;

namespace HereticalSolutions.MVVM.Unity.Common.MonoBehaviours
{
    public class ButtonView
        : AView,
          ISetUppable,
          ITearDownable
    {
        private readonly string commandID;
        
        private readonly Button button;
        
        private CommandDelegate onClickCommand = null;
        
        public ButtonView(
            string commandID,
            Button button)
        {
            this.commandID = commandID;

            this.button = button;
        }

        #region ISetUppable
        
        public bool SetUp()
        {
            if (ViewModel == null)
                return false;
            
            onClickCommand = ViewModel.GetCommand(
                commandID);

            if (onClickCommand == null)
                throw new Exception(
                    $"COULD NOT OBTAIN COMMAND \"{commandID}\" FROM VIEWMODEL \"{ViewModel.GetType().Name}\"");

            button.onClick.AddListener(
                OnButtonClicked);

            return true;
        }
        
        #endregion
        
        private void OnButtonClicked()
        {
            onClickCommand();
        }

        #region ITearDownable

        public void TearDown()
        {
            button.onClick.RemoveAllListeners();
        }
        
        #endregion
    }
}