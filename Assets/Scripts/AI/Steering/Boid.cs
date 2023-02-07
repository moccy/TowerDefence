using UnityEngine;

public class Boid : MonoBehaviour
{
    public Rigidbody2D target;
    public float maxVelocity;
    private Rigidbody2D _rb;
    private SteeringBehaviour[] _steeringBehaviours;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _steeringBehaviours = GetComponents<SteeringBehaviour>();
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
        
        foreach (var steeringBehaviour in _steeringBehaviours)
        {
            force += steeringBehaviour.GetSteeringForce(_rb, target, maxVelocity);
        }

        return force;
    }
}