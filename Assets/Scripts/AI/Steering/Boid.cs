using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public string swarmWithTag;

    public float movementSpeed = 0.5f;
    public float rotationSpeed = 1f;
    public float repulsionRadius = 0.2f;
    public float alignmentRadius= 0.3f;
    public float attractionRadius = 0.4f;

    private Camera _camera;
    private GameObject[] _swarmers;
    private List<GameObject> _repulsables = new();
    private List<GameObject> _alignables = new();
    private List<GameObject> _attractables = new();
    private Vector2 _direction = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        _swarmers = GameObject.FindGameObjectsWithTag(swarmWithTag)
            .Where(x => x.GetInstanceID() != gameObject.GetInstanceID()).ToArray();
        _repulsables = new List<GameObject>();
        _alignables = new List<GameObject>();
        _attractables = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        // var mouseDir = FollowMouse();
        AssignSwarmables();
        var repulseDir = Repulse();
        Align();
            // var attractDir = Attract();
        transform.position = Vector2.MoveTowards(transform.position, (repulseDir) / 1, Time.deltaTime * movementSpeed);
    }

    private Vector2 FollowMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        var position = transform.position;
        mousePosition.z = position.z - _camera.transform.position.z;
        mousePosition = _camera.ScreenToWorldPoint(mousePosition);
        Vector3 direction = (mousePosition - position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90.0f;
        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        
        return mousePosition - transform.position;
        
        return Vector2.MoveTowards(position, mousePosition,
            Time.deltaTime * movementSpeed);
    }

    private void AssignSwarmables()
    {
        foreach (var swarmer in _swarmers)
        {
            var distance = Vector2.Distance(transform.position, swarmer.transform.position);
            if (distance >= attractionRadius) continue;

            if (distance <= repulsionRadius)
            {
                if (_repulsables.Contains(swarmer)) continue;
                _repulsables.Add(swarmer);
                _alignables.Remove(swarmer);
                _attractables.Remove(swarmer);
                continue;
            }

            if (distance <= alignmentRadius)
            {
                if (_alignables.Contains(swarmer)) continue;
                _repulsables.Remove(swarmer);
                _alignables.Add(swarmer);
                _attractables.Remove(swarmer);
                continue;
            }

            if (distance <= attractionRadius)
            {
                if (_attractables.Contains(swarmer)) continue;
                _repulsables.Remove(swarmer);
                _alignables.Remove(swarmer);
                _attractables.Add(swarmer);
            }
        }
    }

    private Vector2 Repulse()
    {
        if (_repulsables.Count == 0) return Vector2.zero;
        
        var averageRepulsionPosition = new Vector2(_repulsables.Sum(
                x => x.transform.position.x) / _repulsables.Count,
                _repulsables.Sum(x => x.transform.position.y) / _repulsables.Count
                );

        return -(averageRepulsionPosition - (Vector2)transform.position);
        return Vector2.MoveTowards(transform.position, averageRepulsionPosition, -Time.deltaTime * movementSpeed);
    }

    private void Align()
    {
        if (_alignables.Count == 0) return;

        var averageAlignAngle = Quaternion.identity;
        for (var i = 0; i < _alignables.Count; i++)
        {
            averageAlignAngle = Quaternion.Slerp(averageAlignAngle, _alignables[i].transform.rotation, 1.0f / (i + 1));
        }

        transform.rotation =
            Quaternion.Slerp(transform.rotation, Quaternion.Normalize(averageAlignAngle), Time.deltaTime * rotationSpeed);
    }

    private Vector2 Attract()
    {
        if (_attractables.Count == 0) return Vector2.zero;
        
        var averageAttractionPosition = new Vector2(_attractables.Sum(
                x => x.transform.position.x) / _attractables.Count,
            _attractables.Sum(x => x.transform.position.y) / _attractables.Count
        );

        return averageAttractionPosition - (Vector2)transform.position;
        
        return Vector2.MoveTowards(transform.position, averageAttractionPosition, Time.deltaTime * movementSpeed);
    }
}
