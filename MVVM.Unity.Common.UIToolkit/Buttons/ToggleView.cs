using System;

using HereticalSolutions.LifetimeManagement;

using UnityEngine.UIElements;

namespace HereticalSolutions.MVVM.Unity.Common.UIToolkit
{
    public class ToggleView
        : AView,
          ISetUppable,
          ITearDownable
    {
        protected string propertyID;
        
        protected Toggle toggle;

        protected IObservableProperty<bool> boolProperty = null;

        protected bool togglePressed = false;
        
        public ToggleView(
            string propertyID,
            Toggle toggle)
        {
            this.propertyID = propertyID;

            this.toggle = toggle;
        }

        #region ISetUppable

        public bool SetUp()
        {
            if (ViewModel == null)
                return false;

            if (!ViewModel.TryGetObservable<bool>(
                propertyID,
                out boolProperty))
                throw new Exception(
                    $"Could not obtain property \"{propertyID}\" from ViewModel \"{ViewModel.GetType()}\"");

            boolProperty.OnValueChanged += OnBoolChanged;

            OnBoolChanged(boolProperty.Value);
            
            toggle.RegisterValueChangedCallback(OnToggleValueChanged);

            return true;
        }

        #endregion

        protected virtual void OnBoolChanged(
            bool newValue)
        {
            if (togglePressed)
                return;
            
            toggle.value = newValue;
        }
        
        protected void OnToggleValueChanged(
            ChangeEvent<bool> @event)
        {
            togglePressed = true;
            
            boolProperty.Value = @event.newValue;

            togglePressed = false;
        }

        #region ITearDownable

        public void TearDown()
        {
            togglePressed = false;
            
            if (boolProperty != null)
            {
                boolProperty.OnValueChanged -= OnBoolChanged;

                boolProperty = null;
            }
            
            toggle.UnregisterValueChangedCallback(OnToggleValueChanged);
        }

        #endregion
    }
}