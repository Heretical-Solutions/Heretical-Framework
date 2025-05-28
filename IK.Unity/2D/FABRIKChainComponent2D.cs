using System.Collections.Generic;

using UnityEngine;

namespace HereticalSolutions.IK.Unity
{
	public class FABRIKChainComponent2D
		: AIKChainComponent2D
	{
		protected override void Start()
		{
			base.Start();

			chain = new FABRIKChain2D(
				joints,
				maxIterations,
				tolerance);
				//matchEndEffectorOrientation);
		}
	}
}