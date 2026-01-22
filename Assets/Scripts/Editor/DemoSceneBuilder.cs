using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Builds a quick demo scene with camera, light, GameRoot prefab (if present),
/// NetworkManager prefab (if present), SampleLinePathManager/LinePath, and SampleUnit.
/// If prefabs are missing, creates an in-scene fallback GameRoot with temporary layers/material.
/// </summary>
public static class DemoSceneBuilder
{
    [MenuItem("Tools/Create Demo Scene")]
    public static void CreateDemoScene()
    {
        // New untitled scene
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Camera
        var camGO = new GameObject("Main Camera");
        var cam = camGO.AddComponent<Camera>();
        camGO.tag = "MainCamera";
        cam.transform.position = new Vector3(0f, 50f, -100f);
        cam.transform.LookAt(Vector3.zero);

        // Light
        var lightGO = new GameObject("Directional Light");
        var light = lightGO.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.1f;
        light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        // Instantiate prefabs if they exist
        var hasGameRootPrefab = TryInstantiatePrefab("Assets/Prefabs/GameRoot.prefab", "GameRoot");
        TryInstantiatePrefab("Assets/Prefabs/NetworkManager.prefab", "NetworkManager");
        TryInstantiatePrefab("Assets/Prefabs/SampleLinePathManager.prefab", "SampleLinePathManager");
        TryInstantiatePrefab("Assets/Prefabs/SampleLinePath.prefab", "SampleLinePath");
        TryInstantiatePrefab("Assets/Prefabs/SampleUnit.prefab", "SampleUnit");

        // If no GameRoot prefab instantiated, build a temporary in-scene GameRoot
        if (!hasGameRootPrefab && GameObject.FindObjectOfType<GameManager>() == null)
        {
            BuildTempGameRoot();
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorUtility.DisplayDialog("Demo Scene", "Demo scene created. Press Play to view the terrain.", "OK");
    }

    private static bool TryInstantiatePrefab(string path, string name)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab != null)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.name = name;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Builds an in-scene GameRoot with temporary layers/material so terrain renders even without prefabs.
    /// </summary>
    private static void BuildTempGameRoot()
    {
        var root = new GameObject("GameRoot");
        var gm = root.AddComponent<GameManager>();
        var md = root.AddComponent<MapData>();
        var tg = root.AddComponent<TerrainGenerator>();
        root.AddComponent<NetworkManagerGO>();

        // Temporary layers (not saved as assets)
        var elev = ScriptableObject.CreateInstance<ElevationLayer>();
        var fuel = ScriptableObject.CreateInstance<FuelCodeLayer>();
        elev.tileSize = 122;
        fuel.tileSize = 122;

        md.elevationLayer = elev;
        md.fuelCodeLayer = fuel;
        md.Initialize(256, 256, 0, 0, 30, -30);

        // Fill defaults
        md.Fill(0, 98);

        // Fallback material
        var lit = Shader.Find("Universal Render Pipeline/Lit");
        var def = Shader.Find("Standard");
        var mat = new Material(lit != null ? lit : def);

        tg.mapData = md;
        tg.elevationLayer = elev;
        tg.terrainMaterial = mat;

        gm.mapData = md;
        gm.terrainGenerator = tg;
        if (gm.fuelCodeManager == null)
        {
            gm.fuelCodeManager = gm.gameObject.AddComponent<FuelCodeManager>();
        }

        // Build tiles immediately
        tg.BuildAllTilesImmediate();
    }
}
