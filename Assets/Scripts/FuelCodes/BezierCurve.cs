using UnityEngine;
using System;

/// <summary>
/// 4-point cubic bezier curve with min/max bounds.
/// P0 = (0, min), P1 = (x1, y1), P2 = (x2, y2), P3 = (max, maxValue).
/// </summary>
[Serializable]
public class BezierCurve
{
    public float x1 = 0f;
    public float y1 = 0f;
    public float x2 = 0f;
    public float y2 = 0f;
    public float min = 0f;
    public float max = 100f;

    public float Evaluate(float t)
    {
        t = Mathf.Clamp01(t);
        float u = 1f - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector2 p0 = new Vector2(0f, min);
        Vector2 p1 = new Vector2(x1, y1);
        Vector2 p2 = new Vector2(x2, y2);
        Vector2 p3 = new Vector2(max, max);

        Vector2 result = uuu * p0 + 3f * uu * t * p1 + 3f * u * tt * p2 + ttt * p3;
        return Mathf.Clamp(result.y, min, max);
    }

    public float EvaluateWithInput(float input, float inputMin, float inputMax)
    {
        float nt = Mathf.InverseLerp(inputMin, inputMax, input);
        return Evaluate(nt);
    }

    public static BezierCurve From(float x1, float y1, float x2, float y2, float min, float max)
    {
        return new BezierCurve
        {
            x1 = x1,
            y1 = y1,
            x2 = x2,
            y2 = y2,
            min = min,
            max = max
        };
    }
}
