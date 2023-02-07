using UnityEngine;

/// <summary>
/// Evades a target by fleeing from the estimated future position of the target.
/// </summary>
public class EvadeBehaviour : SteeringBehaviour
{
    public override Vector2 GetSteeringForce(Rigidbody2D self, Rigidbody2D target, float maxVelocity)
    {
        var directionToTarget = target.position - self.position;
        var lookAheadTime = directionToTarget.magnitude / (maxVelocity + target.velocity.magnitude);
        return Flee(self, target.position + target.velocity * lookAheadTime, maxVelocity);
    }
}