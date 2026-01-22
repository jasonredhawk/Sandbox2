using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Computes per-family brightness/contrast modifiers from ROS/Flame Length.
/// </summary>
public static class FuelCodeFamilyAutoTuner
{
    public static void Apply(FuelCodeSet set, float windSpeed, MoistureState moisture)
    {
        if (set == null || set.fuelCodes == null || set.fuelCodes.Count == 0) return;

        var familyBuckets = new Dictionary<string, List<FuelCodeData>>();
        foreach (var fc in set.fuelCodes)
        {
            if (fc == null) continue;
            string prefix = GetFamilyPrefix(fc);
            if (!familyBuckets.TryGetValue(prefix, out var list))
            {
                list = new List<FuelCodeData>();
                familyBuckets[prefix] = list;
            }
            list.Add(fc);
        }

        foreach (var kvp in familyBuckets)
        {
            var list = kvp.Value;
            if (list.Count == 0) continue;

            float minFlame = float.PositiveInfinity;
            float maxFlame = float.NegativeInfinity;
            float minRos = float.PositiveInfinity;
            float maxRos = float.NegativeInfinity;

            foreach (var fc in list)
            {
                float flame = fc.CalculateFlameLength(windSpeed, moisture);
                float ros = fc.CalculateROS(windSpeed, moisture);
                minFlame = Mathf.Min(minFlame, flame);
                maxFlame = Mathf.Max(maxFlame, flame);
                minRos = Mathf.Min(minRos, ros);
                maxRos = Mathf.Max(maxRos, ros);
            }

            float flameRange = maxFlame - minFlame;
            float rosRange = maxRos - minRos;

            foreach (var fc in list)
            {
                float flame = fc.CalculateFlameLength(windSpeed, moisture);
                float ros = fc.CalculateROS(windSpeed, moisture);

                float flameN = flameRange > 0.0001f ? Mathf.Clamp01((flame - minFlame) / flameRange) : 0.5f;
                float rosN = rosRange > 0.0001f ? Mathf.Clamp01((ros - minRos) / rosRange) : 0.5f;

                // Normalize: brightness by flame length, saturation by ROS
                fc.familyBrightness = Mathf.Lerp(0.3f, 1.0f, flameN);
                fc.familySaturation = Mathf.Lerp(0.3f, 1.0f, rosN);
                EditorUtility.SetDirty(fc);
            }
        }

        EditorUtility.SetDirty(set);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static string GetFamilyPrefix(FuelCodeData data)
    {
        string src = !string.IsNullOrWhiteSpace(data.title) ? data.title : data.codeGIS;
        if (string.IsNullOrWhiteSpace(src)) return "??";
        if (src.Length < 2) return src.ToUpperInvariant();
        return src.Substring(0, 2).ToUpperInvariant();
    }
}
