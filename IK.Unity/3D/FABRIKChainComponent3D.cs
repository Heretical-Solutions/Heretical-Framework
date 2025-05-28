using System.Collections.Generic;

using UnityEngine;

namespace HereticalSolutions.IK.Unity
{
	public class FABRIKChainComponent3D
		: AIKChainComponent3D
	{
		protected override void Start()
		{
			base.Start();

			chain = new FABRIKChain3D(
				joints,
				maxIterations,
				tolerance);
				//matchEndEffectorOrientation);
		}
	}
}