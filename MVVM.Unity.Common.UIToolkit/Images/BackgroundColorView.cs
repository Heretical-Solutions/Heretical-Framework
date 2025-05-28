using System;

using HereticalSolutions.LifetimeManagement;

using UnityEngine;
using UnityEngine.UIElements;

namespace HereticalSolutions.MVVM.Unity.Common.UIToolkit
{
    public class BackgroundColorView
        : AView,
        ISetUppable,
        ITearDownable
    {
        private readonly string propertyID;

        protected VisualElement visualElement;

        private IObservableProperty<Color> colorProperty = null;

        public BackgroundColorView(
            string propertyID,
            VisualElement visualElement)
        {
            this.propertyID = propertyID;

            this.visualElement = visualElement;
        }

        #region ISetUppable

        public bool SetUp()
        {
            if (ViewModel == null)
                return false;

            if (!ViewModel.TryGetObservable<Color>(
                    propertyID,
                    out colorProperty))
                throw new Exception(
                    $"COULD NOT OBTAIN PROPERTY \"{propertyID}\" FROM VIEWMODEL \"{ViewModel.GetType().Name}\"");

            colorProperty.OnValueChanged += OnColorChanged;

            OnColorChanged(colorProperty.Value);

            return true;
        }

        #endregion

        private void OnColorChanged(Color newValue)
        {
            visualElement.style.backgroundColor = newValue;
        }

        #region ITearDownable

        public void TearDown()
        {
            if (colorProperty != null)
            {
                colorProperty.OnValueChanged -= OnColorChanged;

                colorProperty = null;
            }
        }

        #endregion
    }
}