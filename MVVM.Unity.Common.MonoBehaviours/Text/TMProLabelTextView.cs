using System;

using HereticalSolutions.LifetimeManagement;

using TMPro;

namespace HereticalSolutions.MVVM.Unity.Common.MonoBehaviours
{
    public class TMProLabelTextView
        : AView,
          ISetUppable,
          ITearDownable
    {
        private readonly string propertyID;
        
        private readonly TextMeshProUGUI textMeshProUGUI;
        
        private IObservableProperty<string> textProperty = null;
        
        public TMProLabelTextView(
            string propertyID,
            TextMeshProUGUI textMeshProUGUI)
        {
            this.propertyID = propertyID;

            this.textMeshProUGUI = textMeshProUGUI;
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
        
        private void OnTextChanged(string newValue)
        {
            textMeshProUGUI.text = newValue;
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