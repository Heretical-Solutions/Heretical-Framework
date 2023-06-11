using System;

namespace HereticalSolutions.Time
{
    public class SynchronizationContextDescriptor
    {
        public string ID { get; private set; }
        
        #region Toggleness

        public bool CanBeToggled { get; private set; }

        private bool active = true;
        
        public bool Active
        {
            get { return active; }
            set
            {
                if (!CanBeToggled)
                    throw new Exception(
                        "[SynchronizationContextDescriptor] ATTEMPT TO TOGGLE UNTOGGLABLE CONTEXT");

                active = value;
            }
        }

        #endregion
        
        #region Scalability
        
        public bool CanScale { get; private set; }

        private float scale = 1f;

        public float Scale
        {
            get { return scale; }
            set
            {
                if (!CanScale)
                    throw new Exception(
                        "[SynchronizationContextDescriptor] ATTEMPT TO CHANGE SCALE OF UNSCALABLE CONTEXT");

                scale = value;
            }
        }
        
        #endregion

        public SynchronizationContextDescriptor(
            string id,
            bool canBeToggled,
            bool canScale)
        {
            ID = id;
            
            CanBeToggled = canBeToggled;

            CanScale = canScale;
        }
    }
}