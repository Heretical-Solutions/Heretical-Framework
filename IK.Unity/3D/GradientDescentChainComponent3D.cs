using System.Collections.Generic;

using UnityEngine;

namespace HereticalSolutions.IK.Unity
{
	public class GradientDescentChainComponent3D
		: AIKChainComponent3D
	{
		[Header("Gradient descent settings")]

		[SerializeField]
		protected float baseRotationStep = 5f;

		[SerializeField]
		protected float baseLearningRate = 1f;

		[SerializeField]
		protected float momentumFactor = 0.5f;

		protected override void Start()
		{
			base.Start();

			chain = new GradientDescentChain3D(
				joints,
				maxIterations,
				tolerance,
				baseRotationStep,
				baseLearningRate,
				momentumFactor);
				//matchEndEffectorOrientation);
		}
	}
}