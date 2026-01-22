using UnityEngine;

/// <summary>
/// Fuel code data ScriptableObject (matches fuelcodes_export.json structure).
/// </summary>
[CreateAssetMenu(fileName = "FuelCode", menuName = "Fuel Codes/Fuel Code Data")]
public class FuelCodeData : ScriptableObject
{
    [Header("Fuel Code Info")]
    public string title;
    public string codeGIS;
    public short fuelCodeID;
    public Color baseColor = Color.green;

    [Header("Fuel Load (tons/acre)")]
    public float hour1 = 0f;
    public float hour10 = 0f;
    public float hour100 = 0f;

    [Header("ROS Curves (Rate of Spread)")]
    public BezierCurve rosVeryLow;
    public BezierCurve rosLow;
    public BezierCurve rosMedium;
    public BezierCurve rosHigh;

    [Header("Flame Length Curves")]
    public BezierCurve flameVeryLow;
    public BezierCurve flameLow;
    public BezierCurve flameMedium;
    public BezierCurve flameHigh;

    [Header("Slope Curves")]
    public BezierCurve slopeVeryLow;
    public BezierCurve slopeLow;
    public BezierCurve slopeMedium;
    public BezierCurve slopeHigh;

    public BezierCurve GetROSCurve(MoistureState moisture) =>
        moisture switch
        {
            MoistureState.VeryLow => rosVeryLow,
            MoistureState.Low => rosLow,
            MoistureState.High => rosHigh,
            _ => rosMedium
        };

    public BezierCurve GetFlameCurve(MoistureState moisture) =>
        moisture switch
        {
            MoistureState.VeryLow => flameVeryLow,
            MoistureState.Low => flameLow,
            MoistureState.High => flameHigh,
            _ => flameMedium
        };

    public BezierCurve GetSlopeCurve(MoistureState moisture) =>
        moisture switch
        {
            MoistureState.VeryLow => slopeVeryLow,
            MoistureState.Low => slopeLow,
            MoistureState.High => slopeHigh,
            _ => slopeMedium
        };

    public float CalculateROS(float windSpeed, MoistureState moisture)
    {
        var c = GetROSCurve(moisture);
        if (c == null) return 0f;
        return c.EvaluateWithInput(windSpeed, 0f, 50f);
    }

    public float CalculateFlameLength(float windSpeed, MoistureState moisture)
    {
        var c = GetFlameCurve(moisture);
        if (c == null) return 0f;
        return c.EvaluateWithInput(windSpeed, 0f, 50f);
    }

    public float CalculateSlopeFactor(float slopeAngle, MoistureState moisture)
    {
        var c = GetSlopeCurve(moisture);
        if (c == null) return 1f;
        return c.EvaluateWithInput(slopeAngle, 0f, 90f);
    }
}
