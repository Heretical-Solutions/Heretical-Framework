using System;

using HereticalSolutions.LifetimeManagement;

using UnityEngine;

namespace HereticalSolutions.MVVM.Unity.Common.MonoBehaviours
{
    public class GameObjectEnabledView
		: AView,
          ISetUppable,
          ITearDownable
    {
        private readonly string propertyID;
        
        private readonly GameObject targetGameObject;
        
        private IObservableProperty<bool> enabledProperty = null;
        
        public GameObjectEnabledView(
            string propertyID,
            GameObject targetGameObject)
        {
            this.propertyID = propertyID;

            this.targetGameObject = targetGameObject;
        }

        #region ISetUppable

        public bool SetUp()
        {
            if (ViewModel == null)
                return false;
            
            if (!ViewModel.TryGetObservable<bool>(
                    propertyID,
                    out enabledProperty))
                throw new Exception(
                    $"COULD NOT OBTAIN PROPERTY \"{propertyID}\" FROM VIEWMODEL \"{ViewModel.GetType().Name}\"");

            enabledProperty.OnValueChanged += OnEnabledChanged;

            OnEnabledChanged(enabledProperty.Value);

            return true;
        }
        
        #endregion
        
        private void OnEnabledChanged(bool newValue)
        {
            targetGameObject.SetActive(newValue);
        }

        #region ITearDownable

        public void TearDown()
        {
            if (enabledProperty != null)
            {
                enabledProperty.OnValueChanged -= OnEnabledChanged;

                enabledProperty = null;
            }
        }
        
        #endregion
    }
}