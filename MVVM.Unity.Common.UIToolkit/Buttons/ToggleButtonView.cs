using System;

using HereticalSolutions.LifetimeManagement;

using UnityEngine.UIElements;

namespace HereticalSolutions.MVVM.Unity.Common.UIToolkit
{
    public class ToggleButtonView
        : AView,
          ISetUppable,
          ITearDownable
    {
        protected string propertyID;

        private readonly Button button;

        protected IObservableProperty<bool> boolProperty = null;

        public ToggleButtonView(
            string propertyID,
            Button button)
        {
            this.propertyID = propertyID;

            this.button = button;
        }

        #region ISetUppable

        public bool SetUp()
        {
            if (ViewModel == null)
                return false;

            if(!ViewModel.TryGetObservable<bool>(
                    propertyID,
                    out boolProperty))
                throw new Exception(
                    $"COULD NOT OBTAIN PROPERTY \"{propertyID}\" FROM VIEWMODEL \"{ViewModel.GetType().Name}\"");

            button.RegisterCallback<ClickEvent>(OnButtonClicked);

            return true;
        }

        #endregion

        private void OnButtonClicked(
            ClickEvent @event)
        {
            boolProperty.Value = !boolProperty.Value;
        }

        #region ITearDownable

        public void TearDown()
        {
            button.UnregisterCallback<ClickEvent>(OnButtonClicked);
        }

        #endregion
    }
}