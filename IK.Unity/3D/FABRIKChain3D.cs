using System.Collections.Generic;

using UnityEngine;

namespace HereticalSolutions.IK.Unity
{
    public class FABRIKChain3D
		: AIKChain3D
    {
        public FABRIKChain3D(
            IKJoint3D[] joints,

            int maxIterations = 10,
            float tolerance = 0.01f)
			//bool matchEndEffectorOrientation = false)
			: base(
				joints,
				maxIterations,
				tolerance)
				//matchEndEffectorOrientation)
        {
        }
        
        protected override void Solve()
        {
			for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                // Forward reaching - move from end effector to root
                Vector3 currentTarget = targetPosition;
                
                for (int i = joints.Length - 1; i >= 0; i--)
                {
                    Vector3 jointToEnd = currentTarget - joints[i].Position;
                    
                    // Calculate desired orientation to target
                    Quaternion targetOrientation = Quaternion.LookRotation(jointToEnd);
                    
                    joints[i].SetRotation(targetOrientation);
                    
                    if (i > 0)
                    {
                        joints[i].Position =
                            currentTarget
                            - (joints[i].GetEndPosition() - joints[i].Position);
                            
                        currentTarget = joints[i].Position;
                    }
                }
                
                // Backward reaching - move from root to end effector
                Vector3 basePos = joints[0].Position;
                
                for (int i = 0; i < joints.Length; i++)
                {
                    if (i == 0)
                    {
                        joints[i].Position = basePos;
                    }
                    else
                    {
                        Vector3 prevEnd = joints[i-1].GetEndPosition();

                        joints[i].Position = prevEnd;
                        
                        // Calculate target direction
                        Vector3 targetDir;
                        if (i < joints.Length - 1)
                        {
                            targetDir = joints[i+1].Position - joints[i].Position;
                        }
                        else
                        {
                            targetDir = targetPosition - joints[i].Position;
                        }
                        
                        // Calculate target orientation
                        Quaternion targetOrientation = Quaternion.LookRotation(targetDir);
                        
                        joints[i].SetRotation(targetOrientation);
                    }
                }
                
                if (Vector3.Distance(
                        joints[joints.Length - 1].GetEndPosition(),
                        targetPosition)
                    < tolerance)
                    break;
            }
        }
    }
}