using UnityEngine;

public class JumpVelocityCalculator
{
    public JumpVelocityData Calculate(Transform own, Transform target, float gravity, float height, bool updateHeight)
    {
        var displacementY = target.position.y - own.position.y;
        var displacementXZ = new Vector3(target.position.x - own.position.x, 0, target.position.z - own.position.z);
        var updatedHeight = displacementY - height > 0 ? displacementY + height : height;
        var time = Mathf.Sqrt(-2 * updatedHeight / gravity) + Mathf.Sqrt(2 * (displacementY - updatedHeight) / gravity);
        var velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * updatedHeight);
        var velocityXZ = displacementXZ / time;

        return new JumpVelocityData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
    }

    public void DrawPath(Transform own, Transform target, float gravity, float height, bool updateHeight)
    {
        var calculatedJump = Calculate(own, target.transform, gravity, height, updateHeight);
        var previousDrawPoint = own.position;

        int resolution = 30;
        for (int i = 1; i <= resolution; i++)
        {
            var simulationTime = i / (float)resolution * calculatedJump.simulatedTime;
            var displacement = calculatedJump.velocity * simulationTime + Vector3.up * gravity * simulationTime * simulationTime / 2f;
            var drawPoint = own.position + displacement;
            Debug.DrawLine(previousDrawPoint, drawPoint, Color.green);
            previousDrawPoint = drawPoint;
        }
    }

    public float GetGravity2D(Rigidbody2D rb){
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