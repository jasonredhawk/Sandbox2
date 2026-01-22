using UnityEngine;

/// <summary>
/// Registers a LinePath with a LinePathManager at Start.
/// </summary>
[RequireComponent(typeof(LinePath))]
public class LinePathRegistrar : MonoBehaviour
{
    public LinePathManager manager;

    void Start()
    {
        if (manager == null)
        {
            manager = FindObjectOfType<LinePathManager>();
        }

        var lp = GetComponent<LinePath>();
        if (manager != null && lp != null)
        {
            manager.Register(lp);
        }
    }
}
