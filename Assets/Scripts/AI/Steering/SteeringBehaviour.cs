using UnityEngine;

namespace AI.Steering
{
    public abstract class SteeringBehaviour: MonoBehaviour
    {
        public abstract Vector2 GetSteeringForce(Rigidbody2D self, Rigidbody2D target, float maxVelocity);

        public Vector2 Seek(Rigidbody2D self, Vector2 targetPosition, float maxVelocity)
        {
            var desiredVelocity = (targetPosition - self.position).normalized * maxVelocity;
            return desiredVelocity - self.velocity;
        }

        public Vector2 Flee(Rigidbody2D self, Vector2 targetPosition, float maxVelocity) =>
            -Seek(self, targetPosition, maxVelocity);
    }
}