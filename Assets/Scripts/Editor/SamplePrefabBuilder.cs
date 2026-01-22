using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Builds sample prefabs for LinePath+Manager and Unit.
/// Menus:
///  - Tools/Build Sample LinePath Prefab
///  - Tools/Build Sample LinePathManager Prefab
///  - Tools/Build Sample Unit Prefab
/// </summary>
public class SamplePrefabBuilder : EditorWindow
{
    private const string linePathPrefabPath = "Assets/Prefabs/SampleLinePath.prefab";
    private const string linePathManagerPrefabPath = "Assets/Prefabs/SampleLinePathManager.prefab";
    private const string unitPrefabPath = "Assets/Prefabs/SampleUnit.prefab";

    [MenuItem("Tools/Build Sample LinePath Prefab")]
    public static void BuildLinePathPrefab()
    {
        Directory.CreateDirectory("Assets/Prefabs");

        var go = new GameObject("SampleLinePath");
        try
        {
            var lp = go.AddComponent<LinePath>();
            var lr = go.GetComponent<LineRenderer>();
            if (lr == null) lr = go.AddComponent<LineRenderer>();
            lr.widthMultiplier = 0.2f;
            var lit = Shader.Find("Universal Render Pipeline/Lit");
            var def = Shader.Find("Sprites/Default");
            lr.material = new Material(lit != null ? lit : def);

            // Create two points as children
            var p0 = new GameObject("Point0");
            p0.transform.parent = go.transform;
            p0.transform.localPosition = Vector3.zero;
            p0.AddComponent<LinePathPoint>();

            var p1 = new GameObject("Point1");
            p1.transform.parent = go.transform;
            p1.transform.localPosition = new Vector3(5f, 0f, 0f);
            p1.AddComponent<LinePathPoint>();

            // Assign positions directly to avoid null refs during creation
            lp.points.Clear();
            lp.points.Add(p0.GetComponent<LinePathPoint>());
            lp.points.Add(p1.GetComponent<LinePathPoint>());
            var posList = new System.Collections.Generic.List<Vector3>
            {
                p0.transform.position,
                p1.transform.position
            };
            lp.SetPoints(posList);

            PrefabUtility.SaveAsPrefabAsset(go, linePathPrefabPath);
            EditorUtility.DisplayDialog("Prefab created", $"Saved to {linePathPrefabPath}", "OK");
        }
        finally
        {
            Object.DestroyImmediate(go);
        }
    }

    [MenuItem("Tools/Build Sample LinePathManager Prefab")]
    public static void BuildLinePathManagerPrefab()
    {
        Directory.CreateDirectory("Assets/Prefabs");

        var go = new GameObject("SampleLinePathManager");
        try
        {
            go.AddComponent<LinePathManager>();
            PrefabUtility.SaveAsPrefabAsset(go, linePathManagerPrefabPath);
            EditorUtility.DisplayDialog("Prefab created", $"Saved to {linePathManagerPrefabPath}", "OK");
        }
        finally
        {
            Object.DestroyImmediate(go);
        }
    }

    [MenuItem("Tools/Build Sample Unit Prefab")]
    public static void BuildUnitPrefab()
    {
        Directory.CreateDirectory("Assets/Prefabs");

        var go = new GameObject("SampleUnit");
        try
        {
            var unit = go.AddComponent<Unit>();
            var lvl = go.AddComponent<UnitLeveling>();
            unit.unitData = CreateTempUnitDataAsset();
            // Basic level
            lvl.levels.Add(new UnitLevel { level = 1, firepointsCost = 0, moveSpeedMult = 1f, waterCapacityMult = 1f, suppressionSpeedMult = 1f, cutSpeedMult = 1f });
            lvl.firepoints = 0;

            PrefabUtility.SaveAsPrefabAsset(go, unitPrefabPath);
            EditorUtility.DisplayDialog("Prefab created", $"Saved to {unitPrefabPath}", "OK");
        }
        finally
        {
            Object.DestroyImmediate(go);
        }
    }

    private static UnitData CreateTempUnitDataAsset()
    {
        var asset = ScriptableObject.CreateInstance<UnitData>();
        asset.id = "unit_sample";
        asset.displayName = "Sample Unit";
        asset.unitType = "groundcrew";
        asset.agency = "groundcrew";
        asset.moveSpeed = 5f;
        asset.cutSpeed = 1f;
        asset.maxSlope = 35f;
        asset.waterCapacity = 500f;
        asset.waterInputAmount = 50f;
        asset.waterOutputAmount = 50f;
        asset.suppressionSpeed = 10f;
        asset.effectiveRadius = 30f;

        var path = "Assets/Prefabs/SampleUnitData.asset";
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return asset;
    }
}
