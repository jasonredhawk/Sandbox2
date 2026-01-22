using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles leveling logic: tracks firepoints and applies level multipliers.
/// </summary>
public class UnitLeveling : MonoBehaviour
{
    public UnitData unitData;
    public List<UnitLevel> levels = new List<UnitLevel>();
    public int currentLevelIndex = 0;
    public int firepoints = 0;

    public bool CanLevelUp()
    {
        if (levels == null || levels.Count == 0) return false;
        int next = currentLevelIndex + 1;
        return next < levels.Count && firepoints >= levels[next].firepointsCost;
    }

    public void LevelUp()
    {
        if (!CanLevelUp()) return;
        int next = currentLevelIndex + 1;
        firepoints -= levels[next].firepointsCost;
        currentLevelIndex = next;
    }

    public float MoveSpeedMultiplier => GetLevel().moveSpeedMult;
    public float WaterCapacityMultiplier => GetLevel().waterCapacityMult;
    public float SuppressionSpeedMultiplier => GetLevel().suppressionSpeedMult;
    public float CutSpeedMultiplier => GetLevel().cutSpeedMult;

    private UnitLevel GetLevel()
    {
        if (levels == null || levels.Count == 0) return new UnitLevel();
        int idx = Mathf.Clamp(currentLevelIndex, 0, levels.Count - 1);
        return levels[idx];
    }
}
