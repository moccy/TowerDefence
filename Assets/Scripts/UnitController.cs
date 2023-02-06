using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class UnitController: MonoBehaviour
{
    [CanBeNull] public GameObject path;
    [Range(0f, 10f)]
    public float moveSpeed = 5f;
    private readonly Queue<Transform> _waypointQueue = new();
    private void Start()
    {
        PopulateWaypoints();
    }

    private void Update()
    {
        FollowPath();
    }

    private void FollowPath()
    {
        if (_waypointQueue.Count <= 0) return;
        var currentTarget = _waypointQueue.Peek();
        transform.position = Vector2.MoveTowards(
            transform.position,
            currentTarget.position,
            Time.deltaTime * moveSpeed);

        if (Vector2.Distance(transform.position, currentTarget.position) <= Constants.MinFloatDistance)
        {
            _waypointQueue.Dequeue();
        }
    }

    private void PopulateWaypoints()
    {
        if (path == null)
        {
            return;
        }

        for(var i = 0; i < path.transform.childCount; i++)
        {
            var childTransforms = path.transform.GetChild(i).GetComponentsInChildren<Transform>();
            foreach (var child in childTransforms)
            {
                if (child.CompareTag("Waypoint"))
                {
                    _waypointQueue.Enqueue(child);
                }
            }
        }
    }
}
