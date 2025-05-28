using UnityEngine;

namespace HereticalSolutions.IK.Unity
{
    public class IKJoint2D
    {
        public Vector2 Position;

        public float Angle;
        public float BaseForwardAngle;
        public float TotalAngle;

        public float Length;
        
        public float MinAngle;
        public float MaxAngle;
        
        public void Initialize(
            Vector2 position,
            Vector2 forwardDirection,

            float length,

            float minAngle = -180f,
            float maxAngle = 180f)
        {
            Position = position;

            Length = length;

            MinAngle = minAngle;
            MaxAngle = maxAngle;
            
            // Store the initial forward angle relative to right
            BaseForwardAngle = Vector2.SignedAngle(
                Vector2.right,
                forwardDirection);

            if (BaseForwardAngle > 180f)
                BaseForwardAngle -= 360f;
            else if (BaseForwardAngle < -180f)
                BaseForwardAngle += 360f;
            
            Angle = -BaseForwardAngle;
            TotalAngle = 0f;
        }
        
        public void SetAngle(
            float newAngle)
        {
            if (newAngle > 180f)
                newAngle -= 360f;
            else if (newAngle < -180f)
                newAngle += 360f;

            TotalAngle = Mathf.Clamp(
                newAngle,
                MinAngle,
                MaxAngle);

            Angle = TotalAngle - BaseForwardAngle;

            if (Angle > 180f)
                Angle -= 360f;
            else if (Angle < -180f)
                Angle += 360f;
        }
        
        public Vector2 GetEndPosition()
        {
            // Rotate right vector by total angle to get current direction
            Vector2 currentDirection = new Vector2(
                Mathf.Cos(TotalAngle * Mathf.Deg2Rad),
                Mathf.Sin(TotalAngle * Mathf.Deg2Rad)
            );
            
            return Position + currentDirection * Length;
        }
    }
}