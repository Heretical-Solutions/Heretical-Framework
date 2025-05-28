using System.Collections.Generic;

using UnityEngine;

namespace HereticalSolutions.IK.Unity
{
    public class GradientDescentChain3D
		: AIKChain3D
    {
		private float baseRotationStep;

		private float baseLearningRate;

		private float momentumFactor;

		private GradientState3D[] jointGradientStates;

        private Vector3[] originalPositions;

        private Quaternion[] originalRotations;

		private Vector3[] baseAxes;

        public GradientDescentChain3D(
            IKJoint3D[] joints,

            int maxIterations = 10,
            float tolerance = 0.01f,

			float baseRotationStep = 5f,
			float baseLearningRate = 1f,
			float momentumFactor = 0.5f)
			//bool matchEndEffectorOrientation = false)
			: base(
				joints,
				maxIterations,
				tolerance)
				//matchEndEffectorOrientation)
        {
			this.baseRotationStep = baseRotationStep;

			this.baseLearningRate = baseLearningRate;

			this.momentumFactor = momentumFactor;


			jointGradientStates = new GradientState3D[joints.Length];

			for (int i = 0; i < joints.Length; i++)
			{
				jointGradientStates[i] = new GradientState3D();
			}

            originalPositions = new Vector3[joints.Length];

            originalRotations = new Quaternion[joints.Length];

			baseAxes = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero };
        }
        
        protected override void Solve()
        {
			float learningRate = baseLearningRate;
            
            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                Vector3 endEffector = joints[joints.Length - 1].GetEndPosition();

                float currentError = Vector3.Distance(
                    endEffector,
                    targetPosition);
                
                if (currentError < tolerance)
                    break;
                
                for (int i = 0; i < joints.Length; i++)
                {
                    Quaternion originalRotation = joints[i].TotalRotation;

                    float rotationStep = 
                        baseRotationStep * (1f - (float)i / joints.Length * 0.5f);
                    
                    // Store original state
                    for (int j = 0; j < joints.Length; j++)
                    {
                        originalPositions[j] = joints[j].Position;

                        originalRotations[j] = joints[j].TotalRotation;
                    }
                    
                    // Get base orientation axes
                    Vector3 baseForward = joints[i].TotalRotation * Vector3.forward;

                    Vector3 baseUp = joints[i].TotalRotation * Vector3.up;

                    Vector3 baseRight = Vector3.Cross(
                        baseUp,
                        baseForward);
                    
                    Vector3 gradient = Vector3.zero;

                    //Vector3[] baseAxes = new Vector3[] { baseRight, baseUp, baseForward };
					baseAxes[0]	= baseRight;
					baseAxes[1]	= baseUp;
					baseAxes[2]	= baseForward;
                    
                    for (int axis = 0; axis < 3; axis++)
                    {
                        // Try positive rotation around base axis
                        Quaternion rotPlus =
                            Quaternion.AngleAxis(
                                rotationStep,
                                baseAxes[axis])
                            * originalRotation;

                        joints[i].SetRotation(rotPlus);

                        UpdateForwardKinematics(i);

                        endEffector = joints[joints.Length - 1].GetEndPosition();

                        float errorPlus = Vector3.Distance(endEffector, targetPosition);
                        
                        // Restore state
                        for (int j = 0; j < joints.Length; j++)
                        {
                            joints[j].Position = originalPositions[j];

                            joints[j].SetRotation(originalRotations[j]);
                        }
                        
                        // Try negative rotation around base axis
                        Quaternion rotMinus = 
                            Quaternion.AngleAxis(
                                -rotationStep,
                                baseAxes[axis])
                            * originalRotation;

                        joints[i].SetRotation(rotMinus);

                        UpdateForwardKinematics(i);

                        endEffector = joints[joints.Length - 1].GetEndPosition();
                        float errorMinus = Vector3.Distance(endEffector, targetPosition);
                        
                        // Restore state
                        for (int j = 0; j < joints.Length; j++)
                        {
                            joints[j].Position = originalPositions[j];
                            joints[j].SetRotation(originalRotations[j]);
                        }
                        
                        gradient[axis] = (errorPlus - errorMinus) / (2f * rotationStep);
                    }
                    
                    // Apply momentum
                    GradientState3D state = jointGradientStates[i];

                    Vector3 momentumGradient = gradient + state.momentum * momentumFactor;

                    state.momentum = momentumGradient;

                    state.previousGradient = gradient;
                    
                    // Apply rotations around base axes
                    Quaternion totalRotation = originalRotation;

                    for (int axis = 0; axis < 3; axis++)
                    {
                        float angle = -learningRate * momentumGradient[axis];

                        totalRotation = 
                            Quaternion.AngleAxis(
                                angle,
                                baseAxes[axis])
                            * totalRotation;
                    }
                    
                    joints[i].SetRotation(totalRotation);

                    UpdateForwardKinematics(i);
                    
                    float newError =
                        Vector3.Distance(
                            joints[joints.Length - 1].GetEndPosition(),
                            targetPosition);

                    if (newError < currentError * 0.9f)
                    {
                        learningRate *= 1.1f;
                    }
                    else if (newError > currentError)
                    {
                        learningRate *= 0.5f;

                        joints[i].SetRotation(originalRotation);

                        UpdateForwardKinematics(i);
                    }
                    
                    learningRate = Mathf.Clamp(learningRate, 0.1f, 5f);
                }
                
                // Maintain chain integrity
                Vector3 basePos = joints[0].Position;

                for (int i = 1; i < joints.Length; i++)
                {
                    Vector3 prevEnd = joints[i-1].GetEndPosition();

                    joints[i].Position = prevEnd;
                }
            }
        }
    }
}