using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Stores elevation data (DEM) in meters as short.
/// </summary>
[CreateAssetMenu(fileName = "ElevationLayer", menuName = "GIS/Elevation Layer")]
public class ElevationLayer : GISDataLayer
{
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
        return (short)0;
    }

    public override void SetData(int x, int z, object value)
    {
        EnsureCache();
        if (tileSize <= 0) tileSize = 122;
        var tileCoord = GetTileCoord(x, z);
        var local = GetLocalCoord(x, z);
        if (!tiles.ContainsKey(tileCoord))
        {
            tiles[tileCoord] = new short[tileSize, tileSize];
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
        return new short[tileSize, tileSize];
    }

    public override void SetTileData(int tileX, int tileZ, short[,] data)
    {
        EnsureCache();
        tiles[new Vector2Int(tileX, tileZ)] = data;
        SyncToSerialized();
    }

    public short GetElevation(int x, int z) => (short)GetData(x, z);

    public void SetElevation(int x, int z, short elevation)
    {
        EnsureCache();
        if (tileSize <= 0) tileSize = 122;
        var tileCoord = GetTileCoord(x, z);
        var local = GetLocalCoord(x, z);
        if (!tiles.ContainsKey(tileCoord))
        {
            tiles[tileCoord] = new short[tileSize, tileSize];
        }
        tiles[tileCoord][local.x, local.y] = elevation;
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
}
