using System;

using HereticalSolutions.LifetimeManagement;

using UnityEngine;

namespace HereticalSolutions.MVVM.Unity.Common.MonoBehaviours
{
    public class RectTransformPositionView
        : AView,
          ISetUppable,
          ITearDownable
    {
        private readonly string propertyID;
        
        private readonly RectTransform targetRectTransform;
        
        private IObservableProperty<Vector2> positionProperty = null;
        
        public RectTransformPositionView(
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
            
            if (!ViewModel.TryGetObservable<Vector2>(
                    propertyID,
                    out positionProperty))
                throw new Exception(
                    $"COULD NOT OBTAIN PROPERTY \"{propertyID}\" FROM VIEWMODEL \"{ViewModel.GetType().Name}\"");

            positionProperty.OnValueChanged += OnPositionChanged;

            OnPositionChanged(
                positionProperty.Value);

            return true;
        }
        
        #endregion
        
        private void OnPositionChanged(
            Vector2 newValue)
        {
            targetRectTransform.position = newValue;
            //_targetRectTransform.anchoredPosition = newValue;
        }

        #region ITearDownable

        public void TearDown()
        {
            if (positionProperty != null)
            {
                positionProperty.OnValueChanged -= OnPositionChanged;

                positionProperty = null;
            }
        }
        
        #endregion
    }
}