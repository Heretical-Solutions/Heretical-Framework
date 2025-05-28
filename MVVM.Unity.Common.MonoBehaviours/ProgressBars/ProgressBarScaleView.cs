using System;

using HereticalSolutions.LifetimeManagement;

using UnityEngine;

namespace HereticalSolutions.MVVM.Unity.Common.MonoBehaviours
{
    public class ProgressBarScaleView
        : AView,
          ISetUppable,
          ITearDownable
    {
        private readonly string propertyID;
        
        private readonly Transform scaleAnchor;
        
        private IObservableProperty<float> progressProperty = null;
        
        public ProgressBarScaleView(
            string propertyID,
            Transform scaleAnchor)
        {
            this.propertyID = propertyID;

            this.scaleAnchor = scaleAnchor;
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
        
        protected void OnProgressChanged(float newValue)
        {
            scaleAnchor.localScale = new Vector3(
                Mathf.Clamp01(newValue),
                1f,
                1f);
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