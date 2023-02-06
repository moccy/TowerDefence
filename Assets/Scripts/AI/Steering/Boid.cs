using UnityEngine;

public class Boid : MonoBehaviour
{
    public float maxVelocity;
    private Rigidbody2D _rb;
    private Vector2 _target;
    private Camera _mainCamera;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _mainCamera = Camera.main;
    }

    void Update()
    {
        var position = transform.position;

        if (Input.GetMouseButton(0))
        {
            _target = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (_target == default) return;
        
        Debug.DrawRay(position, _target - (Vector2)position, Color.red);

        Vector2 steeringForce = Arrive(_target, DecelerationRate.Fast);
        if (_rb.velocity.sqrMagnitude < maxVelocity * maxVelocity)
        {
            var accel = steeringForce / _rb.mass;
            _rb.velocity += accel * Time.deltaTime;
            
            var direction = (_target - (Vector2)position).normalized;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    Vector2 Seek(Vector2 target)
    {
        var desiredVelocity = (target - (Vector2)transform.position).normalized * maxVelocity;
        return desiredVelocity - _rb.velocity;
    }

    Vector2 Flee(Vector2 target)
    {
        return -Seek(target);
    }
    
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
    /// <returns>The force that needs to be applied.</returns>
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
}

public enum DecelerationRate
{
    Fast = 1,
    Normal = 2,
    Slow = 3
}
