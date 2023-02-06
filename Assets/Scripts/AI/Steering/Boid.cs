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

        Vector2 steeringForce = Seek(_target);
        if (_rb.velocity.sqrMagnitude < maxVelocity * maxVelocity)
        {
            var distanceModifier = Mathf.Min(Vector2.Distance(position, _target), 0.1f);
            _rb.AddForce(distanceModifier * steeringForce, ForceMode2D.Force);

            var direction = (_target - (Vector2)position).normalized;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        if ((Vector2)transform.position == _target) _target = default;
    }

    Vector2 Seek(Vector2 target)
    {
        var desiredVelocity = (target - (Vector2)transform.position).normalized * maxVelocity;
        return desiredVelocity - _rb.velocity;
    }
}
