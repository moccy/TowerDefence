using UnityEngine;

/// <summary>
/// Pursues a target by predicting its future position based on the targets velocity.
/// </summary>
public class PursuitBehaviour : SteeringBehaviour
{
    public override Vector2 GetSteeringForce(Rigidbody2D self, Rigidbody2D target, float maxVelocity)
    {
        // If the target is ahead and facing us, then just head towards its position.
        var directionToTarget = target.position - self.position;

        var headingAngle = Vector2.Dot(self.velocity.normalized, target.velocity.normalized);

        if (Vector2.Dot(directionToTarget, self.velocity.normalized) > 0 && headingAngle < -0.95f)
        {
            return Seek(self, target.position, maxVelocity);
        }

        var lookAheadTime = directionToTarget.magnitude / (maxVelocity + target.velocity.magnitude);
        return Seek(self, target.position + target.velocity * lookAheadTime, maxVelocity);
    }
}