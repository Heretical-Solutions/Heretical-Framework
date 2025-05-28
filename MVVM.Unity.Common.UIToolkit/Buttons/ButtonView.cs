using System;

using HereticalSolutions.LifetimeManagement;

using UnityEngine.UIElements;

namespace HereticalSolutions.MVVM.Unity.Common.UIToolkit
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

            button.RegisterCallback<ClickEvent>(OnButtonClicked);

            return true;
        }

        #endregion

        private void OnButtonClicked(
            ClickEvent @event)
        {
            onClickCommand();
        }

        #region ITearDownable

        public void TearDown()
        {
            button.UnregisterCallback<ClickEvent>(OnButtonClicked);
        }

        #endregion
    }
}