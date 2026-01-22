using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Builds a GameRoot prefab with all core components wired.
/// Menu: Tools/Build GameRoot Prefab
/// </summary>
public class GameRootPrefabBuilder : EditorWindow
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

    private const string prefabPath = "Assets/Prefabs/GameRoot.prefab";

    [MenuItem("Tools/Build GameRoot Prefab")]
    public static void ShowWindow()
    {
        GetWindow<GameRootPrefabBuilder>("Build GameRoot Prefab");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("GameRoot Prefab Builder", EditorStyles.boldLabel);
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
        if (GUILayout.Button("Create Prefab at " + prefabPath))
        {
            CreatePrefab();
        }
    }

    private void CreatePrefab()
    {
        if (elevationLayer == null || fuelCodeLayer == null || terrainMaterial == null)
        {
            EditorUtility.DisplayDialog("Missing references", "Assign ElevationLayer, FuelCodeLayer, and Terrain Material.", "OK");
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(prefabPath));

        var root = new GameObject("GameRoot");
        try
        {
            var gm = root.AddComponent<GameManager>();
            var md = root.AddComponent<MapData>();
            var tg = root.AddComponent<TerrainGenerator>();
            root.AddComponent<NetworkManagerGO>();
            var fcm = gm.gameObject.AddComponent<FuelCodeManager>();

            // Assign data
            md.elevationLayer = elevationLayer;
            md.fuelCodeLayer = fuelCodeLayer;
            md.xWidth = xWidth;
            md.zWidth = zWidth;
            md.startLongitudeMeter = startLongitudeMeter;
            md.startLatitudeMeter = startLatitudeMeter;
            md.longitudeMeterStep = longitudeMeterStep;
            md.latitudeMeterStep = latitudeMeterStep;
            md.xCount = Mathf.CeilToInt(xWidth / (float)elevationLayer.tileSize);
            md.zCount = Mathf.CeilToInt(zWidth / (float)elevationLayer.tileSize);

            tg.mapData = md;
            tg.elevationLayer = elevationLayer;
            tg.terrainMaterial = terrainMaterial;

            gm.mapData = md;
            gm.terrainGenerator = tg;
            fcm.fuelCodeSet = fuelCodeSet;
            gm.fuelCodeManager = fcm;

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            EditorUtility.DisplayDialog("Prefab created", $"GameRoot prefab saved to {prefabPath}", "OK");
        }
        finally
        {
            DestroyImmediate(root);
        }
    }
}
