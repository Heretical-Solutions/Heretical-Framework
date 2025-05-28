using System.Collections.Generic;

using UnityEngine;

namespace HereticalSolutions.IK.Unity
{
    public class GaussSeidelChain3D
		: AIKChain3D
    {
		private float baseDampingFactor;
		private float rotationStepSize;

		private Vector3[] originalPositions;
		private Quaternion[] originalRotations;

		private int linearSolverIterations;
		private int dimensions = 3; // 3D has x,y,z
		private int numJoints;
		private int degreesOfFreedom;
		private float[,] jacobian;
		private float[,] jacobianT;
		private float[,] jtj;
		private float[] jte;
		private float[] deltaRotations;
		private Vector3[] axes;

        public GaussSeidelChain3D(
            IKJoint3D[] joints,

            int maxIterations = 10,
            float tolerance = 0.01f,

			int linearSolverIterations = 10,
			float baseDampingFactor = 0.05f, // Reduced damping for more direct movement
			float rotationStepSize = 5f)
			//bool matchEndEffectorOrientation = false)
			: base(
				joints,
				maxIterations,
				tolerance)
				//matchEndEffectorOrientation)
        {
			this.baseDampingFactor = baseDampingFactor;
			this.rotationStepSize = rotationStepSize;
			this.linearSolverIterations = linearSolverIterations;

			originalPositions = new Vector3[joints.Length];
            originalRotations = new Quaternion[joints.Length];

            // Build the Jacobian matrix - one row per dimension (x,y,z), one column per joint*axis
			numJoints = joints.Length;
            degreesOfFreedom = numJoints * 3; // Each joint has 3 rotational DOFs in 3D
			jacobian = new float[dimensions, degreesOfFreedom];
			jacobianT = new float[degreesOfFreedom, dimensions];
			jtj = new float[degreesOfFreedom, degreesOfFreedom];
			jte = new float[degreesOfFreedom];
			deltaRotations = new float[degreesOfFreedom];
			axes = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero };
        }
        
        protected override void Solve()
        {
			float dampingFactor = baseDampingFactor; // Reduced damping for more direct movement
            
            // Get initial state and error
            Vector3 endEffector = joints[joints.Length - 1].GetEndPosition();
            Vector3 errorVector = targetPosition - endEffector;
            float currentError = errorVector.magnitude;
            
            // Skip if already at target
            if (currentError < tolerance)
                return;
            
            // Store original state before any changes
            for (int j = 0; j < joints.Length; j++)
            {
                originalPositions[j] = joints[j].Position;
                originalRotations[j] = joints[j].TotalRotation;
            }

            // Use outer iterations for progressive refinement 
            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                // Update error for this iteration
                endEffector = joints[joints.Length - 1].GetEndPosition();
                errorVector = targetPosition - endEffector;
                currentError = errorVector.magnitude;
                
                if (currentError < tolerance)
                    break;
                
                // Build the Jacobian matrix - one row per dimension (x,y,z), one column per joint*axis
                
                // Calculate the Jacobian for each joint and axis
                for (int jointIndex = 0; jointIndex < numJoints; jointIndex++)
                {
                    Quaternion originalRotation = joints[jointIndex].TotalRotation;
                    
                    // Get the rotation axes for this joint
                    Vector3 baseForward = originalRotation * Vector3.forward;
                    Vector3 baseUp = originalRotation * Vector3.up;
                    Vector3 baseRight = Vector3.Cross(baseUp, baseForward);
                    //Vector3[] axes = new Vector3[] { baseRight, baseUp, baseForward };
					axes[0] = baseRight;
					axes[1] = baseUp;
					axes[2] = baseForward;
                    
                    // Calculate Jacobian columns for each axis of this joint
                    for (int axis = 0; axis < 3; axis++)
                    {
                        int dofIndex = jointIndex * 3 + axis;
                        
                        // Sample positive rotation
                        Quaternion rotPlus = Quaternion.AngleAxis(rotationStepSize, axes[axis]) * originalRotation;
                        joints[jointIndex].SetRotation(rotPlus);
                        UpdateForwardKinematics(jointIndex);
                        Vector3 positivePos = joints[joints.Length - 1].GetEndPosition();
                        
                        // Restore state between samples
                        for (int j = 0; j < joints.Length; j++)
                        {
                            joints[j].Position = originalPositions[j];
                            joints[j].SetRotation(originalRotations[j]);
                        }
                        
                        // Sample negative rotation
                        Quaternion rotMinus = Quaternion.AngleAxis(-rotationStepSize, axes[axis]) * originalRotation;
                        joints[jointIndex].SetRotation(rotMinus);
                        UpdateForwardKinematics(jointIndex);
                        Vector3 negativePos = joints[joints.Length - 1].GetEndPosition();
                        
                        // Restore state after sampling
                        for (int j = 0; j < joints.Length; j++)
                        {
                            joints[j].Position = originalPositions[j];
                            joints[j].SetRotation(originalRotations[j]);
                        }
                        
                        // Calculate partial derivatives (Jacobian columns)
                        Vector3 derivative = (positivePos - negativePos) / (2f * rotationStepSize);
                        jacobian[0, dofIndex] = derivative.x; // x-component
                        jacobian[1, dofIndex] = derivative.y; // y-component
                        jacobian[2, dofIndex] = derivative.z; // z-component
                    }
                }
                
                // Calculate J^T (Jacobian transpose)
                for (int j = 0; j < degreesOfFreedom; j++)
                {
                    for (int d = 0; d < dimensions; d++)
                    {
                        jacobianT[j, d] = jacobian[d, j];
                    }
                }
                
                // Calculate J^T * J (square matrix, degreesOfFreedom x degreesOfFreedom)
                for (int row = 0; row < degreesOfFreedom; row++)
                {
                    for (int col = 0; col < degreesOfFreedom; col++)
                    {
                        jtj[row, col] = 0;
                        for (int k = 0; k < dimensions; k++)
                        {
                            jtj[row, col] += jacobianT[row, k] * jacobian[k, col];
                        }
                        
                        // Add Levenberg-Marquardt damping to diagonal
                        if (row == col)
                        {
                            jtj[row, col] += dampingFactor;
                        }
                    }
                }
                
                // Calculate J^T * error (vector, degreesOfFreedom x 1)
                for (int j = 0; j < degreesOfFreedom; j++)
                {
                    jte[j] = jacobianT[j, 0] * errorVector.x + 
                             jacobianT[j, 1] * errorVector.y + 
                             jacobianT[j, 2] * errorVector.z;
                }
                
                // Solve the linear system (J^T * J) * delta = J^T * error
                SolveLinearSystem(jtj, jte, deltaRotations, 10); // Use fixed iterations for linear solver
                
                // Apply all rotation adjustments simultaneously
                for (int jointIndex = 0; jointIndex < numJoints; jointIndex++)
                {
                    Quaternion originalRotation = joints[jointIndex].TotalRotation;
                    
                    // Get the rotation axes for this joint
                    Vector3 baseForward = originalRotation * Vector3.forward;
                    Vector3 baseUp = originalRotation * Vector3.up;
                    Vector3 baseRight = Vector3.Cross(baseUp, baseForward);
                    //Vector3[] axes = new Vector3[] { baseRight, baseUp, baseForward };
					axes[0] = baseRight;
					axes[1] = baseUp;
					axes[2] = baseForward;
                    
                    // Apply rotation for each axis
                    Quaternion newRotation = originalRotation;
                    
                    // Apply larger adjustments to base joints (ones closer to root)
                    // This promotes more arm-like reaching rather than snake-like movement
                    float jointFactor = 1.0f + (numJoints - jointIndex - 1) * 0.2f; // Boost factor for base joints
                    
                    for (int axis = 0; axis < 3; axis++)
                    {
                        int dofIndex = jointIndex * 3 + axis;
                        
                        // Apply the joint factor to make base joints rotate more
                        float deltaAngle = deltaRotations[dofIndex] * jointFactor;
                        
                        // Clamp adjustments to avoid extreme movements
                        deltaAngle = Mathf.Clamp(deltaAngle, -30f, 30f);
                        
                        // Apply rotation around this axis
                        newRotation = Quaternion.AngleAxis(deltaAngle, axes[axis]) * newRotation;
                    }
                    
                    // Set the new rotation
                    joints[jointIndex].SetRotation(newRotation);
                }
                
                // Update all positions
                Vector3 basePos = joints[0].Position;
                for (int i = 1; i < joints.Length; i++)
                {
                    Vector3 prevEnd = joints[i-1].GetEndPosition();
                    joints[i].Position = prevEnd;
                }
                
                // Check if we've made progress
                endEffector = joints[joints.Length - 1].GetEndPosition();
                float newError = Vector3.Distance(endEffector, targetPosition);
                
                // Only revert if things get significantly worse
                if (newError > currentError * 1.5f) // Allow some error increase for exploration
                {
                    // Increase damping but don't revert
                    dampingFactor *= 1.5f;
                }
                else
                {
                    // Decrease damping if we're doing well
                    dampingFactor = Mathf.Max(0.001f, dampingFactor * 0.9f);
                    currentError = newError;
                }
            }
        }
        
        // Solve linear system Ax = b using Gauss-Seidel iterative method
        private void SolveLinearSystem(float[,] A, float[] b, float[] x, int iterations)
        {
            int n = b.Length;
            
            // Initialize x to zeros
            for (int i = 0; i < n; i++)
            {
                x[i] = 0;
            }
            
            // Perform Gauss-Seidel iterations
            for (int iter = 0; iter < iterations; iter++)
            {
                for (int i = 0; i < n; i++)
                {
                    float sum = b[i];
                    
                    for (int j = 0; j < n; j++)
                    {
                        if (j != i)
                        {
                            sum -= A[i, j] * x[j];
                        }
                    }
                    
                    if (Mathf.Abs(A[i, i]) > 0.00001f)
                    {
                        x[i] = sum / A[i, i];
                    }
                }
            }
        }
    }
}