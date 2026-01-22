using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Stores fuel code data as short values matching GIS codes.
/// </summary>
[CreateAssetMenu(fileName = "FuelCodeLayer", menuName = "GIS/Fuel Code Layer")]
public class FuelCodeLayer : GISDataLayer
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
        return (short)98; // default: no fuel
    }

    public override void SetData(int x, int z, object value)
    {
        if (tileSize <= 0) tileSize = 122;
        var tileCoord = GetTileCoord(x, z);
        var local = GetLocalCoord(x, z);
        if (!tiles.ContainsKey(tileCoord))
        {
            var arr = new short[tileSize, tileSize];
            for (int i = 0; i < tileSize; i++)
            for (int j = 0; j < tileSize; j++)
                arr[i, j] = 98;
            tiles[tileCoord] = arr;
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
        var arr = new short[tileSize, tileSize];
        for (int i = 0; i < tileSize; i++)
        for (int j = 0; j < tileSize; j++)
            arr[i, j] = 98;
        return arr;
    }

    public override void SetTileData(int tileX, int tileZ, short[,] data)
    {
        tiles[new Vector2Int(tileX, tileZ)] = data;
    }

    public short GetFuelCode(int x, int z) => (short)GetData(x, z);
    public void SetFuelCode(int x, int z, short fuelCode) => SetData(x, z, fuelCode);

    /// <summary>
    /// Fill a rectangular region (0..width, 0..height) with the specified fuel code.
    /// </summary>
    public void Fill(int width, int height, short fuelCodeValue)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                SetFuelCode(x, z, fuelCodeValue);
            }
        }
    }

    public static bool IsRoad(short fuelCode) =>
        fuelCode == 7299 || fuelCode == 7298 || fuelCode == 7297 || fuelCode == 7296;
    public static bool IsUrban(short fuelCode) =>
        fuelCode == 7298 || fuelCode == 7297 || fuelCode == 7296;
    public static bool IsWater(short fuelCode) =>
        fuelCode == 91 || fuelCode == 7292;
}
