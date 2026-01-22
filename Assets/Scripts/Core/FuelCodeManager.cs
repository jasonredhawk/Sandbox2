using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages fuel code lookups and calculations at runtime
/// Based on old project's fuel code system
/// </summary>
public class FuelCodeManager : MonoBehaviour
{
    [Header("Fuel Code Data")]
    public FuelCodeSet fuelCodeSet;
    
    // Cache for fast lookups
    private Dictionary<short, FuelCodeData> fuelCodeCache = new Dictionary<short, FuelCodeData>();
    
    void Awake()
    {
        BuildCache();
    }
    
    void BuildCache()
    {
        fuelCodeCache.Clear();
        if (fuelCodeSet != null)
        {
            foreach (FuelCodeData fuelCode in fuelCodeSet.fuelCodes)
            {
                fuelCodeCache[fuelCode.fuelCodeID] = fuelCode;
            }
        }
    }
    
    /// <summary>
    /// Get ROS (Rate of Spread) for a fuel code
    /// Based on old project's GetROS() method
    /// </summary>
    public float GetROS(short fuelCodeID, float windSpeed, MoistureState moisture = MoistureState.Medium)
    {
        if (fuelCodeCache.TryGetValue(fuelCodeID, out FuelCodeData fuelCode))
        {
            return fuelCode.CalculateROS(windSpeed, moisture);
        }
        return 0f;
    }
    
    /// <summary>
    /// Get flame length for a fuel code
    /// Based on old project's GetFlameLength() method
    /// </summary>
    public float GetFlameLength(short fuelCodeID, float windSpeed, MoistureState moisture = MoistureState.Medium)
    {
        if (fuelCodeCache.TryGetValue(fuelCodeID, out FuelCodeData fuelCode))
        {
            return fuelCode.CalculateFlameLength(windSpeed, moisture);
        }
        return 0f;
    }
    
    /// <summary>
    /// Get slope factor for a fuel code
    /// </summary>
    public float GetSlopeFactor(short fuelCodeID, float slopeAngle, MoistureState moisture = MoistureState.Medium)
    {
        if (fuelCodeCache.TryGetValue(fuelCodeID, out FuelCodeData fuelCode))
        {
            return fuelCode.CalculateSlopeFactor(slopeAngle, moisture);
        }
        return 1f;
    }
    
    /// <summary>
    /// Get fuel code data by ID
    /// </summary>
    public FuelCodeData GetFuelCodeData(short fuelCodeID)
    {
        fuelCodeCache.TryGetValue(fuelCodeID, out FuelCodeData fuelCode);
        return fuelCode;
    }
}
