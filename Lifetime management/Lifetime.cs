using System;

namespace HereticalSolutions.LifetimeManagement
{
	public class Lifetime
		: ILifetimeable,
		  ISetUppable,
		  IInitializable,
		  ICleanuppable,
		  ITearDownable,
		  IDisposable
	{
		protected readonly Func<bool> setUp;

		protected readonly Func<bool> initialize;
		
		protected readonly Action cleanup;
		
		protected readonly Action tearDown;


		protected ESynchronizationFlags setUpFlags;

		protected ESynchronizationFlags initializeFlags;


		protected bool isInitialized = false;

		protected bool isSetUp = false;


		public Lifetime(
			ESynchronizationFlags setUpFlags = 
				ESynchronizationFlags.SYNC_WITH_PARENT,
			
			ESynchronizationFlags initializeFlags = 
				ESynchronizationFlags.SYNC_WITH_PARENT,
			
			Func<bool> setUp = null,
			Func<bool> initialize = null,
			Action cleanup = null,
			Action tearDown = null)
		{
			this.setUpFlags = setUpFlags;
			
			this.initializeFlags = initializeFlags;
			
			this.setUp = setUp;
			
			this.initialize = initialize;
			
			this.cleanup = cleanup;
			
			this.tearDown = tearDown;
		}

		#region ILifetimeable

		public bool IsSetUp => isSetUp;

		public bool IsInitialized => isInitialized;

		public Action<ILifetimeable> OnSetUp { get; set; }

		public ESynchronizationFlags SetUpFlags => setUpFlags;

		public Action<ILifetimeable> OnInitialized { get; set; }

		public ESynchronizationFlags InitializeFlags => initializeFlags;

		public Action<ILifetimeable> OnCleanedUp { get; set; }

		public Action<ILifetimeable> OnTornDown { get; set; }

		#endregion

		#region ISetUppable

		public bool SetUp()
		{
			if (isSetUp)
			{
				return true;
			}

			if (setUp != null
			    && !setUp.Invoke())
			{
				return false;
			}

			isSetUp = true;
			
			OnSetUp?.Invoke(this);

			return true;
		}

		#endregion

		#region IInitializable

		public bool Initialize()
		{
			if (isInitialized)
			{
				return true;
			}

			if (!isSetUp)
			{
				SetUp();
			}

			if (initialize != null
			    && !initialize.Invoke())
			{
				return false;
			}

			isInitialized = true;
			
			OnInitialized?.Invoke(this);

			return true;
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			if (!isInitialized)
				return;

			cleanup?.Invoke();

			isInitialized = false;
			
			OnCleanedUp?.Invoke(this);
		}

		#endregion

		#region ITearDownable

		public void TearDown()
		{
			if (!isSetUp)
				return;

			if (isInitialized)
				Cleanup();
            
			tearDown?.Invoke();
			
			isSetUp = false;
			
			OnTornDown?.Invoke(this);

			
			OnSetUp = null;
            
			OnInitialized = null;

			OnCleanedUp = null;

			OnTornDown = null;
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			TearDown();
		}

		#endregion
		
		public static bool HasFlags(
			ESynchronizationFlags currentFlags,
			ESynchronizationFlags flagsToCheckAgainst)
		{
			return (currentFlags & flagsToCheckAgainst) != 0;
		}
	}
}