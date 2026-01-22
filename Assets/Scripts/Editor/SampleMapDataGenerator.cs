using UnityEditor;
using UnityEngine;

/// <summary>
/// Generates sample elevation + fuel code data into the current scene's MapData.
/// Elevation: Perlin-based hills; Fuel: bands based on Perlin and available fuel codes.
/// </summary>
public static class SampleMapDataGenerator
{
[MenuItem("Tools/Generate Sample Map Data", false, 999)]
    public static void Generate()
    {
        var md = Object.FindObjectOfType<MapData>();
        var tg = Object.FindObjectOfType<TerrainGenerator>();
        var gm = Object.FindObjectOfType<GameManager>();
        if (md == null || tg == null || gm == null)
        {
            EditorUtility.DisplayDialog("Missing", "Need MapData, TerrainGenerator, and GameManager in the scene.", "OK");
            return;
        }
        if (md.elevationLayer == null || md.fuelCodeLayer == null)
        {
            EditorUtility.DisplayDialog("Missing Layers", "Assign ElevationLayer and FuelCodeLayer to MapData.", "OK");
            return;
        }
        if (gm.fuelCodeManager == null || gm.fuelCodeManager.fuelCodeSet == null || gm.fuelCodeManager.fuelCodeSet.fuelCodes.Count == 0)
        {
            EditorUtility.DisplayDialog("Missing FuelCodeSet", "Assign a FuelCodeSet to FuelCodeManager with imported fuel codes.", "OK");
            return;
        }

        int w = md.xWidth;
        int h = md.zWidth;
        if (w <= 0 || h <= 0)
        {
            EditorUtility.DisplayDialog("Invalid Dimensions", "Set MapData xWidth/zWidth to positive values.", "OK");
            return;
        }

        // Generate elevation: Perlin hills
        float scale = 0.008f;
        float amp = 800f;
        var elevLayer = md.elevationLayer;
        var fuelLayer = md.fuelCodeLayer;

        // Choose a few fuel codes from the set
        var codes = gm.fuelCodeManager.fuelCodeSet.fuelCodes;
        int codeCount = codes.Count;

        for (int x = 0; x < w; x++)
        {
            for (int z = 0; z < h; z++)
            {
                float n = Mathf.PerlinNoise(x * scale, z * scale);
                short elev = (short)Mathf.Round(Mathf.Pow(n, 1.5f) * amp);
                elevLayer.SetElevation(x, z, elev);

                // Fuel bands: mix based on Perlin
                float nf = Mathf.PerlinNoise((x + 1000) * (scale * 1.7f), (z + 1000) * (scale * 1.7f));
                int idx = Mathf.Clamp(Mathf.FloorToInt(nf * codeCount), 0, codeCount - 1);
                fuelLayer.SetFuelCode(x, z, codes[idx].fuelCodeID);
            }
        }

    tg.BuildAllTilesImmediate();
    EditorUtility.DisplayDialog("Sample Data Generated", "Perlin hills and fuel bands applied.", "OK");
    }
}
