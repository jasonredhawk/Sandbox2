using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor window to create and wire a GameRoot in the current scene.
/// </summary>
public class GameRootCreatorWindow : EditorWindow
{
    private ElevationLayer elevationLayer;
    private FuelCodeLayer fuelCodeLayer;
    private Material terrainMaterial;
    private FuelCodeSet fuelCodeSet;

    private int xWidth = 256;
    private int zWidth = 256;
    private int startLongitudeMeter = 0;
    private int startLatitudeMeter = 0;
    private int longitudeMeterStep = 30;
    private int latitudeMeterStep = -30;

    [MenuItem("Tools/Create GameRoot")]
    public static void ShowWindow()
    {
        GetWindow<GameRootCreatorWindow>("Create GameRoot");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("GameRoot Builder", EditorStyles.boldLabel);

        elevationLayer = (ElevationLayer)EditorGUILayout.ObjectField("ElevationLayer", elevationLayer, typeof(ElevationLayer), false);
        fuelCodeLayer = (FuelCodeLayer)EditorGUILayout.ObjectField("FuelCodeLayer", fuelCodeLayer, typeof(FuelCodeLayer), false);
        terrainMaterial = (Material)EditorGUILayout.ObjectField("Terrain Material", terrainMaterial, typeof(Material), false);
        fuelCodeSet = (FuelCodeSet)EditorGUILayout.ObjectField("FuelCodeSet (optional)", fuelCodeSet, typeof(FuelCodeSet), false);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Map Dimensions (pixels)", EditorStyles.boldLabel);
        xWidth = EditorGUILayout.IntField("xWidth", xWidth);
        zWidth = EditorGUILayout.IntField("zWidth", zWidth);
        startLongitudeMeter = EditorGUILayout.IntField("startLongitudeMeter", startLongitudeMeter);
        startLatitudeMeter = EditorGUILayout.IntField("startLatitudeMeter", startLatitudeMeter);
        longitudeMeterStep = EditorGUILayout.IntField("longitudeMeterStep", longitudeMeterStep);
        latitudeMeterStep = EditorGUILayout.IntField("latitudeMeterStep", latitudeMeterStep);

        EditorGUILayout.Space();
        if (GUILayout.Button("Create GameRoot in Scene"))
        {
            CreateGameRoot();
        }
    }

    private void CreateGameRoot()
    {
        if (FindObjectOfType<GameManager>() != null)
        {
            if (!EditorUtility.DisplayDialog("GameManager exists", "A GameManager already exists in the scene. Create another GameRoot anyway?", "Yes", "No"))
                return;
        }

        var root = new GameObject("GameRoot");
        Undo.RegisterCreatedObjectUndo(root, "Create GameRoot");

        var gm = root.AddComponent<GameManager>();
        var md = root.AddComponent<MapData>();
        var tg = root.AddComponent<TerrainGenerator>();
        root.AddComponent<NetworkManagerGO>();

        // Assign assets
        md.elevationLayer = elevationLayer;
        md.fuelCodeLayer = fuelCodeLayer;
        md.Initialize(xWidth, zWidth, startLongitudeMeter, startLatitudeMeter, longitudeMeterStep, latitudeMeterStep);

        tg.mapData = md;
        tg.elevationLayer = elevationLayer;
        tg.terrainMaterial = terrainMaterial;

        gm.mapData = md;
        gm.terrainGenerator = tg;
        if (gm.fuelCodeManager == null)
        {
            gm.fuelCodeManager = gm.gameObject.AddComponent<FuelCodeManager>();
        }
        gm.fuelCodeManager.fuelCodeSet = fuelCodeSet;

        // If fuel set is assigned, fill default fuel code (first in set), else use 98
        short defaultFuel = 98;
        if (fuelCodeSet != null && fuelCodeSet.fuelCodes.Count > 0)
        {
            defaultFuel = fuelCodeSet.fuelCodes[0].fuelCodeID;
        }
        // Fill layers with defaults (0 elevation)
        md.Fill(0, defaultFuel);

        // Ensure a LinePathManager exists in the scene
        if (FindObjectOfType<LinePathManager>() == null)
        {
            var lpmGO = new GameObject("LinePathManager");
            Undo.RegisterCreatedObjectUndo(lpmGO, "Create LinePathManager");
            lpmGO.AddComponent<LinePathManager>();
        }

        // Ensure a NetworkManager prefab is placed if present in project (optional)
        var nmPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/NetworkManager.prefab");
        if (nmPrefab != null && FindObjectOfType<Unity.Netcode.NetworkManager>() == null)
        {
            var nm = (GameObject)PrefabUtility.InstantiatePrefab(nmPrefab);
            Undo.RegisterCreatedObjectUndo(nm, "Instantiate NetworkManager");
        }

        // Ensure a GISDataParser is available if user wants to load XYZ (optional)
        if (FindObjectOfType<GISDataParser>() == null)
        {
            var parserGO = new GameObject("GISDataParser");
            Undo.RegisterCreatedObjectUndo(parserGO, "Create GISDataParser");
            parserGO.AddComponent<GISDataParser>();
        }

        Selection.activeGameObject = root;
        EditorUtility.DisplayDialog("GameRoot created", "GameRoot has been added to the scene and wired.", "OK");
    }
}
