using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Show source of truth info
        var mapDataProp = serializedObject.FindProperty("mapData");
        if (mapDataProp != null && mapDataProp.objectReferenceValue != null)
        {
            var md = (MapData)mapDataProp.objectReferenceValue;
            EditorGUILayout.HelpBox($"Tile Size (source of truth): {md.tileSize}", MessageType.Info);
        }

        var tg = (TerrainGenerator)target;
        EditorGUILayout.Space();
        if (GUILayout.Button("Generate Sample Map Data + Build"))
        {
            // Resolve references safely (even if not assigned on TerrainGenerator)
            var md = tg.mapData != null ? tg.mapData : Object.FindObjectOfType<MapData>();
            var gm = tg.gameManager != null ? tg.gameManager : Object.FindObjectOfType<GameManager>();
            if (md == null)
            {
                EditorUtility.DisplayDialog("Missing MapData", "Could not find MapData in the scene.", "OK");
                return;
            }
            if (gm == null)
            {
                EditorUtility.DisplayDialog("Missing GameManager", "Could not find GameManager in the scene.", "OK");
                return;
            }
            if (md.elevationLayer == null || md.fuelCodeLayer == null)
            {
                EditorUtility.DisplayDialog("Missing Layers", "MapData is missing ElevationLayer or FuelCodeLayer.", "OK");
                return;
            }
            if (gm.fuelCodeManager == null || gm.fuelCodeManager.fuelCodeSet == null || gm.fuelCodeManager.fuelCodeSet.fuelCodes.Count == 0)
            {
                EditorUtility.DisplayDialog("Missing FuelCodeSet", "GameManager -> FuelCodeManager needs a FuelCodeSet with entries.", "OK");
                return;
            }

            int w = md.xWidth;
            int h = md.zWidth;
            if (w <= 0 || h <= 0)
            {
                EditorUtility.DisplayDialog("Invalid Dimensions", "Set MapData xWidth/zWidth to > 0 in the scene.", "OK");
                return;
            }

            // Elevation: Perlin hills
            float scale = 0.008f;
            float amp = 800f;
            var elevLayer = md.elevationLayer;
            var fuelLayer = md.fuelCodeLayer;
            var codes = gm.fuelCodeManager.fuelCodeSet.fuelCodes;
            // Use many fuel codes by mapping noise to index
            int codeCount = codes.Count;

            for (int x = 0; x < w; x++)
            {
                for (int z = 0; z < h; z++)
                {
                    float n = Mathf.PerlinNoise(x * scale, z * scale);
                    short elev = (short)Mathf.Round(Mathf.Pow(n, 1.5f) * amp);
                    elevLayer.SetElevation(x, z, elev);

                    // Multi-band fuel distribution
                    float nf = Mathf.PerlinNoise((x + 1000) * (scale * 1.7f), (z + 1000) * (scale * 1.7f));
                    int idx = Mathf.Clamp(Mathf.FloorToInt(nf * codeCount), 0, codeCount - 1);
                    fuelLayer.SetFuelCode(x, z, codes[idx].fuelCodeID);
                }
            }

            // Ensure TerrainGenerator has references
            tg.mapData = md;
            tg.gameManager = gm;
            tg.fuelCodeLayer = md.fuelCodeLayer;
            tg.fuelCodeSet = gm.fuelCodeManager.fuelCodeSet;

            tg.BuildAllTilesImmediate();
            EditorUtility.DisplayDialog("Generated", "Sample elevation + fuel data generated and tiles rebuilt.", "OK");
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Fuel colors are rendered via vertex colors using Sprites/Default shader. If you assigned a URP/Lit material, colors will not show. This script replaces the material with a vertex-color shader when building tiles.", MessageType.Info);
    }
}
