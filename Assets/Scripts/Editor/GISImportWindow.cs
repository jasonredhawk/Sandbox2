using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GISImportWindow : EditorWindow
{
    [Header("Targets")]
    private MapData mapData;
    private ElevationLayer elevationLayer;
    private FuelCodeLayer fuelCodeLayer;
    private TerrainGenerator terrainGenerator;

    [Header("Raster Sources (GeoTIFF/PNG)")]
    private Texture2D elevationTexture;
    private Texture2D fuelTexture;
    private float elevationScale = 1f;
    private float elevationOffset = 0f;
    private float elevationMin = 0f;
    private float elevationMax = 3000f;

    [Header("Fuel Matching")]
    private FuelCodeSet fuelCodeSet;
    private short defaultFuelCode = 98;
    private float alphaIgnoreThreshold = 0.01f;
    private ColorDistanceMode colorDistanceMode = ColorDistanceMode.Rgb;

    [Header("XYZ Sources")]
    private TextAsset elevationXYZ;
    private TextAsset fuelXYZ;
    private string elevationXYZPath;
    private string fuelXYZPath;
    private bool xyzAutoDetectBounds = true;

    [Header("Geo Settings")]
    private int startLongitudeMeter = 0;
    private int startLatitudeMeter = 0;
    private int meterStep = 1;
    private int tileSize = 122;
    private bool flipVertical = true;
    private bool negativeLatitudeStep = true;

    private enum ColorDistanceMode
    {
        Rgb = 0,
        Hsv = 1
    }

    [MenuItem("Tools/GIS Importer")]
    public static void Open()
    {
        GetWindow<GISImportWindow>("GIS Importer");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Targets", EditorStyles.boldLabel);
        mapData = (MapData)EditorGUILayout.ObjectField("Map Data", mapData, typeof(MapData), true);
        elevationLayer = (ElevationLayer)EditorGUILayout.ObjectField("Elevation Layer", elevationLayer, typeof(ElevationLayer), false);
        fuelCodeLayer = (FuelCodeLayer)EditorGUILayout.ObjectField("Fuel Code Layer", fuelCodeLayer, typeof(FuelCodeLayer), false);
        terrainGenerator = (TerrainGenerator)EditorGUILayout.ObjectField("Terrain Generator", terrainGenerator, typeof(TerrainGenerator), true);

        EnsureXYZTargets();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Geo Settings", EditorStyles.boldLabel);
        startLongitudeMeter = EditorGUILayout.IntField("Start Longitude (m)", startLongitudeMeter);
        startLatitudeMeter = EditorGUILayout.IntField("Start Latitude (m)", startLatitudeMeter);
        meterStep = EditorGUILayout.IntField("Meter Step", meterStep);
        tileSize = EditorGUILayout.IntField("Tile Size", tileSize);
        flipVertical = EditorGUILayout.Toggle("Flip Vertical (Top-Left)", flipVertical);
        negativeLatitudeStep = EditorGUILayout.Toggle("Negative Latitude Step", negativeLatitudeStep);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Raster Import (GeoTIFF/PNG)", EditorStyles.boldLabel);
        elevationTexture = (Texture2D)EditorGUILayout.ObjectField("Elevation Texture", elevationTexture, typeof(Texture2D), false);
        fuelTexture = (Texture2D)EditorGUILayout.ObjectField("Fuel Texture", fuelTexture, typeof(Texture2D), false);
        elevationScale = EditorGUILayout.FloatField("Elevation Scale", elevationScale);
        elevationOffset = EditorGUILayout.FloatField("Elevation Offset", elevationOffset);
        elevationMin = EditorGUILayout.FloatField("Elevation Min (m)", elevationMin);
        elevationMax = EditorGUILayout.FloatField("Elevation Max (m)", elevationMax);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Select Elevation TIF..."))
            {
                elevationTexture = ImportGeoTiffTexture("Select Elevation GeoTIFF", "Elevation");
            }
            if (GUILayout.Button("Select Fuel TIF..."))
            {
                fuelTexture = ImportGeoTiffTexture("Select Fuel GeoTIFF", "Fuel");
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Fuel Matching", EditorStyles.boldLabel);
        fuelCodeSet = (FuelCodeSet)EditorGUILayout.ObjectField("Fuel Code Set", fuelCodeSet, typeof(FuelCodeSet), false);
        defaultFuelCode = (short)EditorGUILayout.IntField("Default Fuel Code", defaultFuelCode);
        alphaIgnoreThreshold = EditorGUILayout.Slider("Alpha Ignore Threshold", alphaIgnoreThreshold, 0f, 1f);
        colorDistanceMode = (ColorDistanceMode)EditorGUILayout.EnumPopup("Color Distance Mode", colorDistanceMode);
        int paletteCount = fuelCodeSet != null ? fuelCodeSet.fuelCodes.Count : 0;
        EditorGUILayout.LabelField("Palette Count", paletteCount.ToString());
        EditorGUILayout.HelpBox("GeoTIFFs are imported as textures (Geo metadata ignored). Ensure they are readable/uncompressed.", MessageType.Info);

        using (new EditorGUI.DisabledScope(!IsReadyForRaster()))
        {
            if (GUILayout.Button("Import Raster"))
            {
                ImportRaster();
            }
        }

        using (new EditorGUI.DisabledScope(mapData == null || elevationLayer == null || fuelCodeLayer == null))
        {
            if (GUILayout.Button("Rebuild Terrain Now"))
            {
                RebuildTerrain();
            }
        }

        using (new EditorGUI.DisabledScope(mapData == null || elevationLayer == null || fuelCodeLayer == null))
        {
            if (GUILayout.Button("Run Raster Smoke Test (4x4)"))
            {
                RunRasterSmokeTest();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("XYZ Import", EditorStyles.boldLabel);
        elevationXYZ = (TextAsset)EditorGUILayout.ObjectField("Elevation XYZ (optional)", elevationXYZ, typeof(TextAsset), false);
        fuelXYZ = (TextAsset)EditorGUILayout.ObjectField("Fuel XYZ (optional)", fuelXYZ, typeof(TextAsset), false);
        xyzAutoDetectBounds = EditorGUILayout.Toggle("Auto-Detect Bounds", xyzAutoDetectBounds);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Select Elevation XYZ..."))
            {
                SelectXYZFile("Select Elevation XYZ", ref elevationXYZPath);
            }
            if (GUILayout.Button("Select Fuel XYZ..."))
            {
                SelectXYZFile("Select Fuel XYZ", ref fuelXYZPath);
            }
        }
        if (!string.IsNullOrEmpty(elevationXYZPath))
        {
            EditorGUILayout.LabelField("Elevation XYZ Path", elevationXYZPath);
        }
        if (!string.IsNullOrEmpty(fuelXYZPath))
        {
            EditorGUILayout.LabelField("Fuel XYZ Path", fuelXYZPath);
        }
        EditorGUILayout.HelpBox("If nothing appears, verify meterStep/start coords or enable Auto-Detect Bounds.", MessageType.Info);

        using (new EditorGUI.DisabledScope(!IsReadyForXYZ()))
        {
            if (GUILayout.Button("Import XYZ"))
            {
                ImportXYZ();
            }
        }
    }

    private bool IsReadyForRaster()
    {
        return mapData != null && elevationLayer != null && fuelCodeLayer != null &&
               elevationTexture != null && fuelTexture != null;
    }

    private bool IsReadyForXYZ()
    {
        bool hasElev = elevationXYZ != null || File.Exists(elevationXYZPath);
        bool hasFuel = fuelXYZ != null || File.Exists(fuelXYZPath);
        return mapData != null && elevationLayer != null && fuelCodeLayer != null && hasElev && hasFuel;
    }

    private void ImportRaster()
    {
        var elevTex = EnsureReadable(elevationTexture);
        var fuelTex = EnsureReadable(fuelTexture);

        int width = elevTex.width;
        int height = elevTex.height;
        if (fuelTex.width != width || fuelTex.height != height)
        {
            EditorUtility.DisplayDialog("Size mismatch", "Elevation and Fuel textures must have the same dimensions.", "OK");
            return;
        }

        InitializeMap(width, height);
        if (elevationMax <= elevationMin)
        {
            EditorUtility.DisplayDialog("Invalid Elevation Range", "Elevation Max must be greater than Elevation Min.", "OK");
            return;
        }

        if (elevTex.format == TextureFormat.RFloat)
        {
            var data = elevTex.GetPixelData<float>(0);
            for (int y = 0; y < height; y++)
            {
                int z = flipVertical ? (height - 1 - y) : y;
                int row = y * width;
                for (int x = 0; x < width; x++)
                {
                    float raw = data[row + x];
                    float e = raw * elevationScale + elevationOffset;
                    elevationLayer.SetElevation(x, z, (short)Mathf.Clamp(e, short.MinValue, short.MaxValue));
                }
            }
        }
        else
        {
            var pixels = elevTex.GetPixels32();
            for (int y = 0; y < height; y++)
            {
                int z = flipVertical ? (height - 1 - y) : y;
                int row = y * width;
                for (int x = 0; x < width; x++)
                {
                    var c = pixels[row + x];
                    float grey = c.r / 255f;
                    float e = Mathf.Lerp(elevationMin, elevationMax, grey);
                    elevationLayer.SetElevation(x, z, (short)Mathf.Clamp(e, short.MinValue, short.MaxValue));
                }
            }
        }

        var palette = BuildFuelPalette(fuelCodeSet);
        var colorCache = new System.Collections.Generic.Dictionary<int, short>();

        var fuelPixels = fuelTex.GetPixels32();
        for (int y = 0; y < height; y++)
        {
            int z = flipVertical ? (height - 1 - y) : y;
            int row = y * width;
            for (int x = 0; x < width; x++)
            {
                var c = fuelPixels[row + x];
                short code = MatchFuelCode(c, palette, colorCache);
                fuelCodeLayer.SetFuelCode(x, z, code);
            }
        }

        EditorUtility.SetDirty(elevationLayer);
        EditorUtility.SetDirty(fuelCodeLayer);
        EditorUtility.SetDirty(mapData);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        RebuildTerrain();
        EditorUtility.DisplayDialog("Import complete", "Raster import finished.", "OK");
    }

    private void ImportXYZ()
    {
        EnsureXYZTargets();
        string elevText = ReadXYZText(elevationXYZ, elevationXYZPath);
        string fuelText = ReadXYZText(fuelXYZ, fuelXYZPath);
        if (string.IsNullOrEmpty(elevText) || string.IsNullOrEmpty(fuelText))
        {
            EditorUtility.DisplayDialog("Missing XYZ", "Elevation or Fuel XYZ text could not be read.", "OK");
            return;
        }
        if (mapData == null || elevationLayer == null || fuelCodeLayer == null)
        {
            EditorUtility.DisplayDialog("Missing Targets", "MapData/ElevationLayer/FuelCodeLayer not assigned.", "OK");
            return;
        }
        int elevCount;
        int fuelCount;
        InitializeMapFromXYZ(elevText, fuelText, out elevCount, out fuelCount);
        RebuildTerrain();
        EditorUtility.DisplayDialog(
            "Import complete",
            $"XYZ import finished.\nElevation points: {elevCount}\nFuel points: {fuelCount}",
            "OK");
    }

    private void InitializeMap(int width, int height)
    {
        if (meterStep <= 0) meterStep = 1;
        mapData.tileSize = tileSize > 0 ? tileSize : mapData.tileSize;
        mapData.Initialize(width, height, startLongitudeMeter, startLatitudeMeter, meterStep, negativeLatitudeStep ? -meterStep : meterStep);

        elevationLayer.width = width;
        elevationLayer.height = height;
        elevationLayer.tileSize = mapData.tileSize;
        elevationLayer.startLongitudeMeter = startLongitudeMeter;
        elevationLayer.startLatitudeMeter = startLatitudeMeter;
        elevationLayer.longitudeMeterStep = meterStep;
        elevationLayer.latitudeMeterStep = negativeLatitudeStep ? -meterStep : meterStep;

        fuelCodeLayer.width = width;
        fuelCodeLayer.height = height;
        fuelCodeLayer.tileSize = mapData.tileSize;
        fuelCodeLayer.startLongitudeMeter = startLongitudeMeter;
        fuelCodeLayer.startLatitudeMeter = startLatitudeMeter;
        fuelCodeLayer.longitudeMeterStep = meterStep;
        fuelCodeLayer.latitudeMeterStep = negativeLatitudeStep ? -meterStep : meterStep;
    }

    private void InitializeMapFromXYZ(string elevText, string fuelText, out int elevationCount, out int fuelCount)
    {
        if (meterStep <= 0) meterStep = 1;
        mapData.tileSize = tileSize > 0 ? tileSize : mapData.tileSize;

        if (xyzAutoDetectBounds)
        {
            int minX = int.MaxValue;
            int maxX = int.MinValue;
            int minZ = int.MaxValue;
            int maxZ = int.MinValue;
            ParseXYZMinMax(elevText, ref minX, ref maxX, ref minZ, ref maxZ);
            ParseXYZMinMax(fuelText, ref minX, ref maxX, ref minZ, ref maxZ);

            if (minX == int.MaxValue || minZ == int.MaxValue)
            {
                elevationCount = 0;
                fuelCount = 0;
                EditorUtility.DisplayDialog("XYZ Import", "No valid points found in XYZ files.", "OK");
                return;
            }

            startLongitudeMeter = minX;
            startLatitudeMeter = negativeLatitudeStep ? maxZ : minZ;

            int width = Mathf.Max(1, Mathf.CeilToInt((maxX - minX) / (float)meterStep) + 1);
            int height = Mathf.Max(1, Mathf.CeilToInt((maxZ - minZ) / (float)meterStep) + 1);
            InitializeMap(width, height);
        }
        else
        {
            // Parse to infer width/height from max indices (uses start/meterStep)
            int maxX = 0;
            int maxZ = 0;
            ParseXYZBounds(elevText, ref maxX, ref maxZ);
            ParseXYZBounds(fuelText, ref maxX, ref maxZ);
            int width = maxX + 1;
            int height = maxZ + 1;
            InitializeMap(width, height);
        }

        elevationCount = ParseXYZIntoLayer(elevText, true);
        fuelCount = ParseXYZIntoLayer(fuelText, false);

        EditorUtility.SetDirty(elevationLayer);
        EditorUtility.SetDirty(fuelCodeLayer);
        EditorUtility.SetDirty(mapData);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void ParseXYZMinMax(string text, ref int minX, ref int maxX, ref int minZ, ref int maxZ)
    {
        using (var reader = new System.IO.StringReader(text))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (!TryParseXYZLine(line, out int xMeter, out int zMeter, out _)) continue;

                if (xMeter < minX) minX = xMeter;
                if (xMeter > maxX) maxX = xMeter;
                if (zMeter < minZ) minZ = zMeter;
                if (zMeter > maxZ) maxZ = zMeter;
            }
        }
    }

    private void ParseXYZBounds(string text, ref int maxX, ref int maxZ)
    {
        using (var reader = new System.IO.StringReader(text))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (!TryParseXYZLine(line, out int xMeter, out int zMeter, out _)) continue;
                int x = (xMeter - startLongitudeMeter) / meterStep;
                int z = negativeLatitudeStep
                    ? (startLatitudeMeter - zMeter) / meterStep
                    : (zMeter - startLatitudeMeter) / meterStep;
                if (x > maxX) maxX = x;
                if (z > maxZ) maxZ = z;
            }
        }
    }

    private int ParseXYZIntoLayer(string text, bool isElevation)
    {
        int count = 0;
        using (var reader = new System.IO.StringReader(text))
        {
            string line;
            int lineIndex = 0;
            int totalChars = text.Length;
            int processedChars = 0;
            while ((line = reader.ReadLine()) != null)
            {
                processedChars += line.Length + 1;
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (!TryParseXYZLine(line, out int xMeter, out int zMeter, out float value)) continue;
                short shortValue = (short)Mathf.RoundToInt(value);

                int x = (xMeter - startLongitudeMeter) / meterStep;
                int z = negativeLatitudeStep
                    ? (startLatitudeMeter - zMeter) / meterStep
                    : (zMeter - startLatitudeMeter) / meterStep;
                if (x < 0 || z < 0 || x >= mapData.xWidth || z >= mapData.zWidth) continue;

                if (isElevation)
                {
                    elevationLayer.SetElevation(x, z, shortValue < 0 ? (short)0 : shortValue);
                }
                else
                {
                    fuelCodeLayer.SetFuelCode(x, z, shortValue);
                }
                count++;

                if (lineIndex % 50000 == 0)
                {
                    float progress = totalChars > 0 ? Mathf.Clamp01(processedChars / (float)totalChars) : 0f;
                    if (EditorUtility.DisplayCancelableProgressBar(
                        "Importing XYZ",
                        $"{(isElevation ? "Elevation" : "Fuel")} points: {count}",
                        progress))
                    {
                        EditorUtility.ClearProgressBar();
                        return count;
                    }
                }
                lineIndex++;
            }
        }
        EditorUtility.ClearProgressBar();
        return count;
    }

    private bool TryParseXYZLine(string line, out int xMeter, out int zMeter, out float value)
    {
        xMeter = 0;
        zMeter = 0;
        value = 0f;

        if (string.IsNullOrWhiteSpace(line)) return false;
        int idx = 0;

        if (!TryReadToken(line, ref idx, out string token1)) return false;
        if (!TryReadToken(line, ref idx, out string token2)) return false;
        if (!TryReadToken(line, ref idx, out string token3)) return false;

        if (!int.TryParse(token1, NumberStyles.Integer, CultureInfo.InvariantCulture, out xMeter)) return false;
        if (!int.TryParse(token2, NumberStyles.Integer, CultureInfo.InvariantCulture, out zMeter)) return false;
        if (!float.TryParse(token3, NumberStyles.Float, CultureInfo.InvariantCulture, out value)) return false;
        return true;
    }

    private bool TryReadToken(string line, ref int idx, out string token)
    {
        token = null;
        int len = line.Length;
        while (idx < len)
        {
            char c = line[idx];
            if (c != ' ' && c != '\t' && c != ',') break;
            idx++;
        }
        if (idx >= len) return false;

        int start = idx;
        while (idx < len)
        {
            char c = line[idx];
            if (c == ' ' || c == '\t' || c == ',') break;
            idx++;
        }
        token = line.Substring(start, idx - start);
        return token.Length > 0;
    }

    private static Texture2D EnsureReadable(Texture2D tex)
    {
        if (tex == null) return null;
        string path = AssetDatabase.GetAssetPath(tex);
        if (string.IsNullOrEmpty(path)) return tex;
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return tex;

        bool changed = false;
        if (!importer.isReadable)
        {
            importer.isReadable = true;
            changed = true;
        }
        if (importer.textureCompression != TextureImporterCompression.Uncompressed)
        {
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            changed = true;
        }
        if (importer.mipmapEnabled)
        {
            importer.mipmapEnabled = false;
            changed = true;
        }
        if (importer.sRGBTexture)
        {
            importer.sRGBTexture = false;
            changed = true;
        }

        if (changed)
        {
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    private struct FuelPaletteEntry
    {
        public short code;
        public Color color;
    }

    private System.Collections.Generic.List<FuelPaletteEntry> BuildFuelPalette(FuelCodeSet set)
    {
        var list = new System.Collections.Generic.List<FuelPaletteEntry>();
        if (set == null || set.fuelCodes == null) return list;

        foreach (var fuel in set.fuelCodes)
        {
            if (fuel == null) continue;
            list.Add(new FuelPaletteEntry
            {
                code = fuel.fuelCodeID,
                color = fuel.GetFamilyAdjustedColor()
            });
        }

        return list;
    }

    private short MatchFuelCode(Color32 color, System.Collections.Generic.List<FuelPaletteEntry> palette, System.Collections.Generic.Dictionary<int, short> cache)
    {
        if (color.a <= alphaIgnoreThreshold || palette.Count == 0)
        {
            return defaultFuelCode;
        }

        int key = (color.r << 24) | (color.g << 16) | (color.b << 8) | color.a;
        if (cache.TryGetValue(key, out short cached))
        {
            return cached;
        }

        Color c = new Color(color.r / 255f, color.g / 255f, color.b / 255f, 1f);
        float best = float.MaxValue;
        short bestCode = defaultFuelCode;

        foreach (var entry in palette)
        {
            float d = ColorDistance(c, entry.color);
            if (d < best)
            {
                best = d;
                bestCode = entry.code;
            }
        }

        cache[key] = bestCode;
        return bestCode;
    }

    private float ColorDistance(Color a, Color b)
    {
        if (colorDistanceMode == ColorDistanceMode.Hsv)
        {
            Color.RGBToHSV(a, out float ah, out float asat, out float aval);
            Color.RGBToHSV(b, out float bh, out float bsat, out float bval);
            float dh = Mathf.Abs(ah - bh);
            dh = Mathf.Min(dh, 1f - dh);
            float ds = asat - bsat;
            float dv = aval - bval;
            return (dh * dh) + (ds * ds) + (dv * dv);
        }

        float dr = a.r - b.r;
        float dg = a.g - b.g;
        float db = a.b - b.b;
        return (dr * dr) + (dg * dg) + (db * db);
    }

    private void RunRasterSmokeTest()
    {
        var prevElev = elevationTexture;
        var prevFuel = fuelTexture;
        var prevMin = elevationMin;
        var prevMax = elevationMax;

        try
        {
            elevationMin = 0f;
            elevationMax = 100f;

            elevationTexture = new Texture2D(4, 4, TextureFormat.RGBA32, false, true);
            var elevColors = new Color32[16];
            for (int i = 0; i < elevColors.Length; i++)
            {
                byte g = (byte)Mathf.RoundToInt(Mathf.Lerp(0f, 255f, i / 15f));
                elevColors[i] = new Color32(g, g, g, 255);
            }
            elevationTexture.SetPixels32(elevColors);
            elevationTexture.Apply();

            fuelTexture = new Texture2D(4, 4, TextureFormat.RGBA32, false, true);
            var palette = BuildFuelPalette(fuelCodeSet);
            var colors = new Color32[16];
            for (int i = 0; i < colors.Length; i++)
            {
                if (palette.Count > 0)
                {
                    var color = palette[i % palette.Count].color;
                    colors[i] = color;
                }
                else
                {
                    colors[i] = (i % 2 == 0) ? new Color32(255, 0, 0, 255) : new Color32(0, 255, 0, 255);
                }
            }
            fuelTexture.SetPixels32(colors);
            fuelTexture.Apply();

            ImportRaster();
        }
        finally
        {
            elevationTexture = prevElev;
            fuelTexture = prevFuel;
            elevationMin = prevMin;
            elevationMax = prevMax;
        }
    }

    private void RebuildTerrain()
    {
        if (terrainGenerator == null)
        {
            terrainGenerator = FindObjectOfType<TerrainGenerator>();
        }

        if (terrainGenerator == null)
        {
            Debug.LogWarning("GISImportWindow: No TerrainGenerator found to rebuild.");
            return;
        }

        terrainGenerator.mapData = mapData;
        terrainGenerator.elevationLayer = elevationLayer;
        terrainGenerator.fuelCodeLayer = fuelCodeLayer;
        terrainGenerator.BuildAllTilesImmediate();
    }

    private void EnsureXYZTargets()
    {
        if (mapData == null)
        {
            mapData = FindObjectOfType<MapData>();
        }
        if (elevationLayer == null && mapData != null)
        {
            elevationLayer = mapData.elevationLayer;
        }
        if (fuelCodeLayer == null && mapData != null)
        {
            fuelCodeLayer = mapData.fuelCodeLayer;
        }
    }

    private Texture2D ImportGeoTiffTexture(string title, string prefix)
    {
        string sourcePath = EditorUtility.OpenFilePanel(title, "", "tif,tiff");
        if (string.IsNullOrEmpty(sourcePath)) return null;

        string targetDir = "Assets/ImportSamples";
        Directory.CreateDirectory(targetDir);
        string fileName = Path.GetFileNameWithoutExtension(sourcePath) + ".txt";
        string targetPath = Path.Combine(targetDir, fileName);
        targetPath = AssetDatabase.GenerateUniqueAssetPath(targetPath);

        FileUtil.CopyFileOrDirectory(sourcePath, targetPath);
        AssetDatabase.ImportAsset(targetPath, ImportAssetOptions.ForceUpdate);

        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(targetPath);
        if (tex == null)
        {
            Debug.LogWarning($"{prefix} GeoTIFF import failed: {targetPath}");
            return null;
        }

        return EnsureReadable(tex);
    }

    private void SelectXYZFile(string title, ref string pathField)
    {
        string sourcePath = EditorUtility.OpenFilePanel(title, "", "xyz,txt,csv");
        if (string.IsNullOrEmpty(sourcePath)) return;
        pathField = sourcePath;
    }

    private string ReadXYZText(TextAsset asset, string fallbackPath)
    {
        if (asset != null) return asset.text;
        if (string.IsNullOrEmpty(fallbackPath)) return null;
        try
        {
            return File.ReadAllText(fallbackPath);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Failed to read XYZ file: {fallbackPath} ({ex.Message})");
            return null;
        }
    }
}
