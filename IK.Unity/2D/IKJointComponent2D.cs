using UnityEngine;

namespace HereticalSolutions.IK.Unity
{
	public class IKJointComponent2D
		: MonoBehaviour
	{
		[SerializeField]
		private float length = 1f;

		[SerializeField]
		private float minAngle = -180f;

		[SerializeField]
		private float maxAngle = 180f;
		
		[Header("Joint Orientation")]
		[Tooltip("Local forward direction of the joint. Default is right (1,0)")]
		[SerializeField]
		private Vector2 forwardDirection = Vector2.right;

		[SerializeField]
		private bool projectRotationOntoXYPlane = false;

		//[SerializeField]
		//private bool logErrors = true;

		private IKJoint2D joint;

		private float BaseTransformForwardAngle;

		private Plane xyPlane;

		public void Initialize()
		{
			xyPlane = new Plane(
				Vector3.forward,
				Vector3.zero);

			// Ensure forward direction is normalized
			if (forwardDirection.sqrMagnitude < 0.001f)
				forwardDirection = Vector2.right;
			else
				forwardDirection = forwardDirection.normalized;

			if (projectRotationOntoXYPlane)
			{
				BaseTransformForwardAngle = Vector2.SignedAngle(
					Vector2.right,
					Vector3.ProjectOnPlane(
						transform.forward,
						xyPlane.normal).normalized);
			}
			else
			{
				BaseTransformForwardAngle = Vector2.SignedAngle(
					Vector2.right,
					transform.right);
			}
				
			joint = new IKJoint2D();
			
			joint.Initialize(
				transform.position,
				forwardDirection,

				length,
				
				minAngle,
				maxAngle);

			GetDataFromTransform(); //(true);
		}

		public IKJoint2D GetJoint()
		{
			return joint;
		}

		public void GetDataFromTransform()
			//bool initialization = false)
		{
			if (joint == null)
				return;

			//var previousPosition = joint.Position;
			//var previousAngle = joint.Angle;
			//var previousTotalAngle = joint.TotalAngle;

			joint.Position = new Vector2(
				transform.position.x,
				transform.position.y);
			
			if (projectRotationOntoXYPlane)
			{
				var rotation = transform.rotation * Vector3.forward;

				var projectedRotation = Vector3.ProjectOnPlane(
					rotation,
					xyPlane.normal).normalized;

				joint.Angle = Vector2.SignedAngle(
					Vector2.right,
					projectedRotation) - BaseTransformForwardAngle;
			}
			else
			{
				joint.Angle = transform.rotation.eulerAngles.z - BaseTransformForwardAngle;
			}

			if (joint.Angle > 180f)
				joint.Angle -= 360f;
			else if (joint.Angle < -180f)
				joint.Angle += 360f;

			joint.TotalAngle = joint.Angle + joint.BaseForwardAngle;

			if (joint.TotalAngle > 180f)
				joint.TotalAngle -= 360f;
			else if (joint.TotalAngle < -180f)
				joint.TotalAngle += 360f;

			//var positionError = Vector2.Distance(
			//	joint.Position,
			//	previousPosition);
			//
			//var angleError = Mathf.Abs(
			//	joint.Angle - previousAngle);
			//
			//if (angleError > 180f)
			//	angleError = angleError -= 360f;
			//
			//if (angleError < -180f)
			//	angleError = angleError += 360f;
			//
			//var totalAngleError = Mathf.Abs(
			//	joint.TotalAngle - previousTotalAngle);
			//
			//if (totalAngleError > 180f)
			//	totalAngleError = totalAngleError -= 360f;
			//
			//if (totalAngleError < -180f)
			//	totalAngleError = totalAngleError += 360f;
			//
			//if (logErrors
			//	&& !initialization)
			//{
			//	bool positionErrorSustantial = positionError > 0.01f;
			//	bool angleErrorSustantial = Mathf.Abs(angleError) > 0.01f;
			//	bool totalAngleErrorSustantial = Mathf.Abs(totalAngleError) > 0.01f;
			//
			//	UnityEngine.Debug.Log(
			//		$"Position Error: {(positionErrorSustantial ? "<color=red>" : "")} {positionError} {(positionErrorSustantial ? "</color>" : "")}, Angle Error: {(angleErrorSustantial ? "<color=red>" : "")} {angleError} {(angleErrorSustantial ? "</color>" : "")}, Total Angle Error: {(totalAngleErrorSustantial ? "<color=red>" : "")} {totalAngleError} {(totalAngleErrorSustantial ? "</color>" : "")}",
			//		gameObject);
			//}
		}

		public void SetDataToTransform()
		{
			if (joint == null)
				return;

			transform.position = new Vector3(
				joint.Position.x,
				joint.Position.y,
				transform.position.z); // Keep the original Z position
			
			if (projectRotationOntoXYPlane)
			{
				var rotation = transform.rotation * Vector3.forward;

				UnityEngine.Debug.DrawLine(
					transform.position,
					transform.position + rotation,
					Color.cyan);

				var projectedRotation = Vector3.ProjectOnPlane(
					rotation,
					xyPlane.normal).normalized;

				UnityEngine.Debug.DrawLine(
					transform.position,
					transform.position + projectedRotation,
					Color.magenta);

				var currentAngle = Vector2.SignedAngle(
					Vector2.right,
					projectedRotation);

				var angleDifference = joint.Angle + BaseTransformForwardAngle - currentAngle;

				if (angleDifference > 180f)
					angleDifference -= 360f;
				else if (angleDifference < -180f)
					angleDifference += 360f;

				transform.rotation = Quaternion.Euler(
					0,
					0,
					angleDifference)
					* transform.rotation;
			}
			else
			{
				transform.rotation = Quaternion.Euler(
					0,
					0,
					joint.Angle + BaseTransformForwardAngle);
			}
		}

		public void SetDataToTransform(
			float interpolationValue)
		{
			if (joint == null)
				return;

			transform.position = Vector3.Lerp(
				transform.position,
				new Vector3(
					joint.Position.x,
					joint.Position.y,
					transform.position.z), // Keep the original Z position
				interpolationValue);

			if (projectRotationOntoXYPlane)
			{
				var rotation = transform.rotation * Vector3.forward;

				UnityEngine.Debug.DrawLine(
					transform.position,
					transform.position + rotation,
					Color.cyan);

				var projectedRotation = Vector3.ProjectOnPlane(
					rotation,
					xyPlane.normal).normalized;

				UnityEngine.Debug.DrawLine(
					transform.position,
					transform.position + projectedRotation,
					Color.magenta);

				var currentAngle = Vector2.SignedAngle(
					Vector2.right,
					projectedRotation);

				var angleDifference = joint.Angle + BaseTransformForwardAngle - currentAngle;

				if (angleDifference > 180f)
					angleDifference -= 360f;
				else if (angleDifference < -180f)
					angleDifference += 360f;

				var targetRotation = Quaternion.Euler(
					0,
					0,
					angleDifference)
					* transform.rotation;

				transform.rotation = Quaternion.Lerp(
					transform.rotation,
					targetRotation,
					interpolationValue);
			}
			else
			{
				transform.rotation = Quaternion.Lerp(
					transform.rotation,
					Quaternion.Euler(
						0,
						0,
						joint.Angle + BaseTransformForwardAngle),
					interpolationValue);
			}
		}

		private void OnDrawGizmos()
		{
			if (joint == null)
				return;

			var previousColor = Gizmos.color;

			Gizmos.color = Color.white;
			Gizmos.DrawLine(
				new Vector3(
					joint.Position.x,
					joint.Position.y,
					transform.position.z),
				new Vector3(
					joint.GetEndPosition().x,
					joint.GetEndPosition().y,
					transform.position.z));

			Gizmos.color = Color.green;
			Gizmos.DrawSphere(
				new Vector3(
					joint.Position.x,
					joint.Position.y,
					transform.position.z),
				0.02f);

			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(
				new Vector3(
					joint.GetEndPosition().x,
					joint.GetEndPosition().y,
					transform.position.z),
				0.02f);


			Gizmos.color = previousColor;
		}
	}
}