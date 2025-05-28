using System.Collections.Generic;

using UnityEngine;

namespace HereticalSolutions.IK.Unity
{
    public abstract class AIKChain2D
    {
        protected IKJoint2D[] joints;


        protected int maxIterations;

        protected float tolerance;

        
        // Option to make end effector match explicit target orientation (if provided)
        //protected bool matchEndEffectorOrientation = false;
        

        protected Vector2 targetPosition;

        public AIKChain2D(
			IKJoint2D[] joints,

            int maxIterations = 10,
            float tolerance = 0.01f)
			//bool matchEndEffectorOrientation = false)
        {
            this.joints = joints;

			this.maxIterations = maxIterations;

            this.tolerance = tolerance;

			//this.matchEndEffectorOrientation = matchEndEffectorOrientation;
        }

        public void Solve(
			Vector2 target)
        {
            SolveInternal(
				target,
				0f);
        }
        
        public void Solve(
			Vector2 target,
			float orientation)
        {
            SolveInternal(
				target,
				orientation);
        }
        
        private void SolveInternal(
			Vector2 target,
			float orientation)
        {
            if (joints.Length == 0)
                return;
                
            targetPosition = target;

            // Check if target is already reached with sufficient precision
            Vector2 endEffector = joints[joints.Length - 1].GetEndPosition();

            if (Vector2.Distance(
				endEffector,
				targetPosition)
				< tolerance)
            {
                // Apply orientation options after reaching target
                //if (matchEndEffectorOrientation)
				//{
				//	ApplyExplicitOrientation(
				//		orientation);
				//}

                return;
            }
            
			Solve();
            
            //if (matchEndEffectorOrientation)
			//{
			//	ApplyExplicitOrientation(
			//		orientation);
			//}
        }

		protected abstract void Solve();
        
        // Make the end effector match an explicit orientation
        protected void ApplyExplicitOrientation(
			float orientation)
        {
            if (joints.Length == 0) 
                return;
            
            // Get the last joint
            IKJoint2D lastJoint = joints[joints.Length - 1];
            
            // Set the angle directly
            lastJoint.SetAngle(orientation);
            
            // If we have more than one joint, adjust to maintain chain integrity
            if (joints.Length > 1)
            {
                // Get previous joint's end position
                Vector2 prevEnd = joints[joints.Length - 2].GetEndPosition();
                
                // Ensure last joint's position is at previous joint's end
                lastJoint.Position = prevEnd;
            }
        }

		protected void UpdateForwardKinematics(
            int startIndex)
        {
            if (startIndex >= joints.Length - 1)
                return;
                
            // Update all joints after the modified one
            for (int i = startIndex + 1; i < joints.Length; i++)
            {
                // Get the previous joint's end position
                Vector2 prevEnd = joints[i - 1].GetEndPosition();

                joints[i].Position = prevEnd;
                
                // Calculate and update the angle to maintain the chain direction
                if (i < joints.Length - 1)
                {
                    Vector2 currentToNext = joints[i + 1].Position - joints[i].Position;

                    float angle =
                        Mathf.Atan2(
                            currentToNext.y,
                            currentToNext.x)
                        * Mathf.Rad2Deg;

                    joints[i].SetAngle(angle);
                }
            }
        }
    }
}