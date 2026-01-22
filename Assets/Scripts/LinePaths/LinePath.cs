using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple LinePath container using LineRenderer to visualize points.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class LinePath : MonoBehaviour
{
    public LinePathType pathType = LinePathType.Road;
    public List<LinePathPoint> points = new List<LinePathPoint>();

    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        if (lr == null)
        {
            lr = gameObject.AddComponent<LineRenderer>();
        }
        lr.positionCount = 0;
    }

    public void SetPoints(List<Vector3> positions)
    {
        if (lr == null)
        {
            lr = GetComponent<LineRenderer>();
            if (lr == null)
            {
                Debug.LogWarning("LineRenderer missing on LinePath; adding one.");
                lr = gameObject.AddComponent<LineRenderer>();
            }
        }
        lr.positionCount = positions.Count;
        if (positions.Count > 0)
        {
            lr.SetPositions(positions.ToArray());
        }
    }

    public void RefreshFromChildren()
    {
        points.Clear();
        var pts = GetComponentsInChildren<LinePathPoint>();
        foreach (var p in pts)
        {
            points.Add(p);
            p.parentLine = this;
        }
        var posList = new List<Vector3>();
        foreach (var p in points) posList.Add(p.transform.position);
        SetPoints(posList);
    }
}
