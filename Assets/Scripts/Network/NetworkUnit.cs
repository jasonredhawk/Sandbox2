using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Placeholder for networked unit synchronization.
/// </summary>
[RequireComponent(typeof(Unit))]
public class NetworkUnit : NetworkBehaviour
{
    private Unit unit;

    void Awake()
    {
        unit = GetComponent<Unit>();
    }

    // Future: sync position, action state, capacity via NetworkVariables/RPCs.
}
