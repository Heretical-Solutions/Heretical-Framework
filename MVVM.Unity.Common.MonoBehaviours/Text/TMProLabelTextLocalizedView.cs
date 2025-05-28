using System;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Localization;

using TMPro;

namespace HereticalSolutions.MVVM.Unity.Common.MonoBehaviours
{
    public class TMProLabelTextLocalizedView
        : AView,
          ISetUppable,
          ITearDownable
    {
        private readonly string propertyID;
        
        private readonly TextMeshProUGUI textMeshProUGUI;
        
        private readonly ILocalizationManager localizationManager;
        
        private IObservableProperty<string> _textProperty = null;
        
        public TMProLabelTextLocalizedView(
            string propertyID,
            TextMeshProUGUI textMeshProUGUI,
            ILocalizationManager localizationManager)
        {
            this.propertyID = propertyID;

            this.textMeshProUGUI = textMeshProUGUI;
            
            this.localizationManager = localizationManager;
        }

        #region ISetUppable

        public bool SetUp()
        {
            if (ViewModel == null)
                return false;
            
            if (!ViewModel.TryGetObservable<string>(
                    propertyID,
                    out _textProperty))
                throw new Exception(
                    $"COULD NOT OBTAIN PROPERTY \"{propertyID}\" FROM VIEWMODEL \"{ViewModel.GetType().Name}\"");

            _textProperty.OnValueChanged += OnTextChanged;
            
            localizationManager.OnLanguageChanged += OnLocalizationChanged;

            OnTextChanged(
                _textProperty.Value);

            return true;
        }
        
        #endregion
        
        private void OnTextChanged(
            string newValue)
        {
            textMeshProUGUI.text = localizationManager.GetLocalizedString(newValue, null);
        }
        
        private void OnLocalizationChanged(
            string newLocale)
        {
            textMeshProUGUI.text = localizationManager.GetLocalizedString( 
                _textProperty.Value, null);
        }

        #region ITearDownable

        public void TearDown()
        {
            if (_textProperty != null)
            {
                _textProperty.OnValueChanged -= OnTextChanged;

                _textProperty = null;
            }

            if (localizationManager != null)
            {
                localizationManager.OnLanguageChanged -= OnLocalizationChanged;
            }
        }
        
        #endregion
    }
}