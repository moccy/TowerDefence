using UnityEngine;

namespace AI.Steering.Behaviours
{
    /// <summary>
    /// Flees from a target. The opposite of Seek.
    /// </summary>
    public class FleeBehaviour : SteeringBehaviour
    {
        /// <summary>
        /// The maximum distance between the target and the boid to apply a force.
        /// Ignored if 0.
        /// </summary>
        public float panicDistance;
    
        public override Vector2 GetSteeringForce(Rigidbody2D self, Rigidbody2D target, float maxVelocity)
        {
            var distance = (target.position - self.position).sqrMagnitude;
            if (distance > panicDistance * panicDistance)
            {
                return Vector2.zero;
            }

            return Flee(self, target.position, maxVelocity);
        }
    }
}