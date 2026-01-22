using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a single pixel (30m x 30m real world = 1m x 1m in-game)
/// Based on legacy Pixel.cs with simplified runtime hooks.
/// </summary>
public class Pixel
{
    public int x;
    public int z;

    // State
    public PixelState state = PixelState.ALIVE;
    public float fireIntensity = 0f; // kW/m
    public float flameLength = 0f;   // meters
    public float ROS = 0f;           // rate of spread

    // References
    private readonly GameManager gameManager;

    // Cached values
    private short? _elevation;
    private short? _fuelCode;
    private float? _slope;

    public Pixel(GameManager gm, int x, int z)
    {
        gameManager = gm;
        this.x = x;
        this.z = z;
    }

    public short elevation
    {
        get
        {
            if (!_elevation.HasValue)
            {
                _elevation = gameManager?.mapData?.GetElevation(x, z) ?? 0;
            }
            return _elevation.Value;
        }
        set
        {
            _elevation = value;
            gameManager?.mapData?.SetElevation(x, z, value);
            gameManager?.terrainGenerator?.PixelElevationChanged(this);
            _slope = null;
        }
    }

    public short fuelCode
    {
        get
        {
            if (!_fuelCode.HasValue)
            {
                _fuelCode = gameManager?.mapData?.GetFuelCode(x, z) ?? (short)98;
            }
            return _fuelCode.Value;
        }
        set
        {
            _fuelCode = value;
            gameManager?.mapData?.SetFuelCode(x, z, value);
            gameManager?.terrainGenerator?.PixelFuelCodeChanged(this);
        }
    }

    public float slope
    {
        get
        {
            if (!_slope.HasValue)
            {
                _slope = ComputeSlope();
            }
            return _slope.Value;
        }
    }

    public Vector3 position => new Vector3(x, elevation / 30f, -z);
    public Vector2 location => new Vector2(x, z);

    private float ComputeSlope()
    {
        var md = gameManager?.mapData;
        if (md == null) return 0f;

        float top = md.GetElevation(x, z - 1);
        float left = md.GetElevation(x - 1, z);
        float right = md.GetElevation(x + 1, z);
        float bottom = md.GetElevation(x, z + 1);

        float slx = (right - left) / 30f;
        float sly = (bottom - top) / 30f;
        float sl0 = Mathf.Sqrt(slx * slx + sly * sly);

        return Mathf.Atan(sl0) * Mathf.Rad2Deg;
    }

    public void UpdateFireIntensityFromFlameLength()
    {
        fireIntensity = 259.833f * Mathf.Pow(flameLength, 2.174f) * 30f;
    }

    public void UpdateFlameLengthFromFireIntensity()
    {
        flameLength = Mathf.Pow((fireIntensity / 30f) / 259.833f, 1f / 2.174f);
    }
}

public enum PixelState
{
    ALIVE = 0,
    FIRE = 1,
    DEAD = 2
}
