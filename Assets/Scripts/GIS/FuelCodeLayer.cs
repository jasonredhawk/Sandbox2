using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Stores fuel code data as short values matching GIS codes.
/// </summary>
[CreateAssetMenu(fileName = "FuelCodeLayer", menuName = "GIS/Fuel Code Layer")]
public class FuelCodeLayer : GISDataLayer
{
    private const short DEFAULT_FUEL_CODE = 98;

    // Serializable tile entry for Unity persistence
    [Serializable]
    private class TileEntry
    {
        public int tileX;
        public int tileZ;
        public int size;
        public short[] data; // flattened 2D array

        public TileEntry() { }
        public TileEntry(int x, int z, int size, short[,] arr)
        {
            tileX = x;
            tileZ = z;
            this.size = size;
            data = Flatten(arr, size);
        }

        public short[,] ToArray()
        {
            var arr = new short[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    arr[i, j] = data[i * size + j];
            return arr;
        }

        private static short[] Flatten(short[,] arr, int size)
        {
            var flat = new short[size * size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    flat[i * size + j] = arr[i, j];
            return flat;
        }
    }

    [SerializeField]
    private List<TileEntry> serializedTiles = new List<TileEntry>();

    // Runtime cache - rebuilt from serializedTiles on access
    private Dictionary<Vector2Int, short[,]> tiles;
    private bool cacheInitialized;

    private void EnsureCache()
    {
        if (cacheInitialized && tiles != null) return;
        tiles = new Dictionary<Vector2Int, short[,]>();
        if (serializedTiles != null)
        {
            foreach (var entry in serializedTiles)
            {
                if (entry != null && entry.data != null)
                {
                    tiles[new Vector2Int(entry.tileX, entry.tileZ)] = entry.ToArray();
                }
            }
        }
        cacheInitialized = true;
    }

    private void SyncToSerialized()
    {
        serializedTiles.Clear();
        int effectiveSize = tileSize > 0 ? tileSize : 122;
        foreach (var kvp in tiles)
        {
            serializedTiles.Add(new TileEntry(kvp.Key.x, kvp.Key.y, effectiveSize, kvp.Value));
        }
    }

    private short[,] CreateDefaultTile()
    {
        int effectiveSize = tileSize > 0 ? tileSize : 122;
        var arr = new short[effectiveSize, effectiveSize];
        for (int i = 0; i < effectiveSize; i++)
            for (int j = 0; j < effectiveSize; j++)
                arr[i, j] = DEFAULT_FUEL_CODE;
        return arr;
    }

    public override object GetData(int x, int z)
    {
        EnsureCache();
        if (tileSize <= 0) tileSize = 122;
        var tileCoord = GetTileCoord(x, z);
        var local = GetLocalCoord(x, z);
        if (tiles.TryGetValue(tileCoord, out var tile))
        {
            return tile[local.x, local.y];
        }
        return DEFAULT_FUEL_CODE;
    }

    public override void SetData(int x, int z, object value)
    {
        EnsureCache();
        if (tileSize <= 0) tileSize = 122;
        var tileCoord = GetTileCoord(x, z);
        var local = GetLocalCoord(x, z);
        if (!tiles.ContainsKey(tileCoord))
        {
            tiles[tileCoord] = CreateDefaultTile();
        }
        tiles[tileCoord][local.x, local.y] = (short)value;
        SyncToSerialized();
    }

    public override short[,] GetTileData(int tileX, int tileZ)
    {
        EnsureCache();
        if (tileSize <= 0) tileSize = 122;
        var key = new Vector2Int(tileX, tileZ);
        if (tiles.TryGetValue(key, out var tile))
        {
            return tile;
        }
        return CreateDefaultTile();
    }

    public override void SetTileData(int tileX, int tileZ, short[,] data)
    {
        EnsureCache();
        tiles[new Vector2Int(tileX, tileZ)] = data;
        SyncToSerialized();
    }

    public short GetFuelCode(int x, int z) => (short)GetData(x, z);

    public void SetFuelCode(int x, int z, short fuelCode)
    {
        EnsureCache();
        if (tileSize <= 0) tileSize = 122;
        var tileCoord = GetTileCoord(x, z);
        var local = GetLocalCoord(x, z);
        if (!tiles.ContainsKey(tileCoord))
        {
            tiles[tileCoord] = CreateDefaultTile();
        }
        tiles[tileCoord][local.x, local.y] = fuelCode;
        // Don't sync on every pixel - caller should call MarkDirty() when done
    }

    /// <summary>
    /// Call after batch updates to persist changes to the serialized data.
    /// </summary>
    public void MarkDirty()
    {
        EnsureCache();
        SyncToSerialized();
    }

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
        MarkDirty();
    }

    /// <summary>
    /// Clear runtime cache to force reload from serialized data.
    /// </summary>
    public void ClearCache()
    {
        tiles = null;
        cacheInitialized = false;
    }

    private void OnEnable()
    {
        cacheInitialized = false;
    }

    public static bool IsRoad(short fuelCode) =>
        fuelCode == 7299 || fuelCode == 7298 || fuelCode == 7297 || fuelCode == 7296;
    public static bool IsUrban(short fuelCode) =>
        fuelCode == 7298 || fuelCode == 7297 || fuelCode == 7296;
    public static bool IsWater(short fuelCode) =>
        fuelCode == 91 || fuelCode == 7292;
}
