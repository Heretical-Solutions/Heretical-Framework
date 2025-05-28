using System.Collections.Generic;

using UnityEngine;

namespace HereticalSolutions.IK.Unity
{
    public class GradientDescentChain2D
		: AIKChain2D
    {
		private float baseAngleStep;

		private float baseLearningRate;

		private float momentumFactor;

		private GradientState2D[] jointGradientStates;

        private Vector2[] originalPositions;

        private float[] originalAngles;

        public GradientDescentChain2D(
            IKJoint2D[] joints,

            int maxIterations = 10,
            float tolerance = 0.01f,

			float baseAngleStep = 5f,
			float baseLearningRate = 1f,
			float momentumFactor = 0.5f)
			//bool matchEndEffectorOrientation = false)
			: base(
				joints,
				maxIterations,
				tolerance)
				//matchEndEffectorOrientation)
        {
			this.baseAngleStep = baseAngleStep;

			this.baseLearningRate = baseLearningRate;

			this.momentumFactor = momentumFactor;


			jointGradientStates = new GradientState2D[joints.Length];

			for (int i = 0; i < joints.Length; i++)
			{
				jointGradientStates[i] = new GradientState2D();
			}

            originalPositions = new Vector2[joints.Length];

            originalAngles = new float[joints.Length];
        }
        
        protected override void Solve()
        {
            float learningRate = baseLearningRate;

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                Vector2 endEffector = joints[joints.Length - 1].GetEndPosition();

                float currentError = Vector2.Distance(
                    endEffector,
                    targetPosition);
                
                if (currentError < tolerance)
                    break;

                for (int i = 0; i < joints.Length; i++)
                {
                    float originalAngle = joints[i].TotalAngle;

                    float angleStep = baseAngleStep * (1f - (float)i / joints.Length * 0.5f);
                    
                    // Store original state
                    for (int j = 0; j < joints.Length; j++)
                    {
                        originalPositions[j] = joints[j].Position;

                        originalAngles[j] = joints[j].TotalAngle;
                    }
                    
                    // Try positive angle change
                    float testAngle = originalAngle + angleStep;

                    joints[i].SetAngle(testAngle);

                    UpdateForwardKinematics(i);

                    endEffector = joints[joints.Length - 1].GetEndPosition();
                    
                    float errorPlus = Vector2.Distance(endEffector, targetPosition);
                    
                    // Restore state
                    for (int j = 0; j < joints.Length; j++)
                    {
                        joints[j].Position = originalPositions[j];

                        joints[j].SetAngle(originalAngles[j]);
                    }
                    
                    // Try negative angle change
                    testAngle = originalAngle - angleStep;

                    joints[i].SetAngle(testAngle);

                    UpdateForwardKinematics(i);

                    endEffector = joints[joints.Length - 1].GetEndPosition();

                    float errorMinus = Vector2.Distance(endEffector, targetPosition);
                    
                    // Restore state
                    for (int j = 0; j < joints.Length; j++)
                    {
                        joints[j].Position = originalPositions[j];

                        joints[j].SetAngle(originalAngles[j]);
                    }
                    
                    // Calculate gradient
                    float gradient = (errorPlus - errorMinus) / (2f * angleStep);
                    
                    // Apply momentum
                    GradientState2D state = jointGradientStates[i];

                    float momentumGradient = gradient + state.momentum * momentumFactor;

                    state.momentum = momentumGradient;

                    state.previousGradient = gradient;
                    
                    // Calculate new angle relative to base forward
                    float newAngle = originalAngle - learningRate * momentumGradient;

                    joints[i].SetAngle(newAngle);

                    UpdateForwardKinematics(i);
                    
                    float newError = Vector2.Distance(
                        joints[joints.Length - 1].GetEndPosition(),
                        targetPosition);

                    if (newError < currentError * 0.9f)
                    {
                        learningRate *= 1.1f;
                    }
                    else if (newError > currentError)
                    {
                        learningRate *= 0.5f;

                        joints[i].SetAngle(originalAngle);

                        UpdateForwardKinematics(i);
                    }
                    
                    learningRate = Mathf.Clamp(learningRate, 0.1f, 5f);
                }
                
                // Maintain chain integrity
                Vector2 basePos = joints[0].Position;

                for (int i = 1; i < joints.Length; i++)
                {
                    Vector2 prevEnd = joints[i-1].GetEndPosition();

                    joints[i].Position = prevEnd;
                }
            }
        }
    }
}