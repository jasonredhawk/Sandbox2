using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Stores elevation data (DEM) in meters as short.
/// </summary>
[CreateAssetMenu(fileName = "ElevationLayer", menuName = "GIS/Elevation Layer")]
public class ElevationLayer : GISDataLayer
{
    private Dictionary<Vector2Int, short[,]> tiles = new Dictionary<Vector2Int, short[,]>();

    public override object GetData(int x, int z)
    {
        // tileSize will be driven by MapData; fallback to 122 if unset
        if (tileSize <= 0) tileSize = 122;
        var tileCoord = GetTileCoord(x, z);
        var local = GetLocalCoord(x, z);
        if (tiles.TryGetValue(tileCoord, out var tile))
        {
            return tile[local.x, local.y];
        }
        return (short)0;
    }

    public override void SetData(int x, int z, object value)
    {
        if (tileSize <= 0) tileSize = 122;
        var tileCoord = GetTileCoord(x, z);
        var local = GetLocalCoord(x, z);
        if (!tiles.ContainsKey(tileCoord))
        {
            tiles[tileCoord] = new short[tileSize, tileSize];
        }
        tiles[tileCoord][local.x, local.y] = (short)value;
    }

    public override short[,] GetTileData(int tileX, int tileZ)
    {
        if (tileSize <= 0) tileSize = 122;
        var key = new Vector2Int(tileX, tileZ);
        if (tiles.TryGetValue(key, out var tile))
        {
            return tile;
        }
        return new short[tileSize, tileSize];
    }

    public override void SetTileData(int tileX, int tileZ, short[,] data)
    {
        tiles[new Vector2Int(tileX, tileZ)] = data;
    }

    public short GetElevation(int x, int z) => (short)GetData(x, z);
    public void SetElevation(int x, int z, short elevation) => SetData(x, z, elevation);

    /// <summary>
    /// Fill a rectangular region (0..width, 0..height) with the specified elevation.
    /// </summary>
    public void Fill(int width, int height, short elevationValue)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                SetElevation(x, z, elevationValue);
            }
        }
    }
}
