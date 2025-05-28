using System.Collections.Generic;

using UnityEngine;

namespace HereticalSolutions.IK.Unity
{
    public class FABRIKChain2D
		: AIKChain2D
    {
        public FABRIKChain2D(
            IKJoint2D[] joints,

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
                Vector2 currentTarget = targetPosition;
                
                for (int i = joints.Length - 1; i >= 0; i--)
                {
                    Vector2 jointToEnd = currentTarget - joints[i].Position;
                    
                    // Calculate target angle relative to world right
                    float targetAngle = Vector2.SignedAngle(
                        Vector2.right,
                        jointToEnd);
                    
                    joints[i].SetAngle(targetAngle);
                    
                    if (i > 0)
                    {
                        joints[i].Position = 
                            currentTarget
                            - (joints[i].GetEndPosition() - joints[i].Position);

                        currentTarget = joints[i].Position;
                    }
                }
                
                // Backward reaching - move from root to end effector
                Vector2 basePos = joints[0].Position;
                
                for (int i = 0; i < joints.Length; i++)
                {
                    if (i == 0)
                    {
                        joints[i].Position = basePos;
                    }
                    else
                    {
                        Vector2 prevEnd = joints[i-1].GetEndPosition();

                        joints[i].Position = prevEnd;
                        
                        // Calculate target direction
                        Vector2 targetDir;

                        if (i < joints.Length - 1)
                        {
                            targetDir = joints[i+1].Position - joints[i].Position;
                        }
                        else
                        {
                            targetDir = targetPosition - joints[i].Position;
                        }
                        
                        // Calculate target angle relative to world right
                        float targetAngle = Vector2.SignedAngle(
                            Vector2.right,
                            targetDir);
                        
                        joints[i].SetAngle(targetAngle);
                    }
                }
                
                if (Vector2.Distance(
                        joints[joints.Length - 1].GetEndPosition(),
                        targetPosition)
                    < tolerance)
                    break;
            }
        }
    }
}