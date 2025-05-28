namespace HereticalSolutions.Synchronization.Time.TimeUpdaters
{
    public class TimeUpdaterDescriptor
    {
        public string ID { get; private set; }


        public bool Togglable { get; private set; }

        public bool Active { get; set; }


        public bool Scalable { get; private set; }

        public float Scale { get; set; }


        public bool HasFixedDelta { get; private set; }

        public float FixedDelta { get; private set; }


        public bool CanHaveNegativeDelta { get; private set; }

        public TimeUpdaterDescriptor(
            string id,
            bool togglable = true,
            bool active = true,
            bool scalable = true,
            float scale = 1f,
            bool hasFixedDelta = false,
            float fixedDelta = 0.02f,
            bool canHaveNegativeDelta = false)
        {
            ID = id;

            Togglable = togglable;

            Active = active;

            Scalable = scalable;

            Scale = scale;

            HasFixedDelta = hasFixedDelta;

            FixedDelta = fixedDelta;

            CanHaveNegativeDelta = canHaveNegativeDelta;
        }
    }
}