using UnityEngine;

/// <summary>
/// Base class for GIS data layers (elevation, fuel codes, canopy, etc.)
/// Provides tile-aware accessors and coordinate helpers.
/// </summary>
public abstract class GISDataLayer : ScriptableObject
{
    [Header("Layer Metadata")]
    public string layerName;
    public int width;
    public int height;
    public int tileSize = 122; // matches legacy chunk size

    [Header("Geographic Bounds")]
    public int startLongitudeMeter;
    public int startLatitudeMeter;
    public int longitudeMeterStep = 30;
    public int latitudeMeterStep = 30;

    /// <summary>Returns the value at pixel coordinates.</summary>
    public abstract object GetData(int x, int z);

    /// <summary>Sets the value at pixel coordinates.</summary>
    public abstract void SetData(int x, int z, object value);

    /// <summary>Gets the underlying tile array.</summary>
    public abstract short[,] GetTileData(int tileX, int tileZ);

    /// <summary>Sets the underlying tile array.</summary>
    public abstract void SetTileData(int tileX, int tileZ, short[,] data);

    protected Vector2Int GetTileCoord(int x, int z)
    {
        return new Vector2Int(
            Mathf.FloorToInt(x / (float)tileSize),
            Mathf.FloorToInt(z / (float)tileSize)
        );
    }

    protected Vector2Int GetLocalCoord(int x, int z)
    {
        int lx = x % tileSize;
        int lz = z % tileSize;
        if (lx < 0) lx += tileSize;
        if (lz < 0) lz += tileSize;
        return new Vector2Int(lx, lz);
    }

    public Vector3 PixelToWorld(int x, int z, float elevation = 0f)
    {
        return new Vector3(x, elevation / 30f, -z);
    }

    public Vector2Int WorldToPixel(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x);
        int z = Mathf.FloorToInt(-worldPos.z);
        return new Vector2Int(x, z);
    }

    public GeoLocation GetGeoCoord(int x, int z)
    {
        int tileX = Mathf.FloorToInt(x / (float)tileSize);
        int tileZ = Mathf.FloorToInt(z / (float)tileSize);
        int localX = x - tileX * tileSize;
        int localZ = z - tileZ * tileSize;

        int longitudeMeter = startLongitudeMeter + tileX * tileSize * longitudeMeterStep + localX * longitudeMeterStep;
        int latitudeMeter = startLatitudeMeter + tileZ * tileSize * latitudeMeterStep + localZ * latitudeMeterStep;

        return new GeoLocation(longitudeMeter, latitudeMeter);
    }
}
