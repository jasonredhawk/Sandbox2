using UnityEngine;

/// <summary>
/// Manages terrain tile loading and unloading
/// Coordinates between MapData and TerrainGenerator
/// </summary>
public class TerrainManager : MonoBehaviour
{
    [Header("References")]
    public MapData mapData;
    public TerrainGenerator terrainGenerator;
    
    [Header("Settings")]
    public int maxLoadedTiles = 25; // Maximum number of tiles to keep loaded
    public float unloadDistance = 500f; // Distance beyond which tiles are unloaded
    
    void Start()
    {
        if (mapData == null)
        {
            mapData = GetComponent<MapData>();
        }
        
        if (terrainGenerator == null)
        {
            terrainGenerator = GetComponent<TerrainGenerator>();
        }
    }
    
    /// <summary>
    /// Request terrain tile to be loaded
    /// </summary>
    public void RequestTile(int tileX, int tileZ)
    {
        // Implementation will be added when TerrainGenerator is created
    }
    
    /// <summary>
    /// Unload terrain tiles beyond a certain distance
    /// </summary>
    public void UnloadDistantTiles(Vector3 centerPosition)
    {
        // Implementation will be added when TerrainGenerator is created
    }
}
