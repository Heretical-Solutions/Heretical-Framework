using System;

using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.NonAlloc;

namespace HereticalSolutions.LifetimeManagement.NonAlloc
{
	public class NonAllocLifetime
		: INonAllocLifetimeable,
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


		protected readonly INonAllocSubscribable onSetUp;

		protected readonly INonAllocSubscribable onInitialized;

		protected readonly INonAllocSubscribable onCleanedUp;

		protected readonly INonAllocSubscribable onTornDown;


		protected ESynchronizationFlags setUpFlags;

		protected ESynchronizationFlags initializeFlags;


		protected bool isInitialized = false;

		protected bool isSetUp = false;


		public NonAllocLifetime(
			INonAllocSubscribable onSetUp,
			INonAllocSubscribable onInitialized,
			INonAllocSubscribable onCleanedUp,
			INonAllocSubscribable onTornDown,

			ESynchronizationFlags setUpFlags =
				ESynchronizationFlags.SYNC_WITH_PARENT,

			ESynchronizationFlags initializeFlags =
				ESynchronizationFlags.SYNC_WITH_PARENT,

			Func<bool> setUp = null,
			Func<bool> initialize = null,
			Action cleanup = null,
			Action tearDown = null)
		{
			this.onSetUp = onSetUp;

			this.onInitialized = onInitialized;

			this.onCleanedUp = onCleanedUp;

			this.onTornDown = onTornDown;


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

		public INonAllocSubscribable OnSetUp => onSetUp;

		public ESynchronizationFlags SetUpFlags => setUpFlags;

		public INonAllocSubscribable OnInitialized => onInitialized;

		public ESynchronizationFlags InitializeFlags => initializeFlags;

		public INonAllocSubscribable OnCleanedUp => onCleanedUp;

		public INonAllocSubscribable OnTornDown => onTornDown;

		#endregion

		#region ISetUppable

		public virtual bool SetUp()
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

			var publisher = onSetUp as IPublisherSingleArgGeneric<INonAllocLifetimeable>;

			publisher?.Publish(
				this);

			return true;
		}

		#endregion

		#region IInitializable

		public virtual bool Initialize()
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

			var publisher = onInitialized as IPublisherSingleArgGeneric<INonAllocLifetimeable>;

			publisher?.Publish(
				this);

			return true;
		}

		#endregion

		#region ICleanUppable

		public virtual void Cleanup()
		{
			if (!isInitialized)
				return;

			cleanup?.Invoke();

			isInitialized = false;

			var publisher = onCleanedUp as IPublisherSingleArgGeneric<INonAllocLifetimeable>;

			publisher?.Publish(
				this);
		}

		#endregion

		#region ITearDownable

		public virtual void TearDown()
		{
			if (!isSetUp)
				return;

			if (isInitialized)
				Cleanup();

			tearDown?.Invoke();

			isSetUp = false;

			var publisher = onTornDown as IPublisherSingleArgGeneric<INonAllocLifetimeable>;

			publisher?.Publish(
				this);


			onSetUp.UnsubscribeAll();

			onInitialized.UnsubscribeAll();

			onCleanedUp.UnsubscribeAll();

			onTornDown.UnsubscribeAll();
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