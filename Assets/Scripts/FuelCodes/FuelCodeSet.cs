using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Collection of fuel codes. Supports loading from the legacy JSON (fuelcodes_export.json).
/// </summary>
[CreateAssetMenu(fileName = "FuelCodeSet", menuName = "Fuel Codes/Fuel Code Set")]
public class FuelCodeSet : ScriptableObject
{
    public List<FuelCodeData> fuelCodes = new List<FuelCodeData>();

    public FuelCodeData GetFuelCode(short fuelCodeID)
    {
        return fuelCodes.FirstOrDefault(fc => fc.fuelCodeID == fuelCodeID);
    }

    [Serializable]
    private class FuelCodeJsonWrapper
    {
        public FuelCodeJsonItem[] items;
    }

    [Serializable]
    private class FuelCodeJsonItem
    {
        public string title;
        public string codeGIS;
        public float hour1;
        public float hour10;
        public float hour100;
        public CurveJson rosVeryLow;
        public CurveJson rosLow;
        public CurveJson rosMedium;
        public CurveJson rosHigh;
        public CurveJson flameVeryLow;
        public CurveJson flameLow;
        public CurveJson flameMedium;
        public CurveJson flameHigh;
        public CurveJson slopeVeryLow;
        public CurveJson slopeLow;
        public CurveJson slopeMedium;
        public CurveJson slopeHigh;
    }

    [Serializable]
    private class CurveJson
    {
        public float x1;
        public float y1;
        public float x2;
        public float y2;
        public float min;
        public float max;
    }

    /// <summary>
    /// Load from fuelcodes_export.json text.
    /// </summary>
    public void LoadFromJsonText(string jsonText)
    {
        if (string.IsNullOrEmpty(jsonText))
        {
            Debug.LogError("FuelCodeSet: JSON text is null or empty.");
            return;
        }

        FuelCodeJsonWrapper wrapper = JsonUtility.FromJson<FuelCodeJsonWrapper>(jsonText);
        if (wrapper == null || wrapper.items == null)
        {
            Debug.LogError("FuelCodeSet: Failed to parse JSON.");
            return;
        }

        fuelCodes.Clear();
        foreach (var item in wrapper.items)
        {
            var fc = ScriptableObject.CreateInstance<FuelCodeData>();
            fc.title = item.title;
            fc.codeGIS = item.codeGIS;
            if (short.TryParse(item.codeGIS, out short id))
                fc.fuelCodeID = id;
            fc.hour1 = item.hour1;
            fc.hour10 = item.hour10;
            fc.hour100 = item.hour100;

            fc.rosVeryLow = ToCurve(item.rosVeryLow);
            fc.rosLow = ToCurve(item.rosLow);
            fc.rosMedium = ToCurve(item.rosMedium);
            fc.rosHigh = ToCurve(item.rosHigh);

            fc.flameVeryLow = ToCurve(item.flameVeryLow);
            fc.flameLow = ToCurve(item.flameLow);
            fc.flameMedium = ToCurve(item.flameMedium);
            fc.flameHigh = ToCurve(item.flameHigh);

            fc.slopeVeryLow = ToCurve(item.slopeVeryLow);
            fc.slopeLow = ToCurve(item.slopeLow);
            fc.slopeMedium = ToCurve(item.slopeMedium);
            fc.slopeHigh = ToCurve(item.slopeHigh);

            fuelCodes.Add(fc);
        }
    }

    private BezierCurve ToCurve(CurveJson c)
    {
        if (c == null) return null;
        return BezierCurve.From(c.x1, c.y1, c.x2, c.y2, c.min, c.max);
    }
}
