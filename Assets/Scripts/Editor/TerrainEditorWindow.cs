using UnityEditor;
using UnityEngine;

/// <summary>
/// Terrain paint tool: paint elevation and fuel codes directly on the tiles.
/// Simple, no undo stack (relies on Unity undo for layer assets if supported).
/// </summary>
public class TerrainEditorWindow : EditorWindow
{
    private enum PaintMode { Elevation, FuelCode }

    private GameManager gameManager;
    private MapData mapData;
    private ElevationLayer elevationLayer;
    private FuelCodeLayer fuelCodeLayer;
    private TerrainGenerator terrainGenerator;

    private PaintMode paintMode = PaintMode.Elevation;
    private float brushRadius = 4f;
    private float brushStrength = 1f;
    private AnimationCurve falloff = AnimationCurve.EaseInOut(0, 1, 1, 0);
    private short targetFuelCode = 98;
    private float targetElevation = 10f;

    [MenuItem("Tools/Terrain Editor")]
    public static void ShowWindow()
    {
        GetWindow<TerrainEditorWindow>("Terrain Editor");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Terrain Paint Tool", EditorStyles.boldLabel);

        gameManager = (GameManager)EditorGUILayout.ObjectField("GameManager", gameManager, typeof(GameManager), true);
        mapData = (MapData)EditorGUILayout.ObjectField("MapData", mapData, typeof(MapData), true);
        elevationLayer = (ElevationLayer)EditorGUILayout.ObjectField("ElevationLayer", elevationLayer, typeof(ElevationLayer), false);
        fuelCodeLayer = (FuelCodeLayer)EditorGUILayout.ObjectField("FuelCodeLayer", fuelCodeLayer, typeof(FuelCodeLayer), false);
        terrainGenerator = (TerrainGenerator)EditorGUILayout.ObjectField("TerrainGenerator", terrainGenerator, typeof(TerrainGenerator), true);

        paintMode = (PaintMode)EditorGUILayout.EnumPopup("Paint Mode", paintMode);
        brushRadius = EditorGUILayout.Slider("Brush Radius", brushRadius, 1f, 32f);
        brushStrength = EditorGUILayout.Slider("Brush Strength", brushStrength, 0.1f, 10f);
        falloff = EditorGUILayout.CurveField("Falloff", falloff);

        if (paintMode == PaintMode.Elevation)
        {
            targetElevation = EditorGUILayout.FloatField("Target Elevation (m)", targetElevation);
        }
        else
        {
            targetFuelCode = (short)EditorGUILayout.IntField("Target Fuel Code", targetFuelCode);
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Click in Scene View to paint. Requires Game view/Scene view raycast on Terrain layer (default layer).", MessageType.Info);
    }

    private void OnFocus()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (mapData == null || elevationLayer == null || fuelCodeLayer == null) return;

        Event e = Event.current;
        if (e == null) return;

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Draw brush preview
            Handles.color = new Color(1f, 1f, 0f, 0.3f);
            Handles.DrawSolidDisc(hit.point, Vector3.up, brushRadius);

            if (e.type == EventType.MouseDown && e.button == 0 && !e.alt && !e.control && !e.command)
            {
                PaintAt(hit.point);
                e.Use();
            }
            else if (e.type == EventType.MouseDrag && e.button == 0 && !e.alt && !e.control && !e.command)
            {
                PaintAt(hit.point);
                e.Use();
            }
        }
    }

    private void PaintAt(Vector3 worldPos)
    {
        // Convert to pixel coords
        int centerX = Mathf.FloorToInt(worldPos.x);
        int centerZ = Mathf.FloorToInt(-worldPos.z); // negated to match Pixel convention

        int radius = Mathf.CeilToInt(brushRadius);
        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dz = -radius; dz <= radius; dz++)
            {
                int px = centerX + dx;
                int pz = centerZ + dz;

                float dist = Vector2.Distance(new Vector2(px, pz), new Vector2(centerX, centerZ));
                if (dist > brushRadius) continue;

                float t = dist / Mathf.Max(brushRadius, 0.0001f);
                float fall = falloff.Evaluate(t);
                float strength = brushStrength * fall;

                if (paintMode == PaintMode.Elevation)
                {
                    short cur = mapData.GetElevation(px, pz);
                    short newElev = (short)Mathf.Round(Mathf.Lerp(cur, targetElevation, strength * 0.1f));
                    mapData.SetElevation(px, pz, newElev);

                    // Rebuild affected tile
                    terrainGenerator?.PixelElevationChanged(new Pixel(gameManager, px, pz));
                }
                else
                {
                    mapData.SetFuelCode(px, pz, targetFuelCode);
                    // Placeholder: could refresh textures here
                }
            }
        }

        // Force scene view repaint
        SceneView.RepaintAll();
    }
}
