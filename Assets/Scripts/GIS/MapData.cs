using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages map data including elevation and fuel codes
/// Based on old project's MapData.cs but improved
/// </summary>
public class MapData : MonoBehaviour
{
    [Header("Map Dimensions")]
    public int xWidth;
    public int zWidth;
    public int xCount; // Number of tiles in X direction
    public int zCount; // Number of tiles in Z direction
    [Header("Tiles")]
    public int tileSize = 122; // single source of truth for chunk size
    
    [Header("Geographic Settings")]
    public int startLongitudeMeter;
    public int startLatitudeMeter;
    public int longitudeMeterStep = 30; // 30m per pixel
    public int latitudeMeterStep = 30;
    
    [Header("Layers")]
    public ElevationLayer elevationLayer;
    public FuelCodeLayer fuelCodeLayer;
    
    private GameManager gameManager;
    
    void Awake()
    {
        gameManager = GetComponent<GameManager>();
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        // Ensure counts are initialized if width/height were set in inspector
        if ((xCount == 0 || zCount == 0) && xWidth > 0 && zWidth > 0)
        {
            xCount = Mathf.CeilToInt((float)xWidth / tileSize);
            zCount = Mathf.CeilToInt((float)zWidth / tileSize);
        }
    }
    
    /// <summary>
    /// Initialize map data with dimensions
    /// </summary>
    public void Initialize(int xWidth, int zWidth, int startLongitudeMeter, int startLatitudeMeter, 
                          int longitudeMeterStep, int latitudeMeterStep)
    {
        this.xWidth = xWidth;
        this.zWidth = zWidth;
        this.startLongitudeMeter = startLongitudeMeter;
        this.startLatitudeMeter = startLatitudeMeter;
        this.longitudeMeterStep = longitudeMeterStep;
        this.latitudeMeterStep = latitudeMeterStep;
        
        xCount = Mathf.CeilToInt((float)xWidth / tileSize);
        zCount = Mathf.CeilToInt((float)zWidth / tileSize);
    }
    
    /// <summary>
    /// Get elevation at pixel coordinates
    /// </summary>
    public short GetElevation(int x, int z)
    {
        if (elevationLayer == null) return 0;
        return elevationLayer.GetElevation(x, z);
    }
    
    /// <summary>
    /// Set elevation at pixel coordinates
    /// </summary>
    public void SetElevation(int x, int z, short elevation)
    {
        if (elevationLayer == null) return;
        elevationLayer.SetElevation(x, z, elevation);
    }
    
    /// <summary>
    /// Get fuel code at pixel coordinates
    /// </summary>
    public short GetFuelCode(int x, int z)
    {
        if (fuelCodeLayer == null) return 98;
        return fuelCodeLayer.GetFuelCode(x, z);
    }
    
    /// <summary>
    /// Set fuel code at pixel coordinates
    /// </summary>
    public void SetFuelCode(int x, int z, short fuelCode)
    {
        if (fuelCodeLayer == null) return;
        fuelCodeLayer.SetFuelCode(x, z, fuelCode);
    }
    
    /// <summary>
    /// Get pixel at coordinates
    /// </summary>
    public Pixel GetPixel(int x, int z)
    {
        // Lightweight on-demand pixel (not cached yet)
        return new Pixel(gameManager, x, z);
    }
    
    /// <summary>
    /// Get geographic coordinates for pixel
    /// </summary>
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
    
    /// <summary>
    /// Reset map to default state
    /// </summary>
    public void ResetMap()
    {
        // TODO: Implement map reset
    }

    /// <summary>
    /// Fill the entire map with default elevation and fuel code.
    /// </summary>
    public void Fill(short elevationValue, short fuelCodeValue)
    {
        if (xWidth <= 0 || zWidth <= 0)
        {
            Debug.LogWarning("MapData.Fill: width/height are zero. Set xWidth/zWidth first.");
            return;
        }
        if (elevationLayer == null || fuelCodeLayer == null) return;

        for (int x = 0; x < xWidth; x++)
        {
            for (int z = 0; z < zWidth; z++)
            {
                elevationLayer.SetElevation(x, z, elevationValue);
                fuelCodeLayer.SetFuelCode(x, z, fuelCodeValue);
            }
        }
    }
}

/// <summary>
/// Represents a map tile (chunk)
/// </summary>
public class MapTile
{
    public int tileX;
    public int tileZ;
    public short[,] elevation;
    public short[,] fuelCode;
    
    public MapTile(int tileX, int tileZ)
    {
        this.tileX = tileX;
        this.tileZ = tileZ;
        elevation = new short[122, 122];
        fuelCode = new short[122, 122];
    }
}
