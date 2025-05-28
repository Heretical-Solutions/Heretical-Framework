using System.Collections.Generic;

using UnityEngine;

namespace HereticalSolutions.IK.Unity
{
    public class GaussSeidelChain2D
		: AIKChain2D
    {
		private float baseDampingFactor;
		private float angleStepSize;

		private Vector2[] originalPositions;

		private float[] originalAngles;

		private int linearSolverIterations;
		private int dimensions = 2; // 2D has x,y
		private int numJoints;
		private float[,] jacobian;
		private float[,] jacobianT;
		private float[,] jtj;
		private float[] jte;
		private float[] deltaAngles;

        public GaussSeidelChain2D(
            IKJoint2D[] joints,

            int maxIterations = 10,
            float tolerance = 0.01f,

			int linearSolverIterations = 10,
			float baseDampingFactor = 0.05f, // Reduced damping for more direct movement
			float angleStepSize = 5f)
			//bool matchEndEffectorOrientation = false)
			: base(
				joints,
				maxIterations,
				tolerance)
				//matchEndEffectorOrientation)
        {
			this.baseDampingFactor = baseDampingFactor;
			this.angleStepSize = angleStepSize;
			this.linearSolverIterations = linearSolverIterations;

			originalPositions = new Vector2[joints.Length];
            originalAngles = new float[joints.Length];

            // Build the Jacobian matrix - one row per dimension (x,y), one column per joint
			numJoints = joints.Length;
			jacobian = new float[dimensions, numJoints];
			jacobianT = new float[numJoints, dimensions];
			jtj = new float[numJoints, numJoints];
			jte = new float[numJoints];
			deltaAngles = new float[numJoints];
        }
        
        protected override void Solve()
        {
            float dampingFactor = baseDampingFactor; // Reduced damping for more direct movement
            
            // Get initial state and error
            Vector2 endEffector = joints[joints.Length - 1].GetEndPosition();
            Vector2 errorVector = targetPosition - endEffector;
            float currentError = errorVector.magnitude;
            
            // Skip if already at target
            if (currentError < tolerance)
                return;
            
            // Store original state before any changes
            for (int j = 0; j < joints.Length; j++)
            {
                originalPositions[j] = joints[j].Position;
                originalAngles[j] = joints[j].TotalAngle;
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
                
                // Calculate Jacobian for each joint
                for (int jointIndex = 0; jointIndex < numJoints; jointIndex++)
                {
                    float originalAngle = joints[jointIndex].TotalAngle;
                    
                    // Sample positive rotation
                    joints[jointIndex].SetAngle(originalAngle + angleStepSize);
                    UpdateForwardKinematics(jointIndex);
                    Vector2 positivePos = joints[joints.Length - 1].GetEndPosition();
                    
                    // Restore original state
                    for (int j = 0; j < joints.Length; j++)
                    {
                        joints[j].Position = originalPositions[j];
                        joints[j].SetAngle(originalAngles[j]);
                    }
                    
                    // Sample negative rotation
                    joints[jointIndex].SetAngle(originalAngle - angleStepSize);

                    UpdateForwardKinematics(jointIndex);

                    Vector2 negativePos = joints[joints.Length - 1].GetEndPosition();
                    
                    // Restore original state
                    for (int j = 0; j < joints.Length; j++)
                    {
                        joints[j].Position = originalPositions[j];
                        joints[j].SetAngle(originalAngles[j]);
                    }
                    
                    // Calculate partial derivatives (Jacobian columns)
                    Vector2 derivative = (positivePos - negativePos) / (2f * angleStepSize);
                    jacobian[0, jointIndex] = derivative.x; // x-component
                    jacobian[1, jointIndex] = derivative.y; // y-component
                }
                
                // Calculate J^T (Jacobian transpose)
                for (int j = 0; j < numJoints; j++)
                {
                    for (int d = 0; d < dimensions; d++)
                    {
                        jacobianT[j, d] = jacobian[d, j];
                    }
                }
                
                // Calculate J^T * J (square matrix, numJoints x numJoints)
                for (int row = 0; row < numJoints; row++)
                {
                    for (int col = 0; col < numJoints; col++)
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
                
                // Calculate J^T * error (vector, numJoints x 1)
                for (int j = 0; j < numJoints; j++)
                {
                    jte[j] = jacobianT[j, 0] * errorVector.x + jacobianT[j, 1] * errorVector.y;
                }
                
                // Solve the linear system (J^T * J) * delta = J^T * error
                SolveLinearSystem(jtj, jte, deltaAngles, linearSolverIterations);
                
                // Apply all angle adjustments simultaneously
                for (int j = 0; j < numJoints; j++)
                {
                    // Apply larger adjustments to base joints (ones closer to root)
                    // This promotes more arm-like reaching rather than snake-like movement
                    float jointFactor = 1.0f + (numJoints - j - 1) * 0.2f; // Boost factor for base joints
                    
                    // Apply the joint factor and clamp adjustments
                    float deltaAngle = deltaAngles[j] * jointFactor;
                    deltaAngle = Mathf.Clamp(deltaAngle, -30f, 30f);
                    
                    joints[j].SetAngle(originalAngles[j] + deltaAngle);
                }
                
                // Update all positions
                Vector2 basePos = joints[0].Position;
                for (int i = 1; i < joints.Length; i++)
                {
                    Vector2 prevEnd = joints[i-1].GetEndPosition();
                    joints[i].Position = prevEnd;
                }
                
                // Check if we've made progress
                endEffector = joints[joints.Length - 1].GetEndPosition();
                float newError = Vector2.Distance(endEffector, targetPosition);
                
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
        private void SolveLinearSystem(
			float[,] A,
			float[] b,
			float[] x,
			int iterations)
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