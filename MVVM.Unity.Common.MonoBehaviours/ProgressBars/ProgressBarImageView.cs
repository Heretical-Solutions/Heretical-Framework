using System;

using HereticalSolutions.LifetimeManagement;

using UnityEngine;
using UnityEngine.UI;

namespace HereticalSolutions.MVVM.Unity.Common.MonoBehaviours
{
    public class ProgressBarImageView
        : AView,
          ISetUppable,
          ITearDownable
    {
        private readonly string propertyID;
        
        private readonly Image progressBar;
        
        private IObservableProperty<float> progressProperty = null;
        
        public ProgressBarImageView(
            string propertyID,
            Image progressBar)
        {
            this.propertyID = propertyID;

            this.progressBar = progressBar;
        }

        #region ISetUppable

        public bool SetUp()
        {
            if (ViewModel == null)
                return false;
            
            if (!ViewModel.TryGetObservable<float>(
                    propertyID,
                    out progressProperty))
                throw new Exception(
                    $"COULD NOT OBTAIN PROPERTY \"{propertyID}\" FROM VIEWMODEL \"{ViewModel.GetType().Name}\"");

            progressProperty.OnValueChanged += OnProgressChanged;

            OnProgressChanged(progressProperty.Value);

            return true;
        }
        
        #endregion
        
        private void OnProgressChanged(float newValue)
        {
            progressBar.fillAmount = Mathf.Clamp01(newValue);
        }

        #region ITearDownable

        public void TearDown()
        {
            if (progressProperty != null)
            {
                progressProperty.OnValueChanged -= OnProgressChanged;

                progressProperty = null;
            }
        }
        
        #endregion
    }
}