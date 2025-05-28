using System.Collections.Generic;

using UnityEngine;

namespace HereticalSolutions.IK.Unity
{
	public class GaussSeidelChainComponent2D
		: AIKChainComponent2D
	{
		[Header("Gauss-Seidel settings")]

		[SerializeField]
		protected int linearSolverIterations = 10;

		[SerializeField]
		protected float baseDampingFactor = 0.05f; // Reduced damping for more direct movement

		[SerializeField]
		protected float angleStepSize = 5f;

		protected override void Start()
		{
			base.Start();
			
			chain = new GaussSeidelChain2D(
				joints,
				maxIterations,
				tolerance,
				linearSolverIterations,
				baseDampingFactor,
				angleStepSize);
				//matchEndEffectorOrientation);
		}
	}
}