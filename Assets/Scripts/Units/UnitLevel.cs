using UnityEngine;

/// <summary>
/// Defines a single level for a unit, with cost and stat multipliers.
/// </summary>
[System.Serializable]
public class UnitLevel
{
    public int level = 1;
    public int firepointsCost = 0;

    [Header("Multipliers")]
    public float moveSpeedMult = 1f;
    public float waterCapacityMult = 1f;
    public float suppressionSpeedMult = 1f;
    public float cutSpeedMult = 1f;
}
