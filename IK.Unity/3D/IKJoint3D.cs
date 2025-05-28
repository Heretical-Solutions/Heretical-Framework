using UnityEngine;

namespace HereticalSolutions.IK.Unity
{
    public class IKJoint3D
    {
        public Vector3 Position;

        public Quaternion Rotation;
        public Quaternion BaseRotation;
        public Quaternion TotalRotation;

        public float Length;
        
        private Vector3 MinRotation;
        private Vector3 MaxRotation;
        
        public void Initialize(
            Vector3 position,
            Vector3 forwardDirection,
            Vector3 upDirection,

            float length,

            Vector3 minRotation = default,
            Vector3 maxRotation = default)
        {
            Position = position;
            
            Length = length;

            MinRotation = minRotation;
            MaxRotation = maxRotation;
            
            // Store the initial orientation using forward and up
            BaseRotation = 
                Quaternion.Inverse(
                    Quaternion.identity)
                * Quaternion.LookRotation(
                    forwardDirection,
                    upDirection);
            
            Rotation = Quaternion.Inverse(BaseRotation) * Quaternion.identity;

            TotalRotation = Quaternion.identity;
        }
        
        public void SetRotation(
            Quaternion rotation)
        {
            // Convert rotation to euler angles for clamping
            Vector3 euler = rotation.eulerAngles;
            
            // Normalize angles to -180 to 180 range
            for (int i = 0; i < 3; i++)
            {
                if (euler[i] > 180f) euler[i] -= 360f;
            }
            
            // Clamp angles
            euler.x = Mathf.Clamp(euler.x, MinRotation.x, MaxRotation.x);
            euler.y = Mathf.Clamp(euler.y, MinRotation.y, MaxRotation.y);
            euler.z = Mathf.Clamp(euler.z, MinRotation.z, MaxRotation.z);
            
            // Store clamped rotation
            TotalRotation = Quaternion.Euler(euler);

            Rotation = TotalRotation * Quaternion.Inverse(BaseRotation);
        }
        
        public Vector3 GetEndPosition()
        {
            // Apply total rotation to forward vector
            Vector3 currentDirection = TotalRotation * Vector3.forward;
            
            return Position + currentDirection * Length;
        }
    }
}