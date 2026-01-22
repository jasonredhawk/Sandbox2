using UnityEditor;
using UnityEngine;

/// <summary>
/// Applies base colors to all FuelCodeData entries in a FuelCodeSet.
/// </summary>
public static class FuelCodeSetColorizer
{
    [MenuItem("Tools/Apply Fuel Code Colors")]
    public static void ApplyColors()
    {
        var set = Selection.activeObject as FuelCodeSet;
        if (set == null)
        {
            EditorUtility.DisplayDialog("Select FuelCodeSet", "Select a FuelCodeSet asset in the Project window.", "OK");
            return;
        }

        int count = 0;
        foreach (var fc in set.fuelCodes)
        {
            if (fc == null) continue;
            fc.baseColor = GetBaseColorForFuel(fc.title, fc.codeGIS);
            EditorUtility.SetDirty(fc);
            count++;
        }

        EditorUtility.SetDirty(set);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Colors Applied", $"Updated baseColor on {count} fuel codes.", "OK");
    }

    private static Color GetBaseColorForFuel(string title, string codeGIS)
    {
        string prefix = null;
        if (!string.IsNullOrEmpty(title) && title.Length >= 2)
        {
            prefix = title.Substring(0, 2).ToUpperInvariant();
        }
        else if (!string.IsNullOrEmpty(codeGIS) && codeGIS.Length >= 2)
        {
            prefix = codeGIS.Substring(0, 2).ToUpperInvariant();
        }
        if (string.IsNullOrEmpty(prefix)) return new Color(0.6f, 0.7f, 0.6f, 1f);

        switch (prefix)
        {
            case "GR": return new Color(1.0f, 0.9f, 0.1f, 1f); // yellow (grass)
            case "GS": return new Color(0.85f, 0.95f, 0.2f, 1f); // yellow-green (grass-shrub)
            case "SH": return new Color(0.5f, 0.85f, 0.3f, 1f); // light green (shrub)
            case "TU": return new Color(0.2f, 0.6f, 0.2f, 1f); // green (timber understory)
            case "TL": return new Color(0.15f, 0.5f, 0.15f, 1f); // darker green (timber litter)
            case "SB": return new Color(0.55f, 0.4f, 0.2f, 1f); // brown (slash/blowdown)
            case "NB": return new Color(0.6f, 0.6f, 0.6f, 1f); // gray (nonburnable)
            case "AG": return new Color(1.0f, 0.6f, 0.8f, 1f); // pink (agriculture)
            case "UR": return new Color(0.5f, 0.0f, 0.6f, 1f); // purple (urban)
            case "RO": return new Color(0.1f, 0.1f, 0.1f, 1f); // black (roads)
            case "WA": return new Color(0.3f, 0.5f, 0.9f, 1f); // blue (water)
            default: return new Color(0.6f, 0.7f, 0.6f, 1f);
        }
    }
}
