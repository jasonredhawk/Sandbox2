using UnityEngine;
using System;
using System.IO;
using System.Threading.Tasks;

/// <summary>
/// Parses GIS XYZ files (space separated: x y value) into elevation and fuel layers.
/// Designed to match the legacy data flow but simplified for local parsing.
/// </summary>
public class GISDataParser : MonoBehaviour
{
    [Header("Inputs")]
    [Tooltip("XYZ file for elevation (DEM)")]
    public TextAsset elevationXYZ;
    [Tooltip("XYZ file for fuel codes")]
    public TextAsset fuelXYZ;

    [Header("Targets")]
    public MapData mapData;
    public ElevationLayer elevationLayer;
    public FuelCodeLayer fuelCodeLayer;

    [Header("Map Parameters")]
    [Tooltip("Number of columns in the source grid")]
    public int numColumns;
    [Tooltip("Number of rows in the source grid")]
    public int numRows;
    [Tooltip("Top edge in meters (native units)")]
    public int latitudeTopMeters;
    [Tooltip("Left edge in meters (native units)")]
    public int longitudeLeftMeters;
    [Tooltip("Cell size in meters (default 30)")]
    public int meterStep = 30;

    /// <summary>
    /// Entry point to parse both elevation and fuel code XYZ files.
    /// </summary>
    [ContextMenu("Parse XYZ")]
    public async void ParseXYZ()
    {
        if (mapData == null || elevationLayer == null || fuelCodeLayer == null)
        {
            Debug.LogError("GISDataParser: Missing mapData or layers.");
            return;
        }

        if (elevationXYZ == null || fuelXYZ == null)
        {
            Debug.LogError("GISDataParser: Missing XYZ TextAssets.");
            return;
        }

        await ParseAsync();
        Debug.Log("GISDataParser: Parse complete.");
    }

    private async Task ParseAsync()
    {
        // Initialize map dimensions from parameters
        int xWidth = numColumns;
        int zWidth = numRows;
        mapData.Initialize(xWidth, zWidth, longitudeLeftMeters, latitudeTopMeters, meterStep, -meterStep);

        elevationLayer.width = xWidth;
        elevationLayer.height = zWidth;
        elevationLayer.startLongitudeMeter = longitudeLeftMeters;
        elevationLayer.startLatitudeMeter = latitudeTopMeters;
        elevationLayer.longitudeMeterStep = meterStep;
        elevationLayer.latitudeMeterStep = -meterStep;

        fuelCodeLayer.width = xWidth;
        fuelCodeLayer.height = zWidth;
        fuelCodeLayer.startLongitudeMeter = longitudeLeftMeters;
        fuelCodeLayer.startLatitudeMeter = latitudeTopMeters;
        fuelCodeLayer.longitudeMeterStep = meterStep;
        fuelCodeLayer.latitudeMeterStep = -meterStep;

        await Task.Yield();

        ParseXYZIntoLayer(elevationXYZ.text, true);
        ParseXYZIntoLayer(fuelXYZ.text, false);
    }

    /// <summary>
    /// Parses a single XYZ text into either elevation or fuel layer.
    /// </summary>
    private void ParseXYZIntoLayer(string text, bool isElevation)
    {
        using (StringReader reader = new StringReader(text))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 3) continue;

                int xMeter = int.Parse(parts[0]);
                int zMeter = int.Parse(parts[1]);
                short value = Convert.ToInt16(float.Parse(parts[2]));

                // Convert meters to pixel indices relative to top-left
                int x = (xMeter - longitudeLeftMeters) / meterStep;
                int z = (latitudeTopMeters - zMeter) / meterStep; // top edge downward

                if (x < 0 || z < 0 || x >= numColumns || z >= numRows) continue;

                if (isElevation)
                {
                    elevationLayer.SetElevation(x, z, value < 0 ? (short)0 : value);
                }
                else
                {
                    fuelCodeLayer.SetFuelCode(x, z, value);
                }
            }
        }
    }
}
