using UnityEngine;

namespace HereticalSolutions.IK.Unity
{
	public class GradientState3D
	{
		public Vector3 previousGradient;
		public Vector3 momentum;
		
		public GradientState3D()
		{
			previousGradient = Vector3.zero;
			momentum = Vector3.zero;
		}
	}
}