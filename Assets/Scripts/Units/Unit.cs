using UnityEngine;

/// <summary>
/// Minimal unit component that reads UnitData and applies leveling multipliers.
/// </summary>
[RequireComponent(typeof(UnitLeveling))]
public class Unit : MonoBehaviour
{
    public UnitData unitData;
    public UnitAction currentAction = UnitAction.MOVE;

    [Header("Runtime")]
    public float currentCapacity;

    private UnitLeveling leveling;

    void Awake()
    {
        leveling = GetComponent<UnitLeveling>();
    }

    void Start()
    {
        if (unitData != null)
        {
            currentCapacity = unitData.waterCapacity * leveling.WaterCapacityMultiplier;
        }
    }

    public float MoveSpeed =>
        unitData != null
            ? unitData.moveSpeed * leveling.MoveSpeedMultiplier
            : 0f;

    public float CutSpeed =>
        unitData != null
            ? unitData.cutSpeed * leveling.CutSpeedMultiplier
            : 0f;

    public float SuppressionSpeed =>
        unitData != null
            ? unitData.suppressionSpeed * leveling.SuppressionSpeedMultiplier
            : 0f;
}
