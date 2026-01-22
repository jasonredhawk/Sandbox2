using UnityEngine;

/// <summary>
/// Represents a geographic location in meters
/// Used for converting between pixel coordinates and real-world coordinates
/// </summary>
[System.Serializable]
public struct GeoLocation
{
    public int longitudeMeter;
    public int latitudeMeter;
    
    public GeoLocation(int longitudeMeter, int latitudeMeter)
    {
        this.longitudeMeter = longitudeMeter;
        this.latitudeMeter = latitudeMeter;
    }
    
    public override string ToString()
    {
        return $"({longitudeMeter}, {latitudeMeter})";
    }
}
