using System.Collections.Generic;

using UnityEngine;

namespace HereticalSolutions.IK.Unity
{
	public class GaussSeidelChainComponent3D
		: AIKChainComponent3D
	{
		[Header("Gauss-Seidel settings")]

		[SerializeField]
		protected int linearSolverIterations = 10;

		[SerializeField]
		protected float baseDampingFactor = 0.05f; // Reduced damping for more direct movement

		[SerializeField]
		protected float rotationStepSize = 5f;

		protected override void Start()
		{
			base.Start();
			
			chain = new GaussSeidelChain3D(
				joints,
				maxIterations,
				tolerance,
				linearSolverIterations,
				baseDampingFactor,
				rotationStepSize);
				//matchEndEffectorOrientation);
		}
	}
}