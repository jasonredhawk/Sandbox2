using UnityEngine;

using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// Generates and manages terrain tiles (simplified: build all tiles once, no LOD).
/// </summary>
[ExecuteAlways]
public class TerrainGenerator : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;
    public MapData mapData;
    public ElevationLayer elevationLayer;
    public FuelCodeLayer fuelCodeLayer;
    public FuelCodeSet fuelCodeSet;
    public Material terrainMaterial;

    private readonly Dictionary<Vector2Int, TerrainTile> tiles = new Dictionary<Vector2Int, TerrainTile>();
#if UNITY_EDITOR
    private bool rebuildQueued;
#endif

    void Start()
    {
#if UNITY_EDITOR
        if (IsInPrefabMode()) return;
#endif
        if (gameManager == null)
        {
            gameManager = GetComponent<GameManager>() ?? FindObjectOfType<GameManager>();
        }
        if (mapData == null)
        {
            mapData = gameManager != null ? gameManager.mapData : FindObjectOfType<MapData>();
        }
        if (elevationLayer == null && mapData != null)
        {
            elevationLayer = mapData.elevationLayer;
        }
        if (fuelCodeLayer == null && mapData != null)
        {
            fuelCodeLayer = mapData.fuelCodeLayer;
        }
        if (fuelCodeSet == null && gameManager != null && gameManager.fuelCodeManager != null)
        {
            fuelCodeSet = gameManager.fuelCodeManager.fuelCodeSet;
        }
        EnsureMaterial();
        BuildAllTilesImmediate();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (Application.isPlaying) return;
        if (IsInPrefabMode()) return;
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
        if (fuelCodeSet == null && gameManager != null && gameManager.fuelCodeManager != null)
        {
            fuelCodeSet = gameManager.fuelCodeManager.fuelCodeSet;
        }
        EnsureMaterial();
        if (mapData != null && mapData.xWidth > 0 && mapData.zWidth > 0)
        {
            QueueRebuild();
        }
    }

    private bool IsInPrefabMode()
    {
        // Skip building when editing the prefab asset to avoid parenting errors
        if (UnityEditor.EditorUtility.IsPersistent(gameObject)) return true;
#if UNITY_2021_1_OR_NEWER
        var stage = PrefabStageUtility.GetCurrentPrefabStage();
        if (stage != null && stage.IsPartOfPrefabContents(gameObject)) return true;
#endif
        return false;
    }

    private void QueueRebuild()
    {
        if (rebuildQueued) return;
        rebuildQueued = true;
        EditorApplication.delayCall += () =>
        {
            rebuildQueued = false;
            if (this == null) return;
            if (Application.isPlaying) return;
            if (IsInPrefabMode()) return;
            BuildAllTilesImmediate();
        };
    }
#endif

    /// <summary>
    /// Build all tiles; exposed for manual/early invocation.
    /// </summary>
    public void BuildAllTilesImmediate()
    {
        // Clear existing children (tiles/placeholders) before rebuild
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }

        // Fallbacks
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
        if (fuelCodeSet == null && gameManager != null && gameManager.fuelCodeManager != null)
        {
            fuelCodeSet = gameManager.fuelCodeManager.fuelCodeSet;
        }
        EnsureMaterial();

        if (mapData == null)
        {
            Debug.LogWarning("TerrainGenerator: missing mapData.");
            return;
        }

        // Always compute tile counts from current dimensions
        int effectiveTileSize = (elevationLayer != null && elevationLayer.tileSize > 0) ? elevationLayer.tileSize : 122;
        // Sync layer tileSize to MapData if available
        if (mapData.tileSize > 0)
        {
            effectiveTileSize = mapData.tileSize;
            if (elevationLayer != null) elevationLayer.tileSize = effectiveTileSize;
            if (mapData.fuelCodeLayer != null) mapData.fuelCodeLayer.tileSize = effectiveTileSize;
        }
        if (mapData.xWidth <= 0 || mapData.zWidth <= 0)
        {
            Debug.LogWarning("TerrainGenerator: xWidth or zWidth is zero; cannot build tiles.");
            return;
        }
        mapData.xCount = Mathf.Max(1, Mathf.CeilToInt(mapData.xWidth / (float)effectiveTileSize));
        mapData.zCount = Mathf.Max(1, Mathf.CeilToInt(mapData.zWidth / (float)effectiveTileSize));

        if (mapData.xCount <= 0 || mapData.zCount <= 0)
        {
            Debug.LogWarning("TerrainGenerator: map dimensions/counts are zero; cannot build tiles.");
            return;
        }

        int tileSize = effectiveTileSize;
        tiles.Clear();

        for (int tx = 0; tx < mapData.xCount; tx++)
        {
            for (int tz = 0; tz < mapData.zCount; tz++)
            {
                var key = new Vector2Int(tx, tz);
                if (!tiles.ContainsKey(key))
                {
                    var tile = new TerrainTile(tx, tz, transform, terrainMaterial, elevationLayer, mapData.fuelCodeLayer, mapData, gameManager, fuelCodeSet);
                    tiles[key] = tile;
                }
            }
        }

        // If nothing was built, create a simple placeholder plane to visualize bounds
        if (tiles.Count == 0 && mapData.xWidth > 0 && mapData.zWidth > 0)
        {
            var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.name = "TerrainPlaceholder";
            plane.transform.parent = transform;
            plane.transform.position = new Vector3(mapData.xWidth / 2f, 0f, -mapData.zWidth / 2f);
            plane.transform.localScale = new Vector3(mapData.xWidth / 10f, 1f, mapData.zWidth / 10f); // plane is 10x10 units
            var mr = plane.GetComponent<MeshRenderer>();
            mr.sharedMaterial = terrainMaterial != null ? terrainMaterial : new Material(Shader.Find("Standard"));
        }
    }

    private void EnsureMaterial()
    {
        if (terrainMaterial != null) return;
        var lit = Shader.Find("Universal Render Pipeline/Lit");
        var def = Shader.Find("Standard");
        terrainMaterial = new Material(lit != null ? lit : def);
    }

    /// <summary>
    /// Update pixel when elevation changes (rebuild affected tile mesh).
    /// </summary>
    public void PixelElevationChanged(Pixel pixel)
    {
        var key = new Vector2Int(Mathf.FloorToInt(pixel.x / (float)elevationLayer.tileSize),
                                 Mathf.FloorToInt(pixel.z / (float)elevationLayer.tileSize));
        if (tiles.TryGetValue(key, out var tile))
        {
            tile.BuildMesh();
        }
    }

    public void PixelFuelCodeChanged(Pixel pixel) { /* placeholder for texture updates */ }
    public void CatchPixelOnFire(Pixel pixel) { /* placeholder for fire viz */ }
    public void UpdatePixelWaterOrRetardant(Pixel pixel) { /* placeholder for water viz */ }
    public void PixelDied(int x, int z, Pixel pixel) { /* placeholder for burn viz */ }
}
