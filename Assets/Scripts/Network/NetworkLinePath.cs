using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Placeholder for networked LinePath synchronization.
/// </summary>
[RequireComponent(typeof(LinePath))]
public class NetworkLinePath : NetworkBehaviour
{
    private LinePath linePath;

    void Awake()
    {
        linePath = GetComponent<LinePath>();
    }

    // Future: sync points and path type across network.
}
