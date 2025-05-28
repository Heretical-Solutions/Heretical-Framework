namespace HereticalSolutions.Synchronization.Time.TimeUpdaters
{
	public class TickUpdaterDescriptor
	{
		public string ID { get; private set; }


		public bool Togglable { get; private set; }

		public bool Active { get; set; }

		public TickUpdaterDescriptor(
			string id,
			bool togglable = true,
			bool active = true)
		{
			ID = id;

			Togglable = togglable;

			Active = active;
		}
	}
}