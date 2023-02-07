using UnityEngine;

namespace AI.Steering.Behaviours
{
    /// <summary>
    /// Seeks out a target. Effectively heading straight towards the targets current position.
    /// </summary>
    public class SeekBehavior : SteeringBehaviour
    {
        public override Vector2 GetSteeringForce(Rigidbody2D self, Rigidbody2D target, float maxVelocity) 
            => Seek(self, target.position, maxVelocity);
    }
}