using System.Collections.Generic;

using UnityEngine;

namespace HereticalSolutions.IK.Unity
{
	public class GradientDescentChainComponent2D
		: AIKChainComponent2D
	{
		[Header("Gradient descent settings")]

		[SerializeField]
		protected float baseAngleStep = 5f;

		[SerializeField]
		protected float baseLearningRate = 1f;

		[SerializeField]
		protected float momentumFactor = 0.5f;

		protected override void Start()
		{
			base.Start();

			chain = new GradientDescentChain2D(
				joints,
				maxIterations,
				tolerance,
				baseAngleStep,
				baseLearningRate,
				momentumFactor);
				//matchEndEffectorOrientation);
		}
	}
}