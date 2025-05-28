using System.Collections.Generic;

using UnityEngine;

namespace HereticalSolutions.IK.Unity
{
    public abstract class AIKChain3D
    {
        protected IKJoint3D[] joints;


        protected int maxIterations;

        protected float tolerance;

        
        // Option to make end effector match explicit target orientation (if provided)
        //protected bool matchEndEffectorOrientation = false;
        

        protected Vector3 targetPosition;


        public AIKChain3D(
			IKJoint3D[] joints,

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
			Vector3 target)
        {
            SolveInternal(
				target,
				Quaternion.identity);
        }
        
        public void Solve(
			Vector3 target,
			Quaternion orientation)
        {
            SolveInternal(
				target,
				orientation);
        }
        
        private void SolveInternal(
			Vector3 target,
			Quaternion orientation)
        {
            if (joints.Length == 0)
                return;
                
            targetPosition = target;
            
            // Check if target is already reached with sufficient precision
            Vector3 endEffector = joints[joints.Length - 1].GetEndPosition();

            if (Vector3.Distance(
					endEffector,
					targetPosition)
				< tolerance)
            {
                // Apply target orientation to end effector if requested
                //if (matchEndEffectorOrientation)
                //{
                //    ApplyTargetOrientationToEndEffector(
				//		orientation);
                //}

                return; // Already at target, no need to solve
            }
            
			Solve();
            
            // Apply target orientation to end effector if requested
            //if (matchEndEffectorOrientation)
            //{
            //    ApplyTargetOrientationToEndEffector(
			//		orientation);
            //}
        }
        
		protected abstract void Solve();
        
        protected void UpdateForwardKinematics(int startIndex)
        {
            if (startIndex >= joints.Length - 1)
                return;
                
            // Update all joints after the modified one
            for (int i = startIndex + 1; i < joints.Length; i++)
            {
                // Get the previous joint's end position
                Vector3 prevEnd = joints[i - 1].GetEndPosition();
                joints[i].Position = prevEnd;
                
                // Calculate and update the rotation to maintain the chain direction
                if (i < joints.Length - 1)
                {
                    Vector3 toNext = joints[i + 1].Position - joints[i].Position;
                    joints[i].SetRotation(Quaternion.LookRotation(toNext));
                }
            }
        }
        
        // Helper method to apply target orientation to the end effector
        private void ApplyTargetOrientationToEndEffector(
			Quaternion orientation)
        {
            if (joints.Length == 0)
                return;
            
            // Get the last joint
            IKJoint3D lastJoint = joints[joints.Length - 1];
            
            // Store original position and rotation
            Vector3 originalPosition = lastJoint.Position;
            Quaternion originalRotation = lastJoint.TotalRotation;
            
            // Set the rotation directly
            lastJoint.SetRotation(orientation);
            
            // If we have more than one joint, we need to adjust to maintain chain integrity
            if (joints.Length > 1)
            {
                // Get previous joint's end position
                Vector3 prevEnd = joints[joints.Length - 2].GetEndPosition();
                
                // Ensure last joint's position is at previous joint's end
                lastJoint.Position = prevEnd;
                
                // Get the new end position after orientation change
                Vector3 newEndPos = lastJoint.GetEndPosition();
                
                // Calculate how far we are from the target
                float distanceToTarget = Vector3.Distance(newEndPos, targetPosition);
                
                // If we're not close enough to the target
                if (distanceToTarget > tolerance)
                {
                    // We could try to adjust position, but that might break the chain
                    // Instead, we'll make a small compromise on the exact orientation
                    // by rotating slightly to get closer to the target
                    
                    // Calculate direction to target from joint position
                    Vector3 toTarget = targetPosition - lastJoint.Position;
                    toTarget.Normalize();
                    
                    // Calculate a rotation that would face the target
                    Quaternion lookAtTarget = Quaternion.LookRotation(toTarget);
                    
                    // Blend between perfect orientation and target-facing orientation
                    // The closer we are to the target, the more we prioritize the desired orientation
                    float blendFactor = Mathf.Clamp01(tolerance / (distanceToTarget + 0.0001f));
                    Quaternion blendedRotation = Quaternion.Slerp(lookAtTarget, orientation, blendFactor);
                    
                    // Apply the blended rotation
                    lastJoint.SetRotation(blendedRotation);
                }
            }
        }
    }
}