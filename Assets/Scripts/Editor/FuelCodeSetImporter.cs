using UnityEditor;
using UnityEngine;
using System.IO;

/// <summary>
/// Import fuelcodes_export.json into a selected FuelCodeSet asset.
/// Creates subassets for each FuelCodeData so the list shows valid references (no type mismatch).
/// </summary>
public static class FuelCodeSetImporter
{
    [MenuItem("Tools/Import Fuel Codes JSON")]
    public static void ImportJson()
    {
        var selected = Selection.activeObject as FuelCodeSet;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("Select FuelCodeSet", "Select a FuelCodeSet asset in the Project window first.", "OK");
            return;
        }

        string path = EditorUtility.OpenFilePanel("Select fuelcodes_export.json", Application.dataPath, "json");
        if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;

        string jsonText = File.ReadAllText(path);
        ImportIntoAsset(selected, jsonText);
    }

    private static void ImportIntoAsset(FuelCodeSet set, string jsonText)
    {
        var assetPath = AssetDatabase.GetAssetPath(set);

        // Clear list and remove existing FuelCodeData subassets
        set.fuelCodes.Clear();
        {
            var subAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            foreach (var sa in subAssets)
            {
                if (sa is FuelCodeData)
                {
                    Object.DestroyImmediate(sa, allowDestroyingAssets: true);
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        // Parse JSON using existing loader to get in-memory instances
        var temp = ScriptableObject.CreateInstance<FuelCodeSet>();
        temp.LoadFromJsonText(jsonText);

        // Add each FuelCodeData as a subasset, then reference it in the main list
        foreach (var fc in temp.fuelCodes)
        {
            fc.hideFlags = HideFlags.None;
            // Name subasset for easy identification in inspector
            var displayName = !string.IsNullOrEmpty(fc.title)
                ? $"{fc.title} ({fc.codeGIS})"
                : fc.codeGIS;
            fc.name = displayName;
            // Assign default base color based on fuel group prefix
            fc.baseColor = GetBaseColorForFuel(fc.title, fc.codeGIS);
            AssetDatabase.AddObjectToAsset(fc, set);
            set.fuelCodes.Add(fc);
        }

        Object.DestroyImmediate(temp);

        FuelCodeFamilyAutoTuner.Apply(set, 10f, MoistureState.Medium);
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        EditorUtility.DisplayDialog("Import complete", "Fuel codes imported into the selected FuelCodeSet (as subassets).", "OK");
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
