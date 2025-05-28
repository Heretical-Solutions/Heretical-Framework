using System;

using HereticalSolutions.LifetimeManagement;

using UnityEngine;

namespace HereticalSolutions.MVVM.Unity.Common.MonoBehaviours
{
    public class RectTransformRotationView
        : AView,
          ISetUppable,
          ITearDownable
    {
        private readonly string propertyID;
        
        private readonly RectTransform targetRectTransform;
        
        private IObservableProperty<float> rotationProperty = null;
        
        public RectTransformRotationView(
            string propertyID,
            RectTransform targetRectTransform)
        {
            this.propertyID = propertyID;

            this.targetRectTransform = targetRectTransform;
        }

        #region ISetUppable

        public bool SetUp()
        {
            if (ViewModel == null)
                return false;
            
            if (!ViewModel.TryGetObservable<float>(
                    propertyID,
                    out rotationProperty))
                throw new Exception(
                    $"COULD NOT OBTAIN PROPERTY \"{propertyID}\" FROM VIEWMODEL \"{ViewModel.GetType().Name}\"");

            rotationProperty.OnValueChanged += OnRotationChanged;

            OnRotationChanged(rotationProperty.Value);

            return true;
        }
        
        #endregion
        
        private void OnRotationChanged(float newValue)
        {
            targetRectTransform.eulerAngles = new Vector3(
                targetRectTransform.eulerAngles.x,
                targetRectTransform.eulerAngles.y,
                newValue);
        }

        #region ITearDownable

        public void TearDown()
        {
            if (rotationProperty != null)
            {
                rotationProperty.OnValueChanged -= OnRotationChanged;

                rotationProperty = null;
            }
        }
        
        #endregion
    }
}