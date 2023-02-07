using UnityEngine;

public class Boid : MonoBehaviour
{
    public Rigidbody2D target;
    public SteeringBehaviour[] steeringBehaviours;
    public DecelerationRate decelerationRate = DecelerationRate.Normal;
    public float maxVelocity;
    private Rigidbody2D _rb;
    private Camera _mainCamera;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _mainCamera = Camera.main;
    }

    void Update()
    {
        var position = transform.position;
        
        Debug.DrawRay(position, target.position - (Vector2)position, Color.red);
        
        var steeringForce = CalculateSteeringForce();
        
        if (_rb.velocity.sqrMagnitude < maxVelocity * maxVelocity)
        {
            var accel = steeringForce / _rb.mass;
            _rb.velocity += accel * Time.deltaTime;
            
            var direction = (target.position - (Vector2)position).normalized;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private Vector2 CalculateSteeringForce()
    {
        var force = Vector2.zero;
        
        foreach (var steeringBehaviour in steeringBehaviours)
        {
            switch (steeringBehaviour)
            {
                case SteeringBehaviour.Arrive:
                    force += Arrive(target.position, decelerationRate);
                    break;
                case SteeringBehaviour.Flee:
                    force += Flee(target.position);
                    break;
                case SteeringBehaviour.Seek:
                    force += Seek(target.position);
                    break;
                case SteeringBehaviour.Pursuit:
                    force += Pursuit(target);
                    break;
                default:
                    continue;
            }
        }

        return force;
    }

    /// <summary>
    /// Seeks out a target. Effectively heading straight towards the targets current position.
    /// </summary>
    /// <param name="target">The target to seek.</param>
    /// <returns>The force needed to seek the target.</returns>
    Vector2 Seek(Vector2 target)
    {
        var desiredVelocity = (target - (Vector2)transform.position).normalized * maxVelocity;
        return desiredVelocity - _rb.velocity;
    }

    /// <summary>
    /// Flees from a target. The opposite of Seek.
    /// </summary>
    /// <param name="target">The target to flee from.</param>
    /// <returns>The force that needs to be applied to flee.</returns>
    Vector2 Flee(Vector2 target)
    {
        return -Seek(target);
    }
    
    /// <summary>
    /// Flees from a target, but only applies a force if the target is within the panic distance.
    /// </summary>
    /// <param name="target">The target to flee from.</param>
    /// <param name="panicDistance">The maximum distance between the target and the boid to apply a force.</param>
    /// <returns>The force that needs to be applied to flee.</returns>
    Vector2 Flee(Vector2 target, double panicDistance)
    {
        var distance = (target - (Vector2)transform.position).sqrMagnitude;
        if (distance > panicDistance * panicDistance)
        {
            return Vector2.zero;
        }
        return -Seek(target);
    }

    /// <summary>
    /// Moves to and smoothly stops at a target.
    /// </summary>
    /// <param name="target">The target to stop at.</param>
    /// <param name="decelerationRate">The rate to decelerate at (modified).</param>
    /// <returns>The force that needs to be applied to arrive.</returns>
    Vector2 Arrive(Vector2 target, DecelerationRate decelerationRate)
    {
        var direction = target - (Vector2)transform.position;
        var distance = direction.magnitude;

        if (distance > Constants.MinFloatDistance)
        {
            const float decelerationModifier = 0.3f;
            var speed = distance / ((int)decelerationRate * decelerationModifier);
            var desiredVelocity = direction * speed / distance;
            return desiredVelocity - _rb.velocity;
        }

        return Vector2.zero;
    }

    /// <summary>
    /// Pursues a target by predicting its future position based on the targets velocity.
    /// </summary>
    /// <param name="target">The target to pursue.</param>
    /// <returns>The force needed to pursue the target.</returns>
    Vector2 Pursuit(Rigidbody2D target)
    {
        // If the target is ahead and facing us, then just head towards its position.
        var directionToTarget = target.position - (Vector2)transform.position;

        var headingAngle = Vector2.Dot(_rb.velocity.normalized, target.velocity.normalized);

        if (Vector2.Dot(directionToTarget, _rb.velocity.normalized) > 0 && headingAngle < -0.95f)
        {
            return Seek(target.position);
        }

        var lookAheadTime = directionToTarget.magnitude / (maxVelocity + target.velocity.magnitude);
        return Seek(target.position + target.velocity * lookAheadTime);
    }
}