using UnityEngine;

/// <summary>
/// ScriptableObject for unit definitions.
/// Keep fields simple; extend as needed.
/// </summary>
[CreateAssetMenu(fileName = "UnitData", menuName = "Units/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("Identity")]
    public string id;
    public string displayName;
    public string unitType; // e.g., groundcrew, engine, aircraft
    public string agency;   // e.g., groundcrew/aircraft

    [Header("Movement")]
    public float moveSpeed = 5f;            // m/s (scaled)
    public float cutSpeed = 1f;             // arbitrary units
    public float maxSlope = 35f;            // degrees

    [Header("Water/Suppression")]
    public float waterCapacity = 1000f;
    public float waterInputAmount = 50f;    // refill rate
    public float waterOutputAmount = 50f;   // spray rate
    public float suppressionSpeed = 10f;    // dirt/retardant rate

    [Header("Combat/Effectiveness")]
    public float effectiveRadius = 30f;     // meters

    [Header("Speed Factors (per fuel code index)")]
    public float[] terrainSpeedFactors = new float[0];
    public float[] cutSpeedFactors = new float[0];
}
