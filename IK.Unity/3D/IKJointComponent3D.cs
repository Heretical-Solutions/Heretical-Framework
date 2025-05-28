using UnityEngine;

namespace HereticalSolutions.IK.Unity
{
	public class IKJointComponent3D : MonoBehaviour
	{
		[SerializeField]
		private float length = 1f;

		[SerializeField]
		private Vector3 minRotation = new Vector3(-180f, -180f, -180f);

		[SerializeField]
		private Vector3 maxRotation = new Vector3(180f, 180f, 180f);

		[Header("Joint Orientation")]
		[Tooltip("Local forward direction of the joint. Default is forward (0,0,1)")]
		[SerializeField]
		private Vector3 forwardDirection = Vector3.forward;

		[Tooltip("Local up direction of the joint. Default is up (0,1,0)")]
		[SerializeField]
		private Vector3 upDirection = Vector3.up;

		//[SerializeField]
		//private bool logErrors = true;

		private IKJoint3D joint;

		private Quaternion BaseTransformForwardRotation;

		public void Initialize()
		{
			// Ensure directions are normalized and perpendicular
			if (forwardDirection.sqrMagnitude < 0.001f)
				forwardDirection = Vector3.forward;
			else
				forwardDirection = forwardDirection.normalized;

			// Project up direction onto plane perpendicular to forward
			upDirection = Vector3.ProjectOnPlane(
				upDirection.sqrMagnitude < 0.001f
					? Vector3.up
					: upDirection,
				forwardDirection).normalized;

			BaseTransformForwardRotation =
				Quaternion.Inverse(
					Quaternion.identity)
				* transform.rotation;

			joint = new IKJoint3D();

			joint.Initialize(
				transform.position,
				forwardDirection,
				upDirection,

				length,
				
				minRotation,
				maxRotation);

			GetDataFromTransform(); //(true);
		}

		public IKJoint3D GetJoint()
		{
			return joint;
		}

		public void GetDataFromTransform()
			//bool initialization = false)
		{
			if (joint == null)
				return;

			//var previousPosition = joint.Position;
			//var previousRotation = joint.Rotation;
			//var previousTotalRotation = joint.TotalRotation;

			joint.Position = transform.position;
			
			joint.Rotation =
				Quaternion.Inverse(BaseTransformForwardRotation)
			 	* transform.rotation;

			joint.TotalRotation =
				joint.Rotation
				* joint.BaseRotation;


			//var positionError = Vector3.Distance(
			//	joint.Position,
			//	previousPosition);
			//
			//var rotationError = Quaternion.Angle(
			//	joint.Rotation,
			//	previousRotation);
			//
			//if (rotationError > 180f)
			//	rotationError = rotationError -= 360f;
			//
			//if (rotationError < -180f)
			//	rotationError = rotationError += 360f;
			//
			//var totalRotationError = Quaternion.Angle(
			//	joint.TotalRotation,
			//	previousTotalRotation);
			//
			//if (totalRotationError > 180f)
			//	totalRotationError = totalRotationError -= 360f;
			//
			//if (totalRotationError < -180f)
			//	totalRotationError = totalRotationError += 360f;
			//
			//if (logErrors
			//	&& !initialization)
			//{
			//	bool positionErrorSustantial = positionError > 0.01f;
			//	bool rotationErrorSustantial = Mathf.Abs(rotationError) > 0.01f;
			//	bool totalRotationErrorSustantial = Mathf.Abs(totalRotationError) > 0.01f;
			//
			//	UnityEngine.Debug.Log(
			//		$"Position Error: {(positionErrorSustantial ? "<color=red>" : "")} {positionError} {(positionErrorSustantial ? "</color>" : "")}, Angle Error: {(rotationErrorSustantial ? "<color=red>" : "")} {rotationError} {(rotationErrorSustantial ? "</color>" : "")}, Total Angle Error: {(totalRotationErrorSustantial ? "<color=red>" : "")} {totalRotationError} {(totalRotationErrorSustantial ? "</color>" : "")}",
			//		gameObject);
			//}
		}

		public void SetDataToTransform()
		{
			if (joint == null)
				return;

			transform.position = joint.Position;

			transform.rotation =
				joint.Rotation
				* BaseTransformForwardRotation;
		}

		public void SetDataToTransform(
			float interpolationValue)
		{
			if (joint == null)
				return;

			transform.position = Vector3.Lerp(
				transform.position,
				joint.Position,
				interpolationValue);

			transform.rotation = Quaternion.Slerp(
				transform.rotation,
				joint.Rotation
					* BaseTransformForwardRotation,
				interpolationValue);
		}

		private void OnDrawGizmos()
		{
			if (joint == null)
				return;

			var previousColor = Gizmos.color;

			Gizmos.color = Color.white;
			Gizmos.DrawLine(
				joint.Position,
				joint.GetEndPosition());

			Gizmos.color = Color.green;
			Gizmos.DrawSphere(
				joint.Position,
				0.02f);

			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(
				joint.GetEndPosition(),
				0.02f);


			Gizmos.color = previousColor;
		}
	}
}