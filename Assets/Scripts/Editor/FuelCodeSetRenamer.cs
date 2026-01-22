using UnityEditor;
using UnityEngine;

/// <summary>
/// Renames FuelCodeData subassets in a FuelCodeSet to "Title (Code)" for clarity.
/// </summary>
public static class FuelCodeSetRenamer
{
    [MenuItem("Tools/Rename FuelCodeSet Entries")]
    public static void RenameSelected()
    {
        var set = Selection.activeObject as FuelCodeSet;
        if (set == null)
        {
            EditorUtility.DisplayDialog("Select FuelCodeSet", "Select a FuelCodeSet asset in the Project window first.", "OK");
            return;
        }

        int renamed = 0;
        foreach (var fc in set.fuelCodes)
        {
            if (fc == null) continue;
            var displayName = !string.IsNullOrEmpty(fc.title)
                ? $"{fc.title} ({fc.codeGIS})"
                : fc.codeGIS;
            if (fc.name != displayName)
            {
                fc.name = displayName;
                EditorUtility.SetDirty(fc);
                renamed++;
            }
        }

        EditorUtility.SetDirty(set);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Rename Complete", $"Renamed {renamed} entries.", "OK");
    }
}
