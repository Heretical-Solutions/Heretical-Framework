using System;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.MVVM
{
    public abstract class AView
        : IView,
          IContainsLifetime
    {
        public AView()
        {
        }

        #region IView

        public IViewModel ViewModel { get; set; }

        #endregion
        
        #region IContainsLifetime

        public ILifetimeable Lifetime { get; set; }

        #endregion

        protected void TryObtainProperty<T>(
            string propertyID,
            out IObservableProperty<T> property)
        {
            bool propertyObtained = ViewModel.TryGetObservable<T>(
                propertyID,
                out property);

            if (!propertyObtained)
            {
                throw new Exception(
                    $"COULD NOT OBTAIN PROPERTY {propertyID} FROM VIEWMODEL {ViewModel.GetType().Name}");
            }
        }
    }
}