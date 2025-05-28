using System;

using HereticalSolutions.LifetimeManagement;

using UnityEngine;
using UnityEngine.UI;

namespace HereticalSolutions.MVVM.Unity.Common.MonoBehaviours
{
    public class ImageAlphaView
        : AView,
        ISetUppable,
        ITearDownable
    {
        private readonly string propertyID;
        
        private readonly Image image;
        
        private IObservableProperty<float> alphaProperty = null;
        
        public ImageAlphaView(
            string propertyID,
            Image image)
        {
            this.propertyID = propertyID;

            this.image = image;
        }

        #region ISetUppable

        public bool SetUp()
        {
            if (ViewModel == null)
                return false;
            
            if (!ViewModel.TryGetObservable<float>(
                    propertyID,
                    out alphaProperty))
                throw new Exception(
                    $"COULD NOT OBTAIN PROPERTY \"{propertyID}\" FROM VIEWMODEL \"{ViewModel.GetType().Name}\"");

            alphaProperty.OnValueChanged += OnAlphaChanged;

            OnAlphaChanged(alphaProperty.Value);

            return true;
        }
        
        #endregion
        
        private void OnAlphaChanged(float newValue)
        {
            image.color = new Color(
                image.color.r,
                image.color.g,
                image.color.b,
                Mathf.Clamp01(newValue));
        }

        #region ITearDownable

        public void TearDown()
        {
            if (alphaProperty != null)
            {
                alphaProperty.OnValueChanged -= OnAlphaChanged;

                alphaProperty = null;
            }
        }
        
        #endregion
    }
}