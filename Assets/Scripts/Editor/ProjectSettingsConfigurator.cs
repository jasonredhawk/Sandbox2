using UnityEditor;
using UnityEngine;

/// <summary>
/// One-click application of default project settings per Phase 1 checklist.
/// Run via Tools/Apply Default Project Settings.
/// </summary>
public static class ProjectSettingsConfigurator
{
    [MenuItem("Tools/Apply Default Project Settings")]
    public static void ApplyDefaults()
    {
        ApplyPlayerSettings();
        ApplyTimeSettings();
        ApplyQualitySettings();
        ApplyPhysicsSettings();
        ApplyLayers();

        Debug.Log("Project settings applied.");
    }

    private static void ApplyPlayerSettings()
    {
        PlayerSettings.runInBackground = true;
        PlayerSettings.fullScreenMode = FullScreenMode.FullScreenWindow;
        PlayerSettings.defaultScreenWidth = 1920;
        PlayerSettings.defaultScreenHeight = 1080;
        PlayerSettings.companyName = "WildlandFireSim";
        PlayerSettings.productName = "Wildland Firefighting Simulator";
    }

    private static void ApplyTimeSettings()
    {
        Time.fixedDeltaTime = 0.02f;       // 50 Hz
        Time.maximumDeltaTime = 0.1f;
    }

    private static void ApplyQualitySettings()
    {
        // Apply to current quality level only (user can copy to others as needed).
        QualitySettings.vSyncCount = 0;
        QualitySettings.shadowDistance = 150f;
        QualitySettings.lodBias = 1.0f;
    }

    private static void ApplyPhysicsSettings()
    {
        Physics.gravity = new Vector3(0f, -9.81f, 0f);
        Physics.defaultSolverIterations = 6;
        Physics.defaultSolverVelocityIterations = 1;
    }

    private static void ApplyLayers()
    {
        // Layers 6–11: Terrain, Units, LinePaths, Water, Fire, UI
        SetLayer(6, "Terrain");
        SetLayer(7, "Units");
        SetLayer(8, "LinePaths");
        SetLayer(9, "Water");
        SetLayer(10, "Fire");
        SetLayer(11, "UI");
    }

    private static void SetLayer(int index, string layerName)
    {
        if (index < 0 || index > 31)
        {
            Debug.LogWarning($"Layer index {index} out of range.");
            return;
        }

        var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        var layersProp = tagManager.FindProperty("layers");
        if (layersProp != null && layersProp.arraySize > index)
        {
            var sp = layersProp.GetArrayElementAtIndex(index);
            if (sp != null && sp.stringValue != layerName)
            {
                sp.stringValue = layerName;
                tagManager.ApplyModifiedProperties();
            }
        }
    }
}
