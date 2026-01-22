using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages LinePaths in the scene. Simplified: create, register, basic path query (nearest point).
/// </summary>
public class LinePathManager : MonoBehaviour
{
    public List<LinePath> linePaths = new List<LinePath>();

    public void Register(LinePath path)
    {
        if (path != null && !linePaths.Contains(path))
        {
            linePaths.Add(path);
        }
    }

    public void Unregister(LinePath path)
    {
        if (path != null && linePaths.Contains(path))
        {
            linePaths.Remove(path);
        }
    }

    public LinePathPoint GetNearestPoint(Vector3 position)
    {
        LinePathPoint best = null;
        float bestDist = float.MaxValue;

        foreach (var path in linePaths)
        {
            foreach (var p in path.points)
            {
                float d = Vector3.SqrMagnitude(p.WorldPosition - position);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = p;
                }
            }
        }

        return best;
    }
}
