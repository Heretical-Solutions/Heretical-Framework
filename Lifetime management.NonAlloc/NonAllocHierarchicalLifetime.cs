using System;
using System.Collections.Generic;

using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.NonAlloc;

using HereticalSolutions.Hierarchy;

using HereticalSolutions.ObjectPools;

namespace HereticalSolutions.LifetimeManagement.NonAlloc
{
	public class NonAllocHierarchicalLifetime
		: NonAllocLifetime,
		  IHierarchySubject<INonAllocLifetimeable>
	{
		private readonly object target;

		private readonly INonAllocLifetimeable parentLifetime;
		
		private readonly IHierarchyNode<INonAllocLifetimeable> hierarchyNode;
		
		private readonly IPool<List<IReadOnlyHierarchyNode<INonAllocLifetimeable>>> bufferPool;

		
		public NonAllocHierarchicalLifetime(
			IHierarchyNode<INonAllocLifetimeable> hierarchyNode,
			IPool<List<IReadOnlyHierarchyNode<INonAllocLifetimeable>>> bufferPool,

			INonAllocSubscribable onSetUp,
			INonAllocSubscribable onInitialized,
			INonAllocSubscribable onCleanedUp,
			INonAllocSubscribable onTornDown,

			ESynchronizationFlags setUpFlags =
				ESynchronizationFlags.SYNC_WITH_PARENT,

			ESynchronizationFlags initializeFlags =
				ESynchronizationFlags.SYNC_WITH_PARENT,

			INonAllocLifetimeable parentLifetime = null,

			Func<bool> setUp = null,
			Func<bool> initialize = null,
			Action cleanup = null,
			Action tearDown = null)
			: base(
				onSetUp,
				onInitialized,
				onCleanedUp,
				onTornDown,

				setUpFlags,
				initializeFlags,

				setUp,
				initialize,
				cleanup,
				tearDown)
		{
			this.hierarchyNode = hierarchyNode;

			this.parentLifetime = parentLifetime;

			this.bufferPool = bufferPool;

			SyncLifetimes(
				this,
				parentLifetime);

			CatchUp();
		}

		public static void SyncLifetimes(
			INonAllocLifetimeable target,
			INonAllocLifetimeable parent)
		{
			if (parent == null)
				return;

			if (target is
					IHierarchySubject<INonAllocLifetimeable> targetAsHierarchySubject
				&& parent is
					IHierarchySubject<INonAllocLifetimeable> parentAsHierarchySubject)
			{
				var targetHierarchyNode = targetAsHierarchySubject.HierarchyNode;

				var parentHierarchyNode = parentAsHierarchySubject.HierarchyNode;

				if (targetHierarchyNode.Parent != null)
				{
					throw new Exception(
						"PARENT NODE IS NOT NULL");
				}
				else
				{
					parentHierarchyNode
						.AddChild(
							targetHierarchyNode);
				}
			}
		}

		protected void CatchUp()
		{
			if (parentLifetime == null)
				return;

			if (setUpFlags.HasFlag(ESynchronizationFlags.SYNC_WITH_PARENT)
				&& !isSetUp
				&& parentLifetime.IsSetUp)
			{
				SetUp();
			}

			if (initializeFlags.HasFlag(ESynchronizationFlags.SYNC_WITH_PARENT)
				&& !isInitialized
				&& parentLifetime.IsInitialized)
			{
				Initialize();
			}
		}

		#region ISetUppable

		public override bool SetUp()
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

			if (hierarchyNode != null
				&& hierarchyNode.ChildCount > 0)
			{
				var childNodes = bufferPool.Pop();

				childNodes.AddRange(
					hierarchyNode.Children);

				foreach (var child in childNodes)
				{
					if (child != null
						&& child.Contents != null)
					{
						var childAsSetUppable = child.Contents as ISetUppable;

						if (childAsSetUppable != null)
						{
							childAsSetUppable.SetUp();
						}
					}
				}

				childNodes.Clear();

				bufferPool.Push(childNodes);
			}

			var publisher = onSetUp as IPublisherSingleArgGeneric<INonAllocLifetimeable>;

			publisher?.Publish(
				this);

			return true;
		}

		#endregion

		#region IInitializable

		public override bool Initialize()
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

			if (hierarchyNode != null
				&& hierarchyNode.ChildCount > 0)
			{
				var childNodes = bufferPool.Pop();

				childNodes.AddRange(
					hierarchyNode.Children);

				foreach (var child in childNodes)
				{
					if (child != null
						&& child.Contents != null)
					{
						var childAsInitializable = child.Contents as IInitializable;

						if (childAsInitializable != null)
						{
							childAsInitializable.Initialize();
						}
					}
				}

				childNodes.Clear();

				bufferPool.Push(childNodes);
			}

			var publisher = onInitialized as IPublisherSingleArgGeneric<INonAllocLifetimeable>;

			publisher?.Publish(
				this);

			return true;
		}

		#endregion

		#region ICleanUppable

		public override void Cleanup()
		{
			if (!isInitialized)
				return;

			cleanup?.Invoke();

			isInitialized = false;

			if (hierarchyNode != null
				&& hierarchyNode.ChildCount > 0)
			{
				var childNodes = bufferPool.Pop();

				childNodes.AddRange(
					hierarchyNode.Children);

				foreach (var child in childNodes)
				{
					if (child != null
						&& child.Contents != null)
					{
						var childAsCleanuppable = child.Contents as ICleanuppable;

						if (childAsCleanuppable != null)
						{
							childAsCleanuppable.Cleanup();
						}
					}
				}

				childNodes.Clear();

				bufferPool.Push(childNodes);
			}

			var publisher = onCleanedUp as IPublisherSingleArgGeneric<INonAllocLifetimeable>;

			publisher?.Publish(
				this);
		}

		#endregion

		#region ITearDownable

		public override void TearDown()
		{
			if (!isSetUp)
				return;

			if (isInitialized)
				Cleanup();

			tearDown?.Invoke();

			isSetUp = false;

			if (hierarchyNode != null
				&& hierarchyNode.ChildCount > 0)
			{
				var childNodes = bufferPool.Pop();

				childNodes.AddRange(
					hierarchyNode.Children);

				foreach (var child in childNodes)
				{
					if (child != null
						&& child.Contents != null)
					{
						var childAsTearDownable = child.Contents as ITearDownable;

						if (childAsTearDownable != null)
						{
							childAsTearDownable.TearDown();
						}
					}
				}

				childNodes.Clear();

				bufferPool.Push(childNodes);
			}

			hierarchyNode.RemoveAllChildren();

			if (hierarchyNode.Parent != null)
				hierarchyNode.RemoveParent();

			var publisher = onTornDown as IPublisherSingleArgGeneric<INonAllocLifetimeable>;

			publisher?.Publish(
				this);


			onSetUp.UnsubscribeAll();

			onInitialized.UnsubscribeAll();

			onCleanedUp.UnsubscribeAll();

			onTornDown.UnsubscribeAll();
		}

		#endregion

		#region IHierarchySubject

		public IHierarchyNode<INonAllocLifetimeable> HierarchyNode { get => hierarchyNode; }

		#endregion
	}
}