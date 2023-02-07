using UnityEngine;

/// <summary>
/// Moves to and smoothly stops at a target.
/// </summary>
public class ArriveBehaviour : SteeringBehaviour
{
    /// <summary>
    /// The rate to decelerate at (modified).
    /// </summary>
    public DecelerationRate decelerationRate = DecelerationRate.Normal;
    public float decelerationModifier = 0.3f;
    
    public override Vector2 GetSteeringForce(Rigidbody2D self, Rigidbody2D target, float maxVelocity)
    {
        var direction = target.position - self.position;
        var distance = direction.magnitude;

        if (distance > Constants.MinFloatDistance)
        {
            var speed = distance / ((int)decelerationRate * decelerationModifier);
            var desiredVelocity = direction * speed / distance;
            return desiredVelocity - self.velocity;
        }

        return Vector2.zero;
    }
}   