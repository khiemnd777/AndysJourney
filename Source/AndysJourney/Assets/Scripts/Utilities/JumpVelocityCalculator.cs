using UnityEngine;

public class JumpVelocityCalculator
{
    public static JumpVelocityData Calculate(Vector3 ownerPosition, Vector3 targetPosition, float gravity, float height, bool updateHeight)
    {
        var offsetY = targetPosition.y - ownerPosition.y;
        var offsetXZ = new Vector3(targetPosition.x - ownerPosition.x, 0, targetPosition.z - ownerPosition.z);
        var updatedHeight = offsetY - height > 0 ? offsetY + height : height;
        var time = Mathf.Sqrt(-2 * updatedHeight / gravity) + Mathf.Sqrt(2 * (offsetY - updatedHeight) / gravity);
        var velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * updatedHeight);
        var velocityXZ = offsetXZ / time;

        return new JumpVelocityData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
    }

    public static void DrawPath(Vector3 ownerPosition, Vector3 targetPosition, float gravity, float height, bool updateHeight)
    {
        var calculatedJump = Calculate(ownerPosition, targetPosition, gravity, height, updateHeight);
        var previousDrawPoint = ownerPosition;

        int resolution = 30;
        for (int i = 1; i <= resolution; i++)
        {
            var simulationTime = i / (float)resolution * calculatedJump.simulatedTime;
            var displacement = calculatedJump.velocity * simulationTime + Vector3.up * gravity * simulationTime * simulationTime / 2f;
            var drawPoint = ownerPosition + displacement;
            Debug.DrawLine(previousDrawPoint, drawPoint, Color.green);
            previousDrawPoint = drawPoint;
        }
    }

    public static float GetGravity2D(Rigidbody2D rb){
        return -rb.gravityScale * Physics2D.gravity.magnitude;
    }

    public struct JumpVelocityData
    {
        public readonly Vector3 velocity;
        public readonly float simulatedTime;

        public JumpVelocityData(Vector3 velocity, float simulatedTime)
        {
            this.velocity = velocity;
            this.simulatedTime = simulatedTime;
        }
    }
}