using System.Collections.Generic;

using HereticalSolutions.Synchronization;

using UnityEngine;

namespace HereticalSolutions.IK.Unity
{
	public abstract class AIKChainComponent2D
		: MonoBehaviour,
		  ISynchronizable
	{
		[Header("Main settings")]

		[SerializeField]
		protected Transform IKTarget;

		[SerializeField]
		private IKJointComponent2D[] jointComponents;

		[SerializeField]
		protected bool lateUpdate = true;

		[Header("Precision settings")]

		[SerializeField]
		protected int maxIterations = 10;

		[SerializeField]
		protected float tolerance = 0.01f;

		//[SerializeField]
		//protected bool matchEndEffectorOrientation = false;

		[Header("Blending settings")]

		[SerializeField]
		protected bool blending = false;

		[SerializeField]
		protected float blendingFactor = 1f;

		[Header("Simulation settings")]

		[SerializeField]
		protected bool getDataFromTransformBeforeSolving = true;

		[SerializeField]
		protected bool setDataToTransformAfterSolving = true;

		[SerializeField]
		protected bool simulate = true;


		protected AIKChain2D chain;

		protected IKJoint2D[] joints;


		protected virtual void Start()
		{
			List<IKJoint2D> jointsList = new List<IKJoint2D>();

			// Get all joints in order
			foreach (IKJointComponent2D jointComponent in jointComponents)
			{
				if (jointComponent != null)
				{
					jointComponent.Initialize();

					jointsList.Add(
						jointComponent.GetJoint());
				}
			}

			joints = jointsList.ToArray();
		}

		public void Synchronize()
		{
			Simulate();
		}

		protected void LateUpdate()
		{
			if (lateUpdate)
			{
				Simulate();
			}
		}

		protected void Simulate()
		{
			if (IKTarget == null
				|| chain == null)
				return;

			if (getDataFromTransformBeforeSolving)
			{
				foreach (var joint in jointComponents)
				{
					joint.GetDataFromTransform();
				}
			}

			if (simulate)
			{
				//if (matchEndEffectorOrientation)
				//	chain.Solve(
				//		IKTarget.position,
				//		IKTarget.rotation.eulerAngles.z);
				//else
					chain.Solve(
						IKTarget.position);
			}

			if (setDataToTransformAfterSolving)
			{
				foreach (var joint in jointComponents)
				{
					if (blending)
					{
						joint.SetDataToTransform(
							blendingFactor);
					}
					else
					{
						joint.SetDataToTransform();
					}
				}
			}
		}

		private void OnDrawGizmos()
		{
			if (chain == null)
				return;

			var previousColor = Gizmos.color;

			var lastJoint = joints[jointComponents.Length - 1];

			Gizmos.color = Color.red;
			Gizmos.DrawLine(
				new Vector3(
					lastJoint.GetEndPosition().x,
					lastJoint.GetEndPosition().y,
					transform.position.z),
				IKTarget.position);


			Gizmos.color = previousColor;
		}
	}
}