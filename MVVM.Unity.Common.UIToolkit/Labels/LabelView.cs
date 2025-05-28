using System;

using HereticalSolutions.LifetimeManagement;

using UnityEngine.UIElements;

namespace HereticalSolutions.MVVM.Unity.Common.UIToolkit
{
    public class TMProLabelTextView
        : AView,
          ISetUppable,
          ITearDownable
    {
        private readonly string propertyID;

        protected Label label;

        private IObservableProperty<string> textProperty = null;

        public TMProLabelTextView(
            string propertyID,
            Label label)
        {
            this.propertyID = propertyID;

            this.label = label;
        }

        #region ISetUppable

        public bool SetUp()
        {
            if (ViewModel == null)
                return false;

            if (!ViewModel.TryGetObservable<string>(
                    propertyID,
                    out textProperty))
                throw new Exception(
                    $"COULD NOT OBTAIN PROPERTY \"{propertyID}\" FROM VIEWMODEL \"{ViewModel.GetType().Name}\"");

            textProperty.OnValueChanged += OnTextChanged;

            OnTextChanged(textProperty.Value);

            return true;
        }

        #endregion

        private void OnTextChanged(
            string newValue)
        {
            label.text = string.IsNullOrEmpty(newValue)
                ? string.Empty
                : newValue;
        }

        #region ITearDownable

        public void TearDown()
        {
            if (textProperty != null)
            {
                textProperty.OnValueChanged -= OnTextChanged;

                textProperty = null;
            }
        }

        #endregion
    }
}