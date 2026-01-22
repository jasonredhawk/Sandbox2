using UnityEngine;

/// <summary>
/// Simplified LinePathPoint with indicator flags.
/// </summary>
public class LinePathPoint : MonoBehaviour
{
    public enum IndicatorType { Water, Urban, Fire, Cut, Dozer, Point }

    [Header("State")]
    public LinePath parentLine;
    public bool water;
    public bool urban;
    public bool fire;
    public bool cut;
    public bool dozer;
    public bool point;

    public Vector3 WorldPosition => transform.position;
}
