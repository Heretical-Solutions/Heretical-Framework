using System;
using System.Collections.Generic;

using HereticalSolutions.LifetimeManagement;

using UnityEngine;

namespace HereticalSolutions.MVVM.Unity.Common.MonoBehaviours
{
    public class TransformReparentingView
        : AView,
          ISetUppable,
          ITearDownable
    {
        private readonly string propertyID;
        
        private readonly Transform parentTransform;
        
        private IObservableProperty<Transform[]> targetTransformsProperty = null;
        
        private Dictionary<Transform, Transform> previousParents = new Dictionary<Transform, Transform>();
        
        public TransformReparentingView(
            string propertyID,
            Transform parentTransform)
        {
            this.propertyID = propertyID;

            this.parentTransform = parentTransform;
        }

        #region ISetUppable

        public bool SetUp()
        {
            if (ViewModel == null)
                return false;
            
            if (!ViewModel.TryGetObservable<Transform[]>(
                    propertyID,
                    out targetTransformsProperty))
                throw new Exception(
                    $"COULD NOT OBTAIN PROPERTY \"{propertyID}\" FROM VIEWMODEL \"{ViewModel.GetType().Name}\"");

            targetTransformsProperty.OnValueChanged += OnTargetsChanged;

            OnTargetsChanged(
                targetTransformsProperty.Value);

            return true;
        }
        
        #endregion
        
        private void OnTargetsChanged(
            Transform[] newValue)
        {
            foreach (var previousTarget in previousParents.Keys)
            {
                if (previousTarget == null)
                    continue;
                
                previousTarget.SetParent(
                    previousParents[previousTarget],
                    true);
            }
            
            previousParents.Clear();
            
            if (newValue == null)
                return;
            
            foreach (var target in newValue)
            {
                if (target == null)
                    continue;
                
                previousParents.Add(
                    target,
                    target.parent);
                
                target.SetParent(
                    parentTransform,
                    true);
            }
        }

        #region ITearDownable

        public void TearDown()
        {
            if (targetTransformsProperty != null)
            {
                targetTransformsProperty.OnValueChanged -= OnTargetsChanged;

                targetTransformsProperty = null;
            }
            
            previousParents.Clear();
        }
        
        #endregion
    }
}