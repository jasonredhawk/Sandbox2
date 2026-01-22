---
name: Wildland Firefighting Simulator Rebuild
overview: Rebuild the wildland firefighting simulator with improved architecture using Unity URP, Netcode for GameObjects, and modern GIS-based terrain system. The system will feature tileable terrain with dynamic loading, a fuel code editor with bezier curve graphs, unit leveling system, and automated LinePath-based unit movement.
todos:
  - id: setup_project
    content: Set up Unity URP project with required packages (Netcode for GameObjects, Input System, Addressables)
    status: pending
  - id: gis_layer_system
    content: Create extensible GIS data layer system supporting multiple layer types (fuel codes, elevation, canopy)
    status: pending
    dependencies:
      - setup_project
  - id: gis_parser
    content: Implement GIS data parser for GeoTiff and PNG files with pixel sampling
    status: pending
    dependencies:
      - gis_layer_system
  - id: terrain_tile_system
    content: Build tileable terrain system with dynamic loading/unloading based on player position
    status: pending
    dependencies:
      - gis_parser
  - id: terrain_editor
    content: Create terrain editor tools for painting elevation and fuel codes with real-time updates
    status: pending
    dependencies:
      - terrain_tile_system
  - id: fuel_code_data
    content: Implement fuel code data structure with ScriptableObject support
    status: pending
    dependencies:
      - setup_project
  - id: graph_editor
    content: Build custom Unity Editor window for 4-node bezier curve editing (ROS, flame length, slope)
    status: pending
    dependencies:
      - fuel_code_data
  - id: unit_data_system
    content: Create unit data system with ScriptableObjects for all unit types and properties
    status: pending
    dependencies:
      - setup_project
  - id: unit_leveling
    content: Implement unit leveling system with firepoints, property upgrades, and unit unlocks
    status: pending
    dependencies:
      - unit_data_system
  - id: unit_automation
    content: Build automated unit controller system for firefighting behavior
    status: pending
    dependencies:
      - unit_leveling
  - id: linepath_system
    content: Port and improve LinePath system with points, indicators, and path types
    status: pending
    dependencies:
      - unit_automation
  - id: linepath_pathfinding
    content: Implement pathfinding system for units to navigate between LinePathPoints
    status: pending
    dependencies:
      - linepath_system
  - id: network_integration
    content: Integrate Unity Netcode for GameObjects and replace PlayerIO dependencies
    status: pending
    dependencies:
      - terrain_tile_system
      - unit_automation
      - linepath_system
  - id: mobile_optimization
    content: Optimize for mobile platforms with LODs, reduced quality, and performance improvements
    status: pending
    dependencies:
      - network_integration
---

# Wildland Firefighting Simulator Rebuild Plan

## Project Overview

Rebuild the wildland firefighting simulator from the old project with modern Unity architecture, focusing on accuracy to Rothermel's surface fire spread model standards. The system uses GIS-based terrain where each pixel represents 30m x 30m of real-world data, and each pixel equals 1m x 1m in-game.

## Architecture Overview

The system is organized into these major components:

1. **GIS Terrain System** - Tileable, dynamically loaded terrain from GIS data
2. **Fuel Code System** - Editor with bezier curve graphs for fire behavior modeling
3. **Unit System** - Leveling, properties, and automated firefighting
4. **LinePath System** - Automated unit movement along configurable paths
5. **Multiplayer** - Unity Netcode for GameObjects integration
6. **Mobile Support** - Optimized for Android and iOS

## Phase 1: Project Setup & Core Infrastructure

### 1.1 Unity Project Configuration

**Step-by-Step Setup:**

1. **Create New Unity Project:**
   - Open Unity Hub
   - Create new 3D URP (Universal Render Pipeline) project
   - Unity version: 2022.3 LTS or later
   - Project name: `WildlandFirefightingSimulator`
   - Location: Choose appropriate directory

2. **Configure Render Pipeline:**
   - In Project Settings > Graphics, ensure URP Asset is assigned
   - Create/configure URP Asset: `Assets/Settings/UniversalRenderPipelineAsset.asset`
   - Set rendering path: Forward Rendering
   - Enable SRP Batcher for performance
   - Configure shadow distance: 150m default
   - Set max shadow cascades: 4 for PC, 2 for mobile

3. **Build Target Configuration:**
   - File > Build Settings
   - Add platforms: PC, Mac & Linux Standalone, Android, iOS
   - Configure each platform:
     - **PC**: Graphics API DirectX 11/12, Vulkan
     - **Mac**: Graphics API Metal, OpenGL Core
     - **Android**: Minimum API Level 24 (Android 7.0), Target API 33+, Graphics API Vulkan or OpenGL ES 3.0
     - **iOS**: Target iOS 13+, Graphics API Metal

4. **Mobile Optimization Settings (do this early):**
   - Create quality presets:
     - **Ultra** (PC): All settings maxed
     - **High** (PC): Minor reductions
     - **Medium** (Mobile): Significant reductions
     - **Low** (Mobile): Minimum settings
   - Configure each preset:
     - Texture Quality: Ultra/High = Full Res, Medium = Half Res, Low = Quarter Res
     - Anisotropic Textures: Ultra/High = Per Texture, Medium = Disabled, Low = Disabled
     - Anti Aliasing: Ultra = MSAA 4x, High = MSAA 2x, Medium/Low = Disabled
     - Soft Particles: Ultra/High = Enabled, Medium/Low = Disabled
     - Realtime Reflection Probes: Ultra = All, High = 1 per scene, Medium/Low = Disabled
     - Shadow Distance: Ultra = 150m, High = 100m, Medium = 50m, Low = 25m
     - Shadow Cascades: Ultra/High = 4, Medium = 2, Low = 1
     - Shadow Resolution: Ultra = Very High, High = High, Medium = Medium, Low = Low
     - LOD Bias: Ultra = 1.0, High = 0.8, Medium = 0.6, Low = 0.4

5. **Package Installation:**
   - Window > Package Manager
   - Install from Unity Registry:
     - **Unity Netcode for GameObjects** (com.unity.netcode.gameobjects) - Latest stable
     - **Unity Input System** (com.unity.inputsystem) - Latest stable
     - **Addressables** (com.unity.addressables) - Latest stable
     - **TextMeshPro** (com.unity.textmeshpro) - Latest stable
     - **ProBuilder** (com.unity.probuilder) - Optional, for prototyping
   - After installation:
     - Input System: Edit > Project Settings > Player > Active Input Handling > Both (for compatibility)
     - Netcode: Will be configured in Phase 6

6. **Project Settings Configuration:**
   - Edit > Project Settings > Player:
     - Company Name: Your Company
     - Product Name: Wildland Firefighting Simulator
     - Default Icon: Create/assign game icon
     - Resolution and Presentation:
       - Run In Background: Enabled
       - Fullscreen Mode: Fullscreen Window
       - Default Screen Width/Height: 1920x1080
   - Edit > Project Settings > Time:
     - Fixed Timestep: 0.02 (50 Hz)
     - Maximum Allowed Timestep: 0.1
   - Edit > Project Settings > Quality:
     - Assign quality levels created above
     - Set default quality for each platform

7. **Folder Structure Setup:**
   - Create the following folder structure in Assets:
     - Scripts/ (all C# scripts)
     - Prefabs/ (all prefabs)
     - Scenes/ (all scenes)
     - Materials/ (all materials)
     - Textures/ (all textures)
     - Audio/ (all audio files)
     - Data/ (ScriptableObjects and data files)
     - Editor/ (editor scripts, will auto-create Editor subfolder)

8. **Layer Setup:**
   - Edit > Project Settings > Tags and Layers
   - Add layers:
     - Layer 6: "Terrain"
     - Layer 7: "Units"
     - Layer 8: "LinePaths"
     - Layer 9: "Water"
     - Layer 10: "Fire"
     - Layer 11: "UI"

9. **Physics Settings:**
   - Edit > Project Settings > Physics:
     - Gravity: Y = -9.81 (default, will be adjusted per 30m scale)
     - Default Solver Iterations: 6
     - Default Solver Velocity Iterations: 1
   - Note: Will need custom physics for 30m scale (adjust gravity accordingly)

### 1.2 Project Structure

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs
│   │   ├── NetworkManager.cs
│   │   └── TerrainManager.cs
│   ├── GIS/
│   │   ├── GISDataLayer.cs
│   │   ├── GISDataParser.cs
│   │   └── TerrainTile.cs
│   ├── FuelCodes/
│   │   ├── FuelCodeData.cs
│   │   ├── FuelCodeEditor.cs
│   │   └── GraphEditor.cs
│   ├── Units/
│   │   ├── Unit.cs
│   │   ├── UnitData.cs
│   │   ├── UnitLeveling.cs
│   │   └── UnitController.cs
│   └── LinePaths/
│       ├── LinePath.cs
│       ├── LinePathPoint.cs
│       └── LinePathManager.cs
├── Data/
│   ├── FuelCodes/
│   └── Units/
└── Prefabs/
    ├── Terrain/
    ├── Units/
    └── LinePaths/
```

## Phase 2: GIS Terrain System

### 2.1 GIS Data Layer Architecture

Create extensible layer system to support multiple GIS data types. Based on old project's `MapData.cs` and `SaveData.cs` structure, but improved with better separation of concerns.

**Files to create:**

- `Scripts/GIS/GISDataLayer.cs` - Base class for all GIS layers
- `Scripts/GIS/GISDataParser.cs` - Parses GeoTiff/PNG files
- `Scripts/GIS/FuelCodeLayer.cs` - Inherits from GISDataLayer
- `Scripts/GIS/ElevationLayer.cs` - Inherits from GISDataLayer  
- `Scripts/GIS/CanopyLayer.cs` - Inherits from GISDataLayer (future)
- `Scripts/GIS/MapSection.cs` - Container for tile data (replaces old SaveData.MapSection)
- `Scripts/GIS/Pixel.cs` - Pixel data structure (ported from old project with improvements)

**Detailed Implementation Steps:**

#### Step 1: Create GISDataLayer Base Class

**File: `Scripts/GIS/GISDataLayer.cs`**

```csharp
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Base class for all GIS data layers (elevation, fuel codes, canopy, etc.)
/// Each layer stores data for a specific GIS data type
/// </summary>
public abstract class GISDataLayer : ScriptableObject
{
    [Header("Layer Metadata")]
    public string layerName;
    public int width;
    public int height;
    public int tileSize = 122; // Based on old project's chunk size
    
    [Header("Geographic Bounds")]
    public int startLongitudeMeter;
    public int startLatitudeMeter;
    public int longitudeMeterStep = 30; // 30m per pixel
    public int latitudeMeterStep = 30;
    
    /// <summary>
    /// Get data value at specific pixel coordinates
    /// </summary>
    public abstract object GetData(int x, int z);
    
    /// <summary>
    /// Set data value at specific pixel coordinates
    /// </summary>
    public abstract void SetData(int x, int z, object value);
    
    /// <summary>
    /// Get data for a specific tile (chunk)
    /// </summary>
    public abstract object[,] GetTileData(int tileX, int tileZ);
    
    /// <summary>
    /// Set data for a specific tile (chunk)
    /// </summary>
    public abstract void SetTileData(int tileX, int tileZ, object[,] data);
    
    /// <summary>
    /// Convert world position to pixel coordinates
    /// </summary>
    public Vector2Int WorldToPixel(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x);
        int z = Mathf.FloorToInt(-worldPos.z); // Note: Z is negated in old project
        return new Vector2Int(x, z);
    }
    
    /// <summary>
    /// Convert pixel coordinates to world position
    /// </summary>
    public Vector3 PixelToWorld(int x, int z, float elevation = 0f)
    {
        return new Vector3(x, elevation / 30f, -z);
    }
    
    /// <summary>
    /// Get geographic coordinates for a pixel
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
}
```

**Key Implementation Details:**
- Use `tileSize = 122` to match old project's chunk system
- Each pixel represents 30m x 30m in real world = 1m x 1m in-game
- Store data in chunks (tiles) for efficient loading/unloading
- Support both local and geographic coordinate systems

#### Step 2: Create ElevationLayer

**File: `Scripts/GIS/ElevationLayer.cs`**

```csharp
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Stores elevation data (DEM - Digital Elevation Model)
/// Values are in meters, stored as short (int16) to match old project
/// </summary>
[CreateAssetMenu(fileName = "ElevationLayer", menuName = "GIS/Elevation Layer")]
public class ElevationLayer : GISDataLayer
{
    // Store elevation data in chunks: Dictionary<Vector2Int, short[,]>
    // Key is tile coordinate (tileX, tileZ), value is elevation array for that tile
    private Dictionary<Vector2Int, short[,]> elevationTiles = new Dictionary<Vector2Int, short[,]>();
    
    public override object GetData(int x, int z)
    {
        Vector2Int tileCoord = GetTileCoord(x, z);
        Vector2Int localCoord = GetLocalCoord(x, z);
        
        if (elevationTiles.TryGetValue(tileCoord, out short[,] tile))
        {
            return tile[localCoord.x, localCoord.y];
        }
        return (short)0;
    }
    
    public override void SetData(int x, int z, object value)
    {
        Vector2Int tileCoord = GetTileCoord(x, z);
        Vector2Int localCoord = GetLocalCoord(x, z);
        
        if (!elevationTiles.ContainsKey(tileCoord))
        {
            elevationTiles[tileCoord] = new short[tileSize, tileSize];
        }
        
        elevationTiles[tileCoord][localCoord.x, localCoord.y] = (short)value;
    }
    
    public override object[,] GetTileData(int tileX, int tileZ)
    {
        Vector2Int tileCoord = new Vector2Int(tileX, tileZ);
        if (elevationTiles.TryGetValue(tileCoord, out short[,] tile))
        {
            return tile;
        }
        return new short[tileSize, tileSize];
    }
    
    public override void SetTileData(int tileX, int tileZ, object[,] data)
    {
        Vector2Int tileCoord = new Vector2Int(tileX, tileZ);
        elevationTiles[tileCoord] = (short[,])data;
    }
    
    private Vector2Int GetTileCoord(int x, int z)
    {
        return new Vector2Int(
            Mathf.FloorToInt(x / (float)tileSize),
            Mathf.FloorToInt(z / (float)tileSize)
        );
    }
    
    private Vector2Int GetLocalCoord(int x, int z)
    {
        return new Vector2Int(
            x % tileSize,
            z % tileSize
        );
    }
    
    /// <summary>
    /// Get elevation at pixel coordinates (convenience method)
    /// </summary>
    public short GetElevation(int x, int z)
    {
        return (short)GetData(x, z);
    }
    
    /// <summary>
    /// Set elevation at pixel coordinates (convenience method)
    /// </summary>
    public void SetElevation(int x, int z, short elevation)
    {
        SetData(x, z, elevation);
    }
}
```

**Key Implementation Details:**
- Store elevation as `short` (int16) to match old project format
- Values are in meters (e.g., 1500 = 1500m elevation)
- Use Dictionary for efficient tile-based storage
- Each tile is 122x122 pixels (matching old project's chunk size)

#### Step 3: Create FuelCodeLayer

**File: `Scripts/GIS/FuelCodeLayer.cs`**

```csharp
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Stores fuel code data (vegetation/fuel type codes)
/// Values are short (int16) fuel code IDs matching GIS color codes
/// </summary>
[CreateAssetMenu(fileName = "FuelCodeLayer", menuName = "GIS/Fuel Code Layer")]
public class FuelCodeLayer : GISDataLayer
{
    // Store fuel code data in chunks: Dictionary<Vector2Int, short[,]>
    private Dictionary<Vector2Int, short[,]> fuelCodeTiles = new Dictionary<Vector2Int, short[,]>();
    
    // Special fuel codes from old project:
    // 7299 = Roads
    // 7298 = High density urban
    // 7297 = Medium density urban
    // 7296 = Low density urban
    // 99 = Cut line (created by units)
    // 98 = Default/No fuel
    // 91 = Water
    
    public override object GetData(int x, int z)
    {
        Vector2Int tileCoord = GetTileCoord(x, z);
        Vector2Int localCoord = GetLocalCoord(x, z);
        
        if (fuelCodeTiles.TryGetValue(tileCoord, out short[,] tile))
        {
            return tile[localCoord.x, localCoord.y];
        }
        return (short)98; // Default to no fuel
    }
    
    public override void SetData(int x, int z, object value)
    {
        Vector2Int tileCoord = GetTileCoord(x, z);
        Vector2Int localCoord = GetLocalCoord(x, z);
        
        if (!fuelCodeTiles.ContainsKey(tileCoord))
        {
            fuelCodeTiles[tileCoord] = new short[tileSize, tileSize];
            // Initialize to default fuel code
            for (int i = 0; i < tileSize; i++)
            {
                for (int j = 0; j < tileSize; j++)
                {
                    fuelCodeTiles[tileCoord][i, j] = 98;
                }
            }
        }
        
        fuelCodeTiles[tileCoord][localCoord.x, localCoord.y] = (short)value;
    }
    
    public override object[,] GetTileData(int tileX, int tileZ)
    {
        Vector2Int tileCoord = new Vector2Int(tileX, tileZ);
        if (fuelCodeTiles.TryGetValue(tileCoord, out short[,] tile))
        {
            return tile;
        }
        // Return default tile
        short[,] defaultTile = new short[tileSize, tileSize];
        for (int i = 0; i < tileSize; i++)
        {
            for (int j = 0; j < tileSize; j++)
            {
                defaultTile[i, j] = 98;
            }
        }
        return defaultTile;
    }
    
    public override void SetTileData(int tileX, int tileZ, object[,] data)
    {
        Vector2Int tileCoord = new Vector2Int(tileX, tileZ);
        fuelCodeTiles[tileCoord] = (short[,])data;
    }
    
    private Vector2Int GetTileCoord(int x, int z)
    {
        return new Vector2Int(
            Mathf.FloorToInt(x / (float)tileSize),
            Mathf.FloorToInt(z / (float)tileSize)
        );
    }
    
    private Vector2Int GetLocalCoord(int x, int z)
    {
        return new Vector2Int(
            x % tileSize,
            z % tileSize
        );
    }
    
    /// <summary>
    /// Get fuel code at pixel coordinates (convenience method)
    /// </summary>
    public short GetFuelCode(int x, int z)
    {
        return (short)GetData(x, z);
    }
    
    /// <summary>
    /// Set fuel code at pixel coordinates (convenience method)
    /// </summary>
    public void SetFuelCode(int x, int z, short fuelCode)
    {
        SetData(x, z, fuelCode);
    }
    
    /// <summary>
    /// Check if fuel code is a road type
    /// </summary>
    public static bool IsRoad(short fuelCode)
    {
        return fuelCode == 7299 || fuelCode == 7298 || fuelCode == 7297 || fuelCode == 7296;
    }
    
    /// <summary>
    /// Check if fuel code is urban
    /// </summary>
    public static bool IsUrban(short fuelCode)
    {
        return fuelCode == 7298 || fuelCode == 7297 || fuelCode == 7296;
    }
    
    /// <summary>
    /// Check if fuel code is water
    /// </summary>
    public static bool IsWater(short fuelCode)
    {
        return fuelCode == 91 || fuelCode == 7292;
    }
}
```

**Key Implementation Details:**
- Store fuel codes as `short` (int16) matching GIS color codes
- Default fuel code is 98 (no fuel)
- Special codes: 7299=Roads, 7298/7297/7296=Urban, 99=Cut line, 91=Water
- Provide helper methods for common checks (IsRoad, IsUrban, IsWater)

#### Step 4: Create Pixel Class

**File: `Scripts/GIS/Pixel.cs`**

Port from old project with improvements. This class represents a single pixel (30m x 30m real world = 1m x 1m in-game).

```csharp
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a single pixel in the GIS terrain system
/// Each pixel = 30m x 30m real world = 1m x 1m in-game
/// Based on old project's Pixel.cs but improved
/// </summary>
public class Pixel
{
    public int x;
    public int z;
    
    // Pixel state (ALIVE = vegetation, FIRE = burning, DEAD = burned)
    public PixelState state = PixelState.ALIVE;
    
    // Fire properties
    public float fireIntensity = 0f; // kilowatts per meter
    public float flameLength = 0f; // meters
    public float ROS = 0f; // Rate of Spread (chains/hour or pixels/hour)
    public int spreadAtTime = 0; // Game time when fire will spread
    public int burnOutTime = 0; // Game time when fire will burn out
    public int timeActivated = 0; // Game time when fire started
    public int burnLevel = 0; // Burn intensity level (0-3)
    
    // Fire spread tracking
    public Pixel fromTerrain = null; // Which pixel this fire spread from
    public bool hasSpread = false;
    public bool isActivated = false;
    public float currentPercentAngle = 1f;
    public float slopeFactor = 1f;
    
    // Suppression properties
    public float waterAmount = 0f;
    public float retardantAmount = 0f;
    public float rateOfEvaporation = 10f; // per second
    public bool isCurrentlyEvaporating = false;
    public bool isIgnoreFuelBreak = false;
    
    // Fuel break properties
    public float favourableFuelBreak = 0f;
    
    // Active units working on this pixel
    public List<Unit> activeUnits = new List<Unit>();
    
    // Visual properties
    public Color fuelCodeColor = Color.black;
    public Color historyColor = Color.clear;
    public Color urbanColor = Color.clear;
    public float historyLevel = 0f;
    public float urbanLevel = 0f;
    
    // Containment
    public GameObject containerBarrierGO;
    public ContainmentBarrier containmentBarrier;
    public GameObject fireLineGO;
    
    // Vegetation
    public Vegetation vegetation;
    
    // Hose connections
    public List<Hose> hosePoints = new List<Hose>();
    
    // Reference to game manager
    private GameManager gameManager;
    
    // Cached properties
    private short? _elevation = null;
    private short? _fuelCode = null;
    private short? _originalFuelCode = null;
    private float? _slope = null;
    
    public Pixel(GameManager gameManager, int x, int z)
    {
        this.gameManager = gameManager;
        this.x = x;
        this.z = z;
    }
    
    /// <summary>
    /// Get elevation in meters (cached)
    /// </summary>
    public short elevation
    {
        get
        {
            if (!_elevation.HasValue)
            {
                _elevation = gameManager.mapData.GetElevation(x, z);
            }
            return _elevation.Value;
        }
        set
        {
            _elevation = value;
            gameManager.mapData.SetElevation(x, z, value);
            gameManager.terrainGenerator.PixelElevationChanged(this);
        }
    }
    
    /// <summary>
    /// Get fuel code (cached)
    /// </summary>
    public short fuelCode
    {
        get
        {
            if (!_fuelCode.HasValue)
            {
                _fuelCode = gameManager.mapData.GetFuelCode(x, z);
            }
            return _fuelCode.Value;
        }
        set
        {
            if (!_originalFuelCode.HasValue) _originalFuelCode = value;
            _fuelCode = value;
            gameManager.mapData.SetFuelCode(x, z, value);
            gameManager.terrainGenerator.PixelFuelCodeChanged(this);
        }
    }
    
    /// <summary>
    /// Get original fuel code before any changes
    /// </summary>
    public short originalFuelCode
    {
        get
        {
            if (!_originalFuelCode.HasValue)
            {
                _originalFuelCode = fuelCode;
            }
            return _originalFuelCode.Value;
        }
    }
    
    /// <summary>
    /// Get base ROS (Rate of Spread) from fuel code and wind speed
    /// </summary>
    public float baseROS
    {
        get
        {
            return gameManager.GetROS(fuelCode, gameManager.windSpeed);
        }
    }
    
    /// <summary>
    /// Get base flame length from fuel code and wind speed
    /// </summary>
    public float baseFlameLength
    {
        get
        {
            return gameManager.GetFlameLength(fuelCode, gameManager.windSpeed);
        }
    }
    
    /// <summary>
    /// Get slope in degrees (cached, calculated from neighboring pixels)
    /// </summary>
    public float slope
    {
        get
        {
            if (!_slope.HasValue)
            {
                _slope = GetSlope();
            }
            return _slope.Value;
        }
    }
    
    /// <summary>
    /// Calculate slope from neighboring elevation values
    /// Based on old project's GetSlope() method
    /// </summary>
    private float GetSlope()
    {
        float topValue = gameManager.mapData.GetElevation(x, z - 1);
        float leftValue = gameManager.mapData.GetElevation(x - 1, z);
        float rightValue = gameManager.mapData.GetElevation(x + 1, z);
        float bottomValue = gameManager.mapData.GetElevation(x, z + 1);
        
        float slx = (rightValue - leftValue) / 30f; // 30m per pixel
        float sly = (bottomValue - topValue) / 30f;
        float sl0 = Mathf.Sqrt(slx * slx + sly * sly);
        
        return Mathf.Atan(sl0) * 180f / Mathf.PI; // Convert to degrees
    }
    
    /// <summary>
    /// Get world position of this pixel
    /// </summary>
    public Vector3 position
    {
        get
        {
            return new Vector3(x, elevation / 30f, -z);
        }
    }
    
    /// <summary>
    /// Get 2D location (x, z)
    /// </summary>
    public Vector2 location
    {
        get
        {
            return new Vector2(x, z);
        }
    }
    
    /// <summary>
    /// Get geographic coordinates
    /// </summary>
    public GeoLocation GetGeoLocation()
    {
        return gameManager.mapData.GetGeoCoord(x + 1, z + 1);
    }
    
    /// <summary>
    /// Update fire intensity from flame length
    /// Formula: I = 259.833 * (L ^ 2.174) * 30m
    /// Based on Rothermel's fire behavior model
    /// </summary>
    public void UpdateFireIntensityFromFlameLength()
    {
        fireIntensity = 259.833f * Mathf.Pow(flameLength, 2.174f) * 30f;
    }
    
    /// <summary>
    /// Update flame length from fire intensity
    /// Inverse of above formula
    /// </summary>
    public void UpdateFlameLengthFromFireIntensity()
    {
        flameLength = Mathf.Pow((fireIntensity / 30f) / 259.833f, 1f / 2.174f);
    }
    
    /// <summary>
    /// Get total suppression output from all active units
    /// </summary>
    public float TotalOutput()
    {
        float output = 0;
        foreach (Unit unit in activeUnits)
        {
            if (unit.currentAction == UnitAction.SPRAY)
            {
                output += unit.unitData.Get("output");
            }
            else if (unit.currentAction == UnitAction.SUPPRESS)
            {
                output += unit.unitData.Get("suppressionSpeed");
            }
        }
        return output;
    }
    
    /// <summary>
    /// Add active unit working on this pixel
    /// </summary>
    public void AddActiveUnit(Unit unit)
    {
        if (!activeUnits.Contains(unit))
        {
            activeUnits.Add(unit);
        }
    }
    
    /// <summary>
    /// Remove active unit from this pixel
    /// </summary>
    public void RemoveActiveUnit(Unit unit)
    {
        if (activeUnits.Contains(unit))
        {
            activeUnits.Remove(unit);
        }
    }
    
    /// <summary>
    /// Evaporate water over time
    /// </summary>
    public void WaterEvaporate()
    {
        if (waterAmount > 0)
        {
            waterAmount -= rateOfEvaporation * gameManager.gameSpeed * Time.deltaTime;
            if (waterAmount < 0)
            {
                waterAmount = 0;
                gameManager.evaporatingPixels.Remove(this);
            }
            gameManager.terrainGenerator.UpdatePixelWaterOrRetardant(this);
        }
    }
    
    /// <summary>
    /// Reset pixel to default state
    /// </summary>
    public void Reset()
    {
        state = PixelState.ALIVE;
        flameLength = 0;
        fireIntensity = 0;
        timeActivated = 0;
        burnLevel = 0;
        isIgnoreFuelBreak = false;
        retardantAmount = 0;
        waterAmount = 0;
        spreadAtTime = 0;
        fromTerrain = null;
        hosePoints.Clear();
        _slope = null; // Reset cache
    }
    
    /// <summary>
    /// Reset pixel and restore original fuel code
    /// </summary>
    public void ResetMap()
    {
        Reset();
        fuelCode = originalFuelCode;
    }
}

/// <summary>
/// Pixel state enumeration
/// </summary>
public enum PixelState
{
    ALIVE = 0,  // Vegetation alive, not burning
    FIRE = 1,   // Currently on fire
    DEAD = 2    // Burned out, no longer burning
}
```

**Key Implementation Details:**
- Port from old project's `Pixel.cs` with improvements
- Cache elevation, fuel code, and slope for performance
- Support fire spread tracking (fromTerrain)
- Track active units working on pixel
- Support water/retardant suppression
- Calculate slope from neighboring elevations
- Convert between fire intensity and flame length using Rothermel's formula

#### Step 5: Create GISDataParser

**File: `Scripts/GIS/GISDataParser.cs`**

Based on old project's `GISDataParser.cs`, but improved for better structure.

```csharp
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using System;

/// <summary>
/// Parses GIS data files (GeoTiff, PNG, XYZ format)
/// Based on old project's GISDataParser.cs
/// </summary>
public class GISDataParser : MonoBehaviour
{
    [Header("GIS File Sources")]
    public TextAsset elevationAsset;
    public TextAsset fuelCodeAsset;
    public TextAsset vegetationAsset;
    
    [Header("Parsing Settings")]
    public int numColumns;
    public int numRows;
    public int latitudeTopMeters;
    public int latitudeBottomMeters;
    public int longitudeLeftMeters;
    public int longitudeRightMeters;
    
    private GameManager gameManager;
    private int gridSizeInMeters = 122 * 30; // 122 pixels * 30m per pixel
    
    // Parsed data
    private string elevationAssetText;
    private string fuelCodeAssetText;
    private string vegetationAssetText;
    
    void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }
    
    /// <summary>
    /// Load GIS data files asynchronously
    /// </summary>
    public async Task LoadGISDataAsync()
    {
        gameManager.consoleLog.text = "GIS loading.. 0%";
        await Task.Delay(1);
        
        // Load elevation data (DEM)
        elevationAssetText = LoadTextFile("d.xyz"); // d = DEM (Digital Elevation Model)
        gameManager.consoleLog.text = "GIS loaded.. 33%";
        await Task.Delay(1);
        
        // Load fuel code data
        fuelCodeAssetText = LoadTextFile("f.xyz"); // f = Fuel codes
        gameManager.consoleLog.text = "GIS loaded.. 67%";
        await Task.Delay(1);
        
        // Load vegetation data
        vegetationAssetText = LoadTextFile("v.xyz"); // v = Vegetation
        gameManager.consoleLog.text = "GIS loaded.. 100%";
    }
    
    /// <summary>
    /// Load text file from Resources or StreamingAssets
    /// </summary>
    private string LoadTextFile(string filename)
    {
        // Try Resources first
        TextAsset asset = Resources.Load<TextAsset>(filename);
        if (asset != null)
        {
            return asset.text;
        }
        
        // Try StreamingAssets
        string path = Path.Combine(Application.streamingAssetsPath, filename);
        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }
        
        Debug.LogError($"Could not load GIS file: {filename}");
        return "";
    }
    
    /// <summary>
    /// Parse map parameters from output_parameters.txt
    /// </summary>
    public void VerifySource()
    {
        string mapParametersText = LoadTextFile("output_parameters.txt");
        if (string.IsNullOrEmpty(mapParametersText))
        {
            Debug.LogError("Could not load output_parameters.txt");
            return;
        }
        
        string[] mapData = mapParametersText.Split('\n');
        int totalParamsFound = 0;
        
        foreach (string line in mapData)
        {
            string[] lineParts = line.Split(':');
            if (lineParts.Length < 2) continue;
            
            string[] values = lineParts[1].Split(' ');
            
            if (lineParts[0] == "Number of Columns")
            {
                numColumns = int.Parse(values[1]);
                totalParamsFound++;
            }
            else if (lineParts[0] == "Number of Rows")
            {
                numRows = int.Parse(values[1]);
                totalParamsFound++;
            }
            else if (lineParts[0] == "Top edge Native")
            {
                string[] valueInt = values[1].Split('.');
                latitudeTopMeters = int.Parse(valueInt[0]);
                latitudeBottomMeters = latitudeTopMeters - numRows * 30;
                totalParamsFound++;
            }
            else if (lineParts[0] == "Left edge Native")
            {
                string[] valueInt = values[1].Split('.');
                longitudeLeftMeters = int.Parse(valueInt[0]);
                longitudeRightMeters = longitudeLeftMeters + numColumns * 30;
                totalParamsFound++;
            }
        }
        
        if (totalParamsFound == 4)
        {
            CalculateInitialParameters();
            gameManager.consoleLog.text = "Source Verified!";
        }
        else
        {
            Debug.LogError($"Failed to parse all parameters. Found {totalParamsFound}/4");
        }
    }
    
    /// <summary>
    /// Calculate initial map parameters
    /// Based on old project's CalculateInitialParameters()
    /// </summary>
    private void CalculateInitialParameters()
    {
        int longitudeMeterStep = (longitudeRightMeters - longitudeLeftMeters) / numColumns;
        int latitudeMeterStep = (latitudeBottomMeters - latitudeTopMeters) / numRows;
        
        // Calculate start/end coordinates aligned to grid
        int startLongitudeMeter = ((int)Mathf.Floor(longitudeLeftMeters / (float)gridSizeInMeters) + 
                                   Mathf.Abs(longitudeMeterStep) / longitudeMeterStep) * gridSizeInMeters;
        int startLatitudeMeter = ((int)Mathf.Floor(latitudeTopMeters / (float)gridSizeInMeters) + 
                                  Mathf.Abs(latitudeMeterStep) / latitudeMeterStep) * gridSizeInMeters;
        
        int endLongitudeMeter = (((int)Mathf.Floor(longitudeLeftMeters + numColumns * longitudeMeterStep) / gridSizeInMeters) - 
                                 Mathf.Abs(longitudeMeterStep) / longitudeMeterStep) * gridSizeInMeters;
        int endLatitudeMeter = (((int)Mathf.Floor(latitudeTopMeters + numRows * latitudeMeterStep) / gridSizeInMeters) - 
                                Mathf.Abs(latitudeMeterStep) / latitudeMeterStep) * gridSizeInMeters;
        
        // Calculate map dimensions in pixels
        int xWidth = (endLongitudeMeter - startLongitudeMeter) / longitudeMeterStep;
        int zWidth = (endLatitudeMeter - startLatitudeMeter) / latitudeMeterStep;
        
        Debug.Log($"Map dimensions: {xWidth} x {zWidth} pixels");
        Debug.Log($"Start: ({startLongitudeMeter}, {startLatitudeMeter})");
        Debug.Log($"End: ({endLongitudeMeter}, {endLatitudeMeter})");
    }
    
    /// <summary>
    /// Parse GIS data and populate MapData
    /// Based on old project's ParseMapDataAsync()
    /// </summary>
    public async Task ParseMapDataAsync()
    {
        VerifySource();
        
        if (string.IsNullOrEmpty(elevationAssetText) || 
            string.IsNullOrEmpty(fuelCodeAssetText) || 
            string.IsNullOrEmpty(vegetationAssetText))
        {
            Debug.LogError("GIS data files not loaded. Call LoadGISDataAsync() first.");
            return;
        }
        
        string[] demData = elevationAssetText.Split('\n');
        string[] fireData = fuelCodeAssetText.Split('\n');
        string[] vegetationData = vegetationAssetText.Split('\n');
        
        int longitudeMeterStep = (longitudeRightMeters - longitudeLeftMeters) / numColumns;
        int latitudeMeterStep = (latitudeBottomMeters - latitudeTopMeters) / numRows;
        
        // Calculate bounds
        int startLongitudeMeter = ((int)Mathf.Floor(longitudeLeftMeters / (float)gridSizeInMeters) + 
                                   Mathf.Abs(longitudeMeterStep) / longitudeMeterStep) * gridSizeInMeters;
        int startLatitudeMeter = ((int)Mathf.Floor(latitudeTopMeters / (float)gridSizeInMeters) + 
                                  Mathf.Abs(latitudeMeterStep) / latitudeMeterStep) * gridSizeInMeters;
        
        int endLongitudeMeter = (((int)Mathf.Floor(longitudeLeftMeters + numColumns * longitudeMeterStep) / gridSizeInMeters) - 
                                 Mathf.Abs(longitudeMeterStep) / longitudeMeterStep) * gridSizeInMeters;
        int endLatitudeMeter = (((int)Mathf.Floor(latitudeTopMeters + numRows * latitudeMeterStep) / gridSizeInMeters) - 
                                Mathf.Abs(latitudeMeterStep) / latitudeMeterStep) * gridSizeInMeters;
        
        int xWidth = (endLongitudeMeter - startLongitudeMeter) / longitudeMeterStep;
        int zWidth = (endLatitudeMeter - startLatitudeMeter) / latitudeMeterStep;
        
        // Initialize MapData
        gameManager.mapData = new MapData(xWidth, zWidth, startLongitudeMeter, startLatitudeMeter, 
                                         longitudeMeterStep, latitudeMeterStep, false, false, true);
        
        int x = 0;
        int z = 0;
        int curValue = 0;
        
        for (int i = 0; i < demData.Length; i++)
        {
            string[] demDetails = demData[i].Split(' ');
            string[] fireDetails = fireData[i].Split(' ');
            string[] vegetationDetails = vegetationData[i].Split(' ');
            
            if (demDetails.Length < 2) continue;
            
            int xCoord = int.Parse(demDetails[0]);
            int zCoord = int.Parse(demDetails[1]);
            
            // Check if we've moved to a new row
            if (curValue != zCoord)
            {
                x = 0;
                z++;
                curValue = zCoord;
            }
            
            // Check if coordinates are within bounds
            bool isValidCoord = false;
            if (longitudeMeterStep == 30 && latitudeMeterStep == -30)
            {
                isValidCoord = (xCoord >= startLongitudeMeter && zCoord <= startLatitudeMeter && 
                               xCoord < endLongitudeMeter && zCoord > endLatitudeMeter);
            }
            else if (longitudeMeterStep == 30 && latitudeMeterStep == 30)
            {
                isValidCoord = (xCoord >= startLongitudeMeter && zCoord >= startLatitudeMeter && 
                               xCoord < endLongitudeMeter && zCoord < endLatitudeMeter);
            }
            // Add other coordinate system checks as needed
            
            if (isValidCoord)
            {
                short demValue = Convert.ToInt16(demDetails[2]);
                short fireValue = Convert.ToInt16(fireDetails[2]);
                short vegetationValue = Convert.ToInt16(vegetationDetails[2]);
                
                // Handle special cases (from old project logic)
                if (fireValue == 91) // Water
                {
                    // Override with vegetation value if it's urban/road
                    switch (vegetationValue)
                    {
                        case 7299: fireValue = 7299; break; // Road
                        case 7298: fireValue = 7298; break; // High density urban
                        case 7297: fireValue = 7297; break; // Medium density urban
                        case 7296: fireValue = 7296; break; // Low density urban
                    }
                }
                
                // Handle invalid elevation (-32768 is no data)
                if (demValue == -32768)
                {
                    demValue = 0;
                    fireValue = 0;
                }
                
                // Calculate local coordinates within map
                int localX = x - (startLongitudeMeter - longitudeLeftMeters) / longitudeMeterStep;
                int localZ = z - (startLatitudeMeter - latitudeTopMeters) / latitudeMeterStep;
                
                if (localX >= 0 && localX < xWidth && localZ >= 0 && localZ < zWidth)
                {
                    gameManager.mapData.SetElevation(localX, localZ, demValue);
                    gameManager.mapData.SetFuelCode(localX, localZ, fireValue);
                }
            }
            
            x++;
            
            // Progress update every 10000 pixels
            if (i % 10000 == 0)
            {
                gameManager.consoleLog.text = $"Processing {Mathf.Round(100 * (float)i / demData.Length)}%";
                await Task.Delay(1);
            }
        }
        
        gameManager.consoleLog.text = "Processed 100%";
    }
}
```

**Key Implementation Details:**
- Support XYZ format (space-separated: x y value)
- Parse elevation (DEM), fuel codes, and vegetation data
- Handle special fuel codes (roads, urban, water)
- Support different coordinate system orientations
- Async parsing with progress updates
- Validate coordinates before setting data

### 2.2 Terrain Tile System

Based on old project's `TerrainChunk.cs` and `TerrainGenerator.cs`, but improved with better structure and performance.

**Files to create:**

- `Scripts/GIS/TerrainTile.cs` - Individual tile component (replaces TerrainChunk)
- `Scripts/Core/TerrainManager.cs` - Manages tile loading/unloading
- `Scripts/GIS/TerrainGenerator.cs` - Generates mesh from GIS data
- `Scripts/GIS/MeshSettings.cs` - Settings for mesh generation
- `Scripts/GIS/HeightMapSettings.cs` - Settings for height map
- `Scripts/GIS/LODInfo.cs` - Level of Detail information

**Detailed Implementation Steps:**

#### Step 1: Create MeshSettings

**File: `Scripts/GIS/MeshSettings.cs`**

```csharp
using UnityEngine;

/// <summary>
/// Settings for terrain mesh generation
/// Based on old project's MeshSettings
/// </summary>
[CreateAssetMenu(fileName = "MeshSettings", menuName = "GIS/Mesh Settings")]
public class MeshSettings : ScriptableObject
{
    public const int numSupportedLODs = 5;
    public const int numSupportedChunkSizes = 9;
    public const int numSupportedFlatshadedChunkSizes = 3;
    public static readonly int[] supportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };
    
    [Range(0, numSupportedChunkSizes - 1)]
    public int chunkSizeIndex = 0;
    
    [Range(0, numSupportedFlatshadedChunkSizes - 1)]
    public int flatshadedChunkSizeIndex = 0;
    
    public float meshScale = 1f;
    public bool useFlatShading;
    
    // Number of vertices per line of mesh rendered at LOD = 0
    // Includes the 2 extra vertices that are excluded from final mesh, but used for calculating normals
    public int numVertsPerLine
    {
        get
        {
            return supportedChunkSizes[(useFlatShading) ? flatshadedChunkSizeIndex : chunkSizeIndex] + 5;
        }
    }
    
    public float meshWorldSize
    {
        get
        {
            return (numVertsPerLine - 3) * meshScale;
        }
    }
}
```

#### Step 2: Create HeightMapSettings

**File: `Scripts/GIS/HeightMapSettings.cs`**

```csharp
using UnityEngine;

/// <summary>
/// Settings for height map generation
/// </summary>
[CreateAssetMenu(fileName = "HeightMapSettings", menuName = "GIS/Height Map Settings")]
public class HeightMapSettings : ScriptableObject
{
    public float heightMultiplier = 1f;
    public AnimationCurve heightCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public bool useFalloff = false;
    public AnimationCurve falloffCurve = AnimationCurve.Linear(0, 1, 1, 0);
}
```

#### Step 3: Create LODInfo

**File: `Scripts/GIS/LODInfo.cs`**

```csharp
using UnityEngine;

/// <summary>
/// Level of Detail information for terrain chunks
/// </summary>
[System.Serializable]
public struct LODInfo
{
    [Range(0, MeshSettings.numSupportedLODs - 1)]
    public int lod;
    public float visibleDstThreshold;
    
    public float sqrVisibleDstThreshold
    {
        get
        {
            return visibleDstThreshold * visibleDstThreshold;
        }
    }
}
```

#### Step 4: Create TerrainTile

**File: `Scripts/GIS/TerrainTile.cs`**

This replaces the old project's `TerrainChunk.cs`. Each tile represents a 122x122 pixel chunk of terrain.

```csharp
using UnityEngine;
using System.Collections;

/// <summary>
/// Represents a single terrain tile (chunk)
/// Based on old project's TerrainChunk.cs but improved
/// Each tile is 122x122 pixels = 3.66km x 3.66km real world = 122m x 122m in-game
/// </summary>
public class TerrainTile
{
    public Vector2 coord; // Tile coordinate (tileX, tileZ)
    
    GameObject meshObject;
    Vector2 sampleCentre;
    Bounds bounds;
    
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;
    
    LODInfo[] detailLevels;
    LODMesh[] lodMeshes;
    int colliderLODIndex;
    
    HeightMap heightMap;
    bool heightMapReceived;
    int previousLODIndex = -1;
    bool hasSetCollider;
    
    HeightMapSettings heightMapSettings;
    MeshSettings meshSettings;
    Transform viewer;
    Transform parent;
    Material material;
    
    // Reference to game manager
    GameManager gameManager;
    
    // Tile data
    public int tileX { get; private set; }
    public int tileZ { get; private set; }
    
    // Event for visibility changes
    public System.Action<TerrainTile, bool> onVisibilityChanged;
    
    public TerrainTile(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, 
                      LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, 
                      GameManager gameManager, Material material)
    {
        this.coord = coord;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.detailLevels = detailLevels;
        this.colliderLODIndex = colliderLODIndex;
        this.parent = parent;
        this.viewer = viewer;
        this.gameManager = gameManager;
        this.material = material;
        
        sampleCentre = coord * meshSettings.meshWorldSize;
        Vector2 position = coord * meshSettings.meshWorldSize;
        bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);
        
        // Calculate tile coordinates
        tileX = Mathf.FloorToInt(coord.x / meshSettings.meshScale);
        tileZ = Mathf.FloorToInt(-coord.y / meshSettings.meshScale); // Note: Y is negated
        
        lodMeshes = new LODMesh[detailLevels.Length];
        for (int i = 0; i < detailLevels.Length; i++)
        {
            lodMeshes[i] = new LODMesh(detailLevels[i].lod);
            lodMeshes[i].updateCallback += UpdateTerrainChunk;
            if (i == colliderLODIndex)
            {
                lodMeshes[i].updateCallback += UpdateCollisionMesh;
            }
        }
    }
    
    public void Load()
    {
        // Request height map data from MapData
        RequestHeightMap();
    }
    
    void RequestHeightMap()
    {
        // Get height map data for this tile from MapData
        heightMap = HeightMapGenerator.GenerateHeightMap(
            meshSettings.numVertsPerLine,
            meshSettings.numVertsPerLine,
            heightMapSettings,
            sampleCentre,
            gameManager.mapData,
            tileX,
            tileZ
        );
        heightMapReceived = true;
        
        UpdateTerrainChunk();
    }
    
    public void UpdateTerrainChunk()
    {
        if (!heightMapReceived) return;
        
        float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewer.position));
        bool wasVisible = IsVisible();
        bool visible = viewerDstFromNearestEdge <= detailLevels[detailLevels.Length - 1].visibleDstThreshold;
        
        if (visible)
        {
            int lodIndex = 0;
            for (int i = 0; i < detailLevels.Length - 1; i++)
            {
                if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold)
                {
                    lodIndex = i + 1;
                }
                else
                {
                    break;
                }
            }
            
            if (lodIndex != previousLODIndex)
            {
                LODMesh lodMesh = lodMeshes[lodIndex];
                if (lodMesh.hasMesh)
                {
                    previousLODIndex = lodIndex;
                    meshFilter.mesh = lodMesh.mesh;
                }
                else if (!lodMesh.hasRequestedMesh)
                {
                    lodMesh.RequestMesh(heightMap, meshSettings);
                }
            }
        }
        
        if (wasVisible != visible)
        {
            SetVisible(visible);
            if (onVisibilityChanged != null)
            {
                onVisibilityChanged(this, visible);
            }
        }
    }
    
    public void UpdateCollisionMesh()
    {
        if (!hasSetCollider)
        {
            LODMesh collisionLODMesh = lodMeshes[colliderLODIndex];
            if (collisionLODMesh.hasMesh)
            {
                meshCollider.sharedMesh = collisionLODMesh.mesh;
                hasSetCollider = true;
            }
        }
    }
    
    public void SetVisible(bool visible)
    {
        if (meshObject != null)
        {
            meshObject.SetActive(visible);
        }
    }
    
    public bool IsVisible()
    {
        return meshObject != null && meshObject.activeSelf;
    }
    
    void OnHeightMapReceived(object heightMap)
    {
        this.heightMap = (HeightMap)heightMap;
        heightMapReceived = true;
        
        UpdateTerrainChunk();
    }
    
    public void OnMapDataChanged()
    {
        // Regenerate height map when map data changes
        heightMapReceived = false;
        RequestHeightMap();
    }
    
    public void CreateMeshObject()
    {
        meshObject = new GameObject("Terrain Chunk");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        meshRenderer.material = material;
        
        meshObject.transform.position = new Vector3(position.x, 0, position.y);
        meshObject.transform.parent = parent;
        SetVisible(false);
    }
    
    Vector2 position
    {
        get
        {
            return coord * meshSettings.meshWorldSize;
        }
    }
}

/// <summary>
/// LOD Mesh data
/// </summary>
class LODMesh
{
    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;
    public int lod;
    public System.Action updateCallback;
    
    public LODMesh(int lod)
    {
        this.lod = lod;
    }
    
    void OnMeshDataReceived(object meshData)
    {
        mesh = ((MeshData)meshData).CreateMesh();
        hasMesh = true;
        
        updateCallback();
    }
    
    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
    {
        hasRequestedMesh = true;
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod);
        mesh = meshData.CreateMesh();
        hasMesh = true;
        updateCallback();
    }
}
```

**Key Implementation Details:**
- Each tile is 122x122 pixels (matching old project's chunk size)
- Support LOD (Level of Detail) for performance
- Generate mesh from height map data
- Support collision mesh at specific LOD
- Handle visibility based on viewer distance
- Update when map data changes

#### Step 5: Create TerrainGenerator

**File: `Scripts/GIS/TerrainGenerator.cs`**

Based on old project's `TerrainGenerator.cs`, but improved.

```csharp
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generates and manages terrain tiles
/// Based on old project's TerrainGenerator.cs
/// </summary>
public class TerrainGenerator : MonoBehaviour
{
    const float viewerMoveThresholdForChunkUpdate = 25f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;
    
    [Header("LOD Settings")]
    public int colliderLODIndex;
    public LODInfo[] detailLevels;
    
    [Header("Mesh Settings")]
    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureSettings;
    
    [Header("References")]
    public Transform viewer;
    public Material mapMaterial;
    
    Vector2 viewerPosition;
    Vector2 viewerPositionOld;
    
    float meshWorldSize;
    int chunksVisibleInViewDst;
    
    Dictionary<Vector2, TerrainTile> terrainChunkDictionary = new Dictionary<Vector2, TerrainTile>();
    List<TerrainTile> visibleTerrainChunks = new List<TerrainTile>();
    
    GameManager gameManager;
    
    void Start()
    {
        gameManager = GetComponent<GameManager>();
        
        float maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
        meshWorldSize = meshSettings.meshWorldSize;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshWorldSize);
        
        UpdateVisibleChunks();
    }
    
    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        
        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
    }
    
    /// <summary>
    /// Create terrain for specified width and height (in tiles)
    /// </summary>
    public void CreateTerrain(int width, int height)
    {
        for (int z = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                Vector2 chunkCoord = new Vector2(x * meshSettings.meshScale, -z * meshSettings.meshScale);
                if (!terrainChunkDictionary.ContainsKey(chunkCoord))
                {
                    TerrainTile newChunk = new TerrainTile(
                        chunkCoord,
                        heightMapSettings,
                        meshSettings,
                        detailLevels,
                        colliderLODIndex,
                        transform,
                        viewer,
                        gameManager,
                        mapMaterial
                    );
                    terrainChunkDictionary.Add(chunkCoord, newChunk);
                    newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
                    newChunk.Load();
                }
            }
        }
    }
    
    void UpdateVisibleChunks()
    {
        HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
        for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--)
        {
            alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
            visibleTerrainChunks[i].UpdateTerrainChunk();
        }
        
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);
        
        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(
                    (currentChunkCoordX + xOffset) * meshSettings.meshScale,
                    (currentChunkCoordY + yOffset) * meshSettings.meshScale
                );
                
                if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                {
                    if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                    {
                        terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    }
                }
            }
        }
    }
    
    void OnTerrainChunkVisibilityChanged(TerrainTile chunk, bool isVisible)
    {
        if (isVisible)
        {
            visibleTerrainChunks.Add(chunk);
        }
        else
        {
            visibleTerrainChunks.Remove(chunk);
        }
    }
    
    /// <summary>
    /// Update pixel when elevation changes
    /// </summary>
    public void PixelElevationChanged(Pixel pixel)
    {
        Vector2 viewedChunkCoord = new Vector2(
            Mathf.Floor(pixel.x / meshSettings.meshWorldSize),
            -Mathf.Floor(pixel.z / meshSettings.meshWorldSize)
        );
        if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
        {
            terrainChunkDictionary[viewedChunkCoord].OnMapDataChanged();
        }
    }
    
    /// <summary>
    /// Update pixel when fuel code changes
    /// </summary>
    public void PixelFuelCodeChanged(Pixel pixel)
    {
        // Update texture/material for fuel code change
        Vector2 viewedChunkCoord = new Vector2(
            Mathf.Floor(pixel.x / meshSettings.meshWorldSize),
            -Mathf.Floor(pixel.z / meshSettings.meshWorldSize)
        );
        if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
        {
            // Update texture to reflect fuel code change
            UpdateChunkTexture(viewedChunkCoord);
        }
    }
    
    /// <summary>
    /// Update pixel when it catches fire
    /// </summary>
    public void CatchPixelOnFire(Pixel pixel)
    {
        Vector2 viewedChunkCoord = new Vector2(
            Mathf.Floor(pixel.x / meshSettings.meshWorldSize),
            -Mathf.Floor(pixel.z / meshSettings.meshWorldSize)
        );
        if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
        {
            // Update fire texture/effects
            UpdateFireTexture(viewedChunkCoord, pixel);
        }
    }
    
    /// <summary>
    /// Update pixel when water/retardant is applied
    /// </summary>
    public void UpdatePixelWaterOrRetardant(Pixel pixel)
    {
        Vector2 viewedChunkCoord = new Vector2(
            Mathf.Floor(pixel.x / meshSettings.meshWorldSize),
            -Mathf.Floor(pixel.z / meshSettings.meshWorldSize)
        );
        if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
        {
            // Update texture to show water/retardant
            UpdateWaterTexture(viewedChunkCoord, pixel);
        }
    }
    
    /// <summary>
    /// Update pixel when it dies (burns out)
    /// </summary>
    public void PixelDied(int x, int z, Pixel pixel)
    {
        Vector2 viewedChunkCoord = new Vector2(
            Mathf.Floor(x / meshSettings.meshWorldSize),
            -Mathf.Floor(z / meshSettings.meshWorldSize)
        );
        if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
        {
            // Update texture to show burned area
            UpdateBurnTexture(viewedChunkCoord, pixel);
        }
    }
    
    void UpdateChunkTexture(Vector2 chunkCoord)
    {
        // Update material texture for fuel code changes
        // Implementation depends on shader setup
    }
    
    void UpdateFireTexture(Vector2 chunkCoord, Pixel pixel)
    {
        // Update material texture for fire visualization
        // Implementation depends on shader setup
    }
    
    void UpdateWaterTexture(Vector2 chunkCoord, Pixel pixel)
    {
        // Update material texture for water/retardant visualization
        // Implementation depends on shader setup
    }
    
    void UpdateBurnTexture(Vector2 chunkCoord, Pixel pixel)
    {
        // Update material texture for burned area visualization
        // Implementation depends on shader setup
    }
}
```

**Key Implementation Details:**
- Manage terrain tiles in a dictionary
- Update visible chunks based on viewer position
- Support LOD system for performance
- Handle pixel updates (elevation, fuel code, fire, water, burn)
- Generate terrain on demand as player moves

### 2.3 Terrain Editor Tools

Create editor tools for painting elevation and fuel codes with real-time updates.

**Files to create:**

- `Scripts/Editor/TerrainEditor.cs` - Main terrain editor window
- `Scripts/Editor/TerrainPaintTool.cs` - Painting tool
- `Scripts/Editor/TerrainBrush.cs` - Brush settings and logic

**Detailed Implementation Steps:**

#### Step 1: Create TerrainEditor Window

**File: `Scripts/Editor/TerrainEditor.cs`**

```csharp
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Terrain editor window for painting elevation and fuel codes
/// </summary>
public class TerrainEditor : EditorWindow
{
    [MenuItem("Tools/Terrain Editor")]
    public static void ShowWindow()
    {
        GetWindow<TerrainEditor>("Terrain Editor");
    }
    
    enum PaintMode { Elevation, FuelCode }
    PaintMode paintMode = PaintMode.Elevation;
    
    float brushSize = 10f;
    float brushStrength = 1f;
    AnimationCurve brushFalloff = AnimationCurve.EaseInOut(0, 1, 1, 0);
    
    short targetFuelCode = 98;
    float targetElevation = 0f;
    
    bool isPainting = false;
    Vector2Int lastPaintPosition;
    
    void OnGUI()
    {
        GUILayout.Label("Terrain Paint Tool", EditorStyles.boldLabel);
        
        paintMode = (PaintMode)EditorGUILayout.EnumPopup("Paint Mode", paintMode);
        
        brushSize = EditorGUILayout.Slider("Brush Size", brushSize, 1f, 50f);
        brushStrength = EditorGUILayout.Slider("Brush Strength", brushStrength, 0.1f, 2f);
        brushFalloff = EditorGUILayout.CurveField("Brush Falloff", brushFalloff);
        
        if (paintMode == PaintMode.Elevation)
        {
            targetElevation = EditorGUILayout.FloatField("Target Elevation", targetElevation);
        }
        else
        {
            targetFuelCode = (short)EditorGUILayout.IntField("Target Fuel Code", targetFuelCode);
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Reset Map"))
        {
            if (EditorUtility.DisplayDialog("Reset Map", "Are you sure you want to reset the entire map?", "Yes", "No"))
            {
                GameManager.instance.mapData.ResetMap();
            }
        }
    }
    
    void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;
        
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            isPainting = true;
            PaintTerrain(e.mousePosition);
        }
        else if (e.type == EventType.MouseDrag && e.button == 0 && isPainting)
        {
            PaintTerrain(e.mousePosition);
        }
        else if (e.type == EventType.MouseUp && e.button == 0)
        {
            isPainting = false;
        }
        
        // Draw brush preview
        DrawBrushPreview(e.mousePosition);
    }
    
    void PaintTerrain(Vector2 mousePosition)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
        {
            Vector3 worldPos = hit.point;
            Vector2Int pixelCoord = new Vector2Int(
                Mathf.FloorToInt(worldPos.x),
                Mathf.FloorToInt(-worldPos.z)
            );
            
            if (pixelCoord != lastPaintPosition)
            {
                PaintAtPosition(pixelCoord, worldPos);
                lastPaintPosition = pixelCoord;
            }
        }
    }
    
    void PaintAtPosition(Vector2Int center, Vector3 worldPos)
    {
        int radius = Mathf.RoundToInt(brushSize);
        GameManager gameManager = GameManager.instance;
        
        for (int x = -radius; x <= radius; x++)
        {
            for (int z = -radius; z <= radius; z++)
            {
                Vector2Int pixelCoord = center + new Vector2Int(x, z);
                float distance = Vector2.Distance(center, pixelCoord);
                
                if (distance <= radius)
                {
                    float falloff = brushFalloff.Evaluate(distance / radius);
                    float strength = brushStrength * falloff;
                    
                    Pixel pixel = gameManager.mapData.GetPixel(pixelCoord.x, pixelCoord.z);
                    if (pixel != null)
                    {
                        if (paintMode == PaintMode.Elevation)
                        {
                            short currentElevation = pixel.elevation;
                            short newElevation = (short)Mathf.Lerp(currentElevation, targetElevation, strength);
                            pixel.elevation = newElevation;
                        }
                        else
                        {
                            pixel.fuelCode = targetFuelCode;
                        }
                    }
                }
            }
        }
        
        // Mark map as dirty for saving
        gameManager.mapData.MarkDirty();
    }
    
    void DrawBrushPreview(Vector2 mousePosition)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
        {
            Handles.color = new Color(1, 1, 0, 0.3f);
            Handles.DrawWireDisc(hit.point, Vector3.up, brushSize);
        }
    }
}
```

**Key Implementation Details:**
- Editor window for terrain painting
- Support elevation and fuel code painting modes
- Configurable brush size, strength, and falloff
- Real-time preview of brush
- Paint on mouse drag
- Update pixels and trigger terrain updates

This is getting very long. Should I continue with the remaining phases (Fuel Code System, Unit System, LinePath System, etc.) with the same level of detail?

## Phase 3: Fuel Code System

### 3.1 Fuel Code Data Structure

Based on old project's `fuelcodes_export.json` structure. Each fuel code has bezier curves for ROS (Rate of Spread), flame length, and slope, with 4 moisture states (VeryLow, Low, Medium, High).

**Files to create:**

- `Scripts/FuelCodes/FuelCodeData.cs` - ScriptableObject for fuel code data
- `Scripts/FuelCodes/FuelCodeSet.cs` - Collection of fuel codes
- `Scripts/FuelCodes/BezierCurve.cs` - Bezier curve data structure
- `Scripts/FuelCodes/MoistureState.cs` - Moisture state enum

**Detailed Implementation Steps:**

#### Step 1: Create MoistureState Enum

**File: `Scripts/FuelCodes/MoistureState.cs`**

```csharp
/// <summary>
/// Moisture state for fuel code curves
/// Based on old project's moisture states
/// </summary>
public enum MoistureState
{
    VeryLow = 0,  // Very low moisture (driest conditions)
    Low = 1,      // Low moisture
    Medium = 2,   // Medium moisture
    High = 3      // High moisture (wettest conditions)
}
```

#### Step 2: Create BezierCurve Data Structure

**File: `Scripts/FuelCodes/BezierCurve.cs`**

Based on old project's curve structure with x1, y1, x2, y2, min, max values.

```csharp
using UnityEngine;
using System;

/// <summary>
/// Represents a 4-node bezier curve for fuel code behavior
/// Based on old project's curve structure
/// </summary>
[Serializable]
public class BezierCurve
{
    [Header("Control Points")]
    public float x1 = 0f;  // First control point X
    public float y1 = 0f;   // First control point Y
    public float x2 = 0f;  // Second control point X
    public float y2 = 0f;  // Second control point Y
    
    [Header("Value Range")]
    public float min = 0f;  // Minimum output value
    public float max = 100f; // Maximum output value
    
    /// <summary>
    /// Evaluate bezier curve at parameter t (0 to 1)
    /// Uses cubic bezier formula: B(t) = (1-t)³P₀ + 3(1-t)²tP₁ + 3(1-t)t²P₂ + t³P₃
    /// Where P₀ = (0, min), P₁ = (x1, y1), P₂ = (x2, y2), P₃ = (max, max)
    /// </summary>
    public float Evaluate(float t)
    {
        t = Mathf.Clamp01(t);
        
        // Normalize input to 0-1 range based on min/max
        // For ROS/FlameLength: input is wind speed, output is ROS/flame length
        // For Slope: input is slope angle, output is slope factor
        
        // Cubic bezier interpolation
        float u = 1f - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;
        
        // Control points in normalized space
        Vector2 p0 = new Vector2(0f, min);
        Vector2 p1 = new Vector2(x1, y1);
        Vector2 p2 = new Vector2(x2, y2);
        Vector2 p3 = new Vector2(max, max);
        
        // Bezier curve calculation
        Vector2 result = uuu * p0 + 3f * uu * t * p1 + 3f * u * tt * p2 + ttt * p3;
        
        return Mathf.Clamp(result.y, min, max);
    }
    
    /// <summary>
    /// Evaluate curve with input value (e.g., wind speed)
    /// Maps input to 0-1 range based on expected input range
    /// </summary>
    public float EvaluateWithInput(float input, float inputMin = 0f, float inputMax = 100f)
    {
        float normalizedInput = Mathf.InverseLerp(inputMin, inputMax, input);
        return Evaluate(normalizedInput);
    }
    
    /// <summary>
    /// Create curve from old project format
    /// </summary>
    public static BezierCurve FromOldFormat(float x1, float y1, float x2, float y2, float min, float max)
    {
        BezierCurve curve = new BezierCurve();
        curve.x1 = x1;
        curve.y1 = y1;
        curve.x2 = x2;
        curve.y2 = y2;
        curve.min = min;
        curve.max = max;
        return curve;
    }
}
```

**Key Implementation Details:**
- Use cubic bezier formula for smooth curves
- Support min/max clamping
- Map input values (wind speed, slope) to 0-1 range
- Match old project's curve format exactly

#### Step 3: Create FuelCodeData ScriptableObject

**File: `Scripts/FuelCodes/FuelCodeData.cs`**

Based on old project's `fuelcodes_export.json` structure.

```csharp
using UnityEngine;
using System;

/// <summary>
/// Fuel code data ScriptableObject
/// Based on old project's fuelcodes_export.json structure
/// Each fuel code has curves for ROS, flame length, and slope at 4 moisture states
/// </summary>
[CreateAssetMenu(fileName = "FuelCode", menuName = "Fuel Codes/Fuel Code Data")]
public class FuelCodeData : ScriptableObject
{
    [Header("Fuel Code Info")]
    public string title;           // Fuel code name (e.g., "GR1", "GR2")
    public string codeGIS;         // GIS code (e.g., "101", "102")
    public short fuelCodeID;       // Numeric ID matching GIS color codes
    
    [Header("Fuel Load (tons/acre)")]
    public float hour1 = 0f;       // 1-hour fuel load
    public float hour10 = 0f;      // 10-hour fuel load
    public float hour100 = 0f;     // 100-hour fuel load
    
    [Header("ROS Curves (Rate of Spread)")]
    public BezierCurve rosVeryLow;  // ROS at very low moisture
    public BezierCurve rosLow;      // ROS at low moisture
    public BezierCurve rosMedium;   // ROS at medium moisture
    public BezierCurve rosHigh;     // ROS at high moisture
    
    [Header("Flame Length Curves (meters)")]
    public BezierCurve flameVeryLow;
    public BezierCurve flameLow;
    public BezierCurve flameMedium;
    public BezierCurve flameHigh;
    
    [Header("Slope Curves (degrees)")]
    public BezierCurve slopeVeryLow;
    public BezierCurve slopeLow;
    public BezierCurve slopeMedium;
    public BezierCurve slopeHigh;
    
    /// <summary>
    /// Get ROS curve for specific moisture state
    /// </summary>
    public BezierCurve GetROSCurve(MoistureState moisture)
    {
        switch (moisture)
        {
            case MoistureState.VeryLow: return rosVeryLow;
            case MoistureState.Low: return rosLow;
            case MoistureState.Medium: return rosMedium;
            case MoistureState.High: return rosHigh;
            default: return rosMedium;
        }
    }
    
    /// <summary>
    /// Get flame length curve for specific moisture state
    /// </summary>
    public BezierCurve GetFlameLengthCurve(MoistureState moisture)
    {
        switch (moisture)
        {
            case MoistureState.VeryLow: return flameVeryLow;
            case MoistureState.Low: return flameLow;
            case MoistureState.Medium: return flameMedium;
            case MoistureState.High: return flameHigh;
            default: return flameMedium;
        }
    }
    
    /// <summary>
    /// Get slope curve for specific moisture state
    /// </summary>
    public BezierCurve GetSlopeCurve(MoistureState moisture)
    {
        switch (moisture)
        {
            case MoistureState.VeryLow: return slopeVeryLow;
            case MoistureState.Low: return slopeLow;
            case MoistureState.Medium: return slopeMedium;
            case MoistureState.High: return slopeHigh;
            default: return slopeMedium;
        }
    }
    
    /// <summary>
    /// Calculate ROS (Rate of Spread) based on wind speed and moisture
    /// Based on old project's GetROS() method
    /// </summary>
    public float CalculateROS(float windSpeed, MoistureState moisture)
    {
        BezierCurve curve = GetROSCurve(moisture);
        // Wind speed typically ranges from 0-50 mph or 0-80 km/h
        // Adjust input range based on your units
        return curve.EvaluateWithInput(windSpeed, 0f, 50f);
    }
    
    /// <summary>
    /// Calculate flame length based on wind speed and moisture
    /// Based on old project's GetFlameLength() method
    /// </summary>
    public float CalculateFlameLength(float windSpeed, MoistureState moisture)
    {
        BezierCurve curve = GetFlameLengthCurve(moisture);
        return curve.EvaluateWithInput(windSpeed, 0f, 50f);
    }
    
    /// <summary>
    /// Calculate slope factor based on slope angle and moisture
    /// </summary>
    public float CalculateSlopeFactor(float slopeAngle, MoistureState moisture)
    {
        BezierCurve curve = GetSlopeCurve(moisture);
        // Slope angle typically ranges from 0-90 degrees
        return curve.EvaluateWithInput(slopeAngle, 0f, 90f);
    }
    
    /// <summary>
    /// Load fuel code from old project JSON format
    /// </summary>
    public void LoadFromJSON(JSONObject json)
    {
        title = json.GetField("title").str;
        codeGIS = json.GetField("codeGIS").str;
        fuelCodeID = (short)int.Parse(codeGIS);
        
        hour1 = json.GetField("hour1").f;
        hour10 = json.GetField("hour10").f;
        hour100 = json.GetField("hour100").f;
        
        // Load ROS curves
        rosVeryLow = LoadCurveFromJSON(json.GetField("rosVeryLow"));
        rosLow = LoadCurveFromJSON(json.GetField("rosLow"));
        rosMedium = LoadCurveFromJSON(json.GetField("rosMedium"));
        rosHigh = LoadCurveFromJSON(json.GetField("rosHigh"));
        
        // Load flame length curves
        flameVeryLow = LoadCurveFromJSON(json.GetField("flameVeryLow"));
        flameLow = LoadCurveFromJSON(json.GetField("flameLow"));
        flameMedium = LoadCurveFromJSON(json.GetField("flameMedium"));
        flameHigh = LoadCurveFromJSON(json.GetField("flameHigh"));
        
        // Load slope curves
        slopeVeryLow = LoadCurveFromJSON(json.GetField("slopeVeryLow"));
        slopeLow = LoadCurveFromJSON(json.GetField("slopeLow"));
        slopeMedium = LoadCurveFromJSON(json.GetField("slopeMedium"));
        slopeHigh = LoadCurveFromJSON(json.GetField("slopeHigh"));
    }
    
    BezierCurve LoadCurveFromJSON(JSONObject json)
    {
        return BezierCurve.FromOldFormat(
            json.GetField("x1").f,
            json.GetField("y1").f,
            json.GetField("x2").f,
            json.GetField("y2").f,
            json.GetField("min").f,
            json.GetField("max").f
        );
    }
}
```

**Key Implementation Details:**
- Match old project's JSON structure exactly
- Support 4 moisture states for each curve type
- Provide convenience methods for calculations
- Support loading from old project's JSON format

#### Step 4: Create FuelCodeSet

**File: `Scripts/FuelCodes/FuelCodeSet.cs`**

```csharp
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Collection of fuel codes
/// Manages all fuel codes in the game
/// </summary>
[CreateAssetMenu(fileName = "FuelCodeSet", menuName = "Fuel Codes/Fuel Code Set")]
public class FuelCodeSet : ScriptableObject
{
    public List<FuelCodeData> fuelCodes = new List<FuelCodeData>();
    
    /// <summary>
    /// Get fuel code by ID
    /// </summary>
    public FuelCodeData GetFuelCode(short fuelCodeID)
    {
        return fuelCodes.FirstOrDefault(fc => fc.fuelCodeID == fuelCodeID);
    }
    
    /// <summary>
    /// Get fuel code by GIS code string
    /// </summary>
    public FuelCodeData GetFuelCodeByGISCode(string codeGIS)
    {
        return fuelCodes.FirstOrDefault(fc => fc.codeGIS == codeGIS);
    }
    
    /// <summary>
    /// Load all fuel codes from old project's JSON file
    /// </summary>
    public void LoadFromJSON(string jsonText)
    {
        JSONObject json = new JSONObject(jsonText);
        JSONObject items = json.GetField("items");
        
        fuelCodes.Clear();
        
        for (int i = 0; i < items.list.Count; i++)
        {
            FuelCodeData fuelCode = ScriptableObject.CreateInstance<FuelCodeData>();
            fuelCode.LoadFromJSON(items.list[i]);
            fuelCodes.Add(fuelCode);
        }
        
        Debug.Log($"Loaded {fuelCodes.Count} fuel codes from JSON");
    }
}
```

### 3.2 Graph Editor System

Custom Unity Editor window for creating and editing bezier curves visually.

**Files to create:**

- `Scripts/FuelCodes/Editor/FuelCodeEditor.cs` - Custom inspector for FuelCodeData
- `Scripts/FuelCodes/Editor/GraphEditor.cs` - Bezier curve editor window
- `Scripts/FuelCodes/Editor/BezierCurveEditor.cs` - Bezier curve property drawer

**Detailed Implementation Steps:**

#### Step 1: Create GraphEditor Window

**File: `Scripts/FuelCodes/Editor/GraphEditor.cs`**

```csharp
using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// Custom editor window for editing bezier curves
/// Similar to Unity's AnimationCurve editor but for 4-node bezier curves
/// </summary>
public class GraphEditor : EditorWindow
{
    BezierCurve curve;
    string curveName;
    Action<BezierCurve> onCurveChanged;
    
    Rect graphRect;
    Vector2 scrollPosition;
    float graphWidth = 400f;
    float graphHeight = 300f;
    
    bool isDragging = false;
    int selectedHandle = -1;
    
    Vector2 p0, p1, p2, p3; // Bezier control points in graph space
    
    public static void ShowWindow(BezierCurve curve, string name, Action<BezierCurve> onChanged)
    {
        GraphEditor window = GetWindow<GraphEditor>("Bezier Curve Editor");
        window.curve = curve;
        window.curveName = name;
        window.onCurveChanged = onChanged;
        window.Initialize();
    }
    
    void Initialize()
    {
        // Convert curve data to graph space
        p0 = new Vector2(0f, curve.min);
        p1 = new Vector2(curve.x1, curve.y1);
        p2 = new Vector2(curve.x2, curve.y2);
        p3 = new Vector2(curve.max, curve.max);
    }
    
    void OnGUI()
    {
        if (curve == null) return;
        
        EditorGUILayout.LabelField(curveName, EditorStyles.boldLabel);
        
        // Curve properties
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Min:", GUILayout.Width(50));
        curve.min = EditorGUILayout.FloatField(curve.min);
        EditorGUILayout.LabelField("Max:", GUILayout.Width(50));
        curve.max = EditorGUILayout.FloatField(curve.max);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Graph area
        graphRect = GUILayoutUtility.GetRect(graphWidth, graphHeight);
        DrawGraph();
        
        EditorGUILayout.Space();
        
        // Control point inputs
        EditorGUILayout.LabelField("Control Points", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("P1:", GUILayout.Width(30));
        curve.x1 = EditorGUILayout.FloatField(curve.x1);
        curve.y1 = EditorGUILayout.FloatField(curve.y1);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("P2:", GUILayout.Width(30));
        curve.x2 = EditorGUILayout.FloatField(curve.x2);
        curve.y2 = EditorGUILayout.FloatField(curve.y2);
        EditorGUILayout.EndHorizontal();
        
        if (GUI.changed)
        {
            Initialize();
            if (onCurveChanged != null)
            {
                onCurveChanged(curve);
            }
        }
    }
    
    void DrawGraph()
    {
        // Draw background
        EditorGUI.DrawRect(graphRect, new Color(0.2f, 0.2f, 0.2f));
        
        // Draw grid
        DrawGrid();
        
        // Draw bezier curve
        DrawBezierCurve();
        
        // Draw control points
        DrawControlPoints();
        
        // Handle mouse input
        HandleMouseInput();
    }
    
    void DrawGrid()
    {
        Handles.BeginGUI();
        Handles.color = new Color(0.3f, 0.3f, 0.3f);
        
        // Vertical lines
        for (int i = 0; i <= 10; i++)
        {
            float x = graphRect.x + (graphRect.width / 10f) * i;
            Handles.DrawLine(new Vector3(x, graphRect.y), new Vector3(x, graphRect.y + graphRect.height));
        }
        
        // Horizontal lines
        for (int i = 0; i <= 10; i++)
        {
            float y = graphRect.y + (graphRect.height / 10f) * i;
            Handles.DrawLine(new Vector3(graphRect.x, y), new Vector3(graphRect.x + graphRect.width, y));
        }
        
        Handles.EndGUI();
    }
    
    void DrawBezierCurve()
    {
        Handles.BeginGUI();
        Handles.color = Color.green;
        
        Vector3[] points = new Vector3[100];
        for (int i = 0; i <= 99; i++)
        {
            float t = i / 99f;
            Vector2 point = EvaluateBezier(t);
            points[i] = GraphToScreen(point);
        }
        
        Handles.DrawPolyLine(points);
        Handles.EndGUI();
    }
    
    void DrawControlPoints()
    {
        Handles.BeginGUI();
        
        // Draw P0 and P3 (fixed endpoints)
        Handles.color = Color.yellow;
        DrawHandle(GraphToScreen(p0), 8f, 0);
        DrawHandle(GraphToScreen(p3), 8f, 3);
        
        // Draw P1 and P2 (control points)
        Handles.color = Color.cyan;
        DrawHandle(GraphToScreen(p1), 10f, 1);
        DrawHandle(GraphToScreen(p2), 10f, 2);
        
        // Draw lines from endpoints to control points
        Handles.color = new Color(0.5f, 0.5f, 0.5f);
        Handles.DrawLine(GraphToScreen(p0), GraphToScreen(p1));
        Handles.DrawLine(GraphToScreen(p3), GraphToScreen(p2));
        
        Handles.EndGUI();
    }
    
    void DrawHandle(Vector3 screenPos, float size, int index)
    {
        Rect rect = new Rect(screenPos.x - size / 2f, screenPos.y - size / 2f, size, size);
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        
        if (selectedHandle == index)
        {
            Handles.color = Color.red;
            Handles.DrawWireDisc(screenPos, Vector3.forward, size / 2f + 2f);
        }
    }
    
    void HandleMouseInput()
    {
        Event e = Event.current;
        
        if (e.type == EventType.MouseDown && graphRect.Contains(e.mousePosition))
        {
            // Check which handle was clicked
            for (int i = 0; i < 4; i++)
            {
                Vector2 point = i == 0 ? p0 : (i == 1 ? p1 : (i == 2 ? p2 : p3));
                Vector3 screenPos = GraphToScreen(point);
                float dist = Vector2.Distance(e.mousePosition, screenPos);
                
                if (dist < 15f)
                {
                    selectedHandle = i;
                    isDragging = true;
                    e.Use();
                    break;
                }
            }
        }
        else if (e.type == EventType.MouseDrag && isDragging && selectedHandle >= 0)
        {
            Vector2 graphPos = ScreenToGraph(e.mousePosition);
            
            // Update selected control point
            if (selectedHandle == 1)
            {
                p1 = graphPos;
                curve.x1 = p1.x;
                curve.y1 = p1.y;
            }
            else if (selectedHandle == 2)
            {
                p2 = graphPos;
                curve.x2 = p2.x;
                curve.y2 = p2.y;
            }
            
            if (onCurveChanged != null)
            {
                onCurveChanged(curve);
            }
            
            Repaint();
            e.Use();
        }
        else if (e.type == EventType.MouseUp)
        {
            isDragging = false;
            selectedHandle = -1;
        }
    }
    
    Vector2 EvaluateBezier(float t)
    {
        float u = 1f - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;
        
        return uuu * p0 + 3f * uu * t * p1 + 3f * u * tt * p2 + ttt * p3;
    }
    
    Vector3 GraphToScreen(Vector2 graphPos)
    {
        float x = graphRect.x + (graphPos.x / curve.max) * graphRect.width;
        float y = graphRect.y + graphRect.height - (graphPos.y / curve.max) * graphRect.height;
        return new Vector3(x, y, 0);
    }
    
    Vector2 ScreenToGraph(Vector2 screenPos)
    {
        float x = ((screenPos.x - graphRect.x) / graphRect.width) * curve.max;
        float y = ((graphRect.y + graphRect.height - screenPos.y) / graphRect.height) * curve.max;
        return new Vector2(x, y);
    }
}
```

**Key Implementation Details:**
- Visual bezier curve editor similar to Unity's AnimationCurve
- Drag control points to edit curves
- Real-time preview of curve
- Support min/max value clamping
- Interactive graph with grid

#### Step 2: Create FuelCodeEditor Custom Inspector

**File: `Scripts/FuelCodes/Editor/FuelCodeEditor.cs`**

```csharp
using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom inspector for FuelCodeData
/// Provides buttons to open graph editors for each curve
/// </summary>
[CustomEditor(typeof(FuelCodeData))]
public class FuelCodeEditor : Editor
{
    FuelCodeData fuelCode;
    
    void OnEnable()
    {
        fuelCode = (FuelCodeData)target;
    }
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Curve Editors", EditorStyles.boldLabel);
        
        // ROS Curves
        EditorGUILayout.LabelField("ROS Curves", EditorStyles.miniLabel);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Edit VeryLow")) GraphEditor.ShowWindow(fuelCode.rosVeryLow, "ROS VeryLow", OnCurveChanged);
        if (GUILayout.Button("Edit Low")) GraphEditor.ShowWindow(fuelCode.rosLow, "ROS Low", OnCurveChanged);
        if (GUILayout.Button("Edit Medium")) GraphEditor.ShowWindow(fuelCode.rosMedium, "ROS Medium", OnCurveChanged);
        if (GUILayout.Button("Edit High")) GraphEditor.ShowWindow(fuelCode.rosHigh, "ROS High", OnCurveChanged);
        EditorGUILayout.EndHorizontal();
        
        // Flame Length Curves
        EditorGUILayout.LabelField("Flame Length Curves", EditorStyles.miniLabel);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Edit VeryLow")) GraphEditor.ShowWindow(fuelCode.flameVeryLow, "Flame VeryLow", OnCurveChanged);
        if (GUILayout.Button("Edit Low")) GraphEditor.ShowWindow(fuelCode.flameLow, "Flame Low", OnCurveChanged);
        if (GUILayout.Button("Edit Medium")) GraphEditor.ShowWindow(fuelCode.flameMedium, "Flame Medium", OnCurveChanged);
        if (GUILayout.Button("Edit High")) GraphEditor.ShowWindow(fuelCode.flameHigh, "Flame High", OnCurveChanged);
        EditorGUILayout.EndHorizontal();
        
        // Slope Curves
        EditorGUILayout.LabelField("Slope Curves", EditorStyles.miniLabel);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Edit VeryLow")) GraphEditor.ShowWindow(fuelCode.slopeVeryLow, "Slope VeryLow", OnCurveChanged);
        if (GUILayout.Button("Edit Low")) GraphEditor.ShowWindow(fuelCode.slopeLow, "Slope Low", OnCurveChanged);
        if (GUILayout.Button("Edit Medium")) GraphEditor.ShowWindow(fuelCode.slopeMedium, "Slope Medium", OnCurveChanged);
        if (GUILayout.Button("Edit High")) GraphEditor.ShowWindow(fuelCode.slopeHigh, "Slope High", OnCurveChanged);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Test curve evaluation
        EditorGUILayout.LabelField("Test Evaluation", EditorStyles.boldLabel);
        float windSpeed = EditorGUILayout.Slider("Wind Speed", 10f, 0f, 50f);
        MoistureState moisture = (MoistureState)EditorGUILayout.EnumPopup("Moisture", MoistureState.Medium);
        
        float ros = fuelCode.CalculateROS(windSpeed, moisture);
        float flameLength = fuelCode.CalculateFlameLength(windSpeed, moisture);
        
        EditorGUILayout.LabelField($"ROS: {ros:F2}");
        EditorGUILayout.LabelField($"Flame Length: {flameLength:F2}m");
    }
    
    void OnCurveChanged(BezierCurve curve)
    {
        EditorUtility.SetDirty(fuelCode);
    }
}
```

### 3.3 Fuel Code Runtime System

Integration with GameManager for runtime fire behavior calculations.

**Files to create:**

- `Scripts/Core/FuelCodeManager.cs` - Manages fuel code lookups and calculations

**Detailed Implementation Steps:**

#### Step 1: Create FuelCodeManager

**File: `Scripts/Core/FuelCodeManager.cs`**

```csharp
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages fuel code lookups and calculations at runtime
/// Based on old project's fuel code system
/// </summary>
public class FuelCodeManager : MonoBehaviour
{
    [Header("Fuel Code Data")]
    public FuelCodeSet fuelCodeSet;
    
    // Cache for fast lookups
    private Dictionary<short, FuelCodeData> fuelCodeCache = new Dictionary<short, FuelCodeData>();
    
    void Awake()
    {
        BuildCache();
    }
    
    void BuildCache()
    {
        fuelCodeCache.Clear();
        if (fuelCodeSet != null)
        {
            foreach (FuelCodeData fuelCode in fuelCodeSet.fuelCodes)
            {
                fuelCodeCache[fuelCode.fuelCodeID] = fuelCode;
            }
        }
    }
    
    /// <summary>
    /// Get ROS (Rate of Spread) for a fuel code
    /// Based on old project's GetROS() method
    /// </summary>
    public float GetROS(short fuelCodeID, float windSpeed, MoistureState moisture = MoistureState.Medium)
    {
        if (fuelCodeCache.TryGetValue(fuelCodeID, out FuelCodeData fuelCode))
        {
            return fuelCode.CalculateROS(windSpeed, moisture);
        }
        return 0f;
    }
    
    /// <summary>
    /// Get flame length for a fuel code
    /// Based on old project's GetFlameLength() method
    /// </summary>
    public float GetFlameLength(short fuelCodeID, float windSpeed, MoistureState moisture = MoistureState.Medium)
    {
        if (fuelCodeCache.TryGetValue(fuelCodeID, out FuelCodeData fuelCode))
        {
            return fuelCode.CalculateFlameLength(windSpeed, moisture);
        }
        return 0f;
    }
    
    /// <summary>
    /// Get slope factor for a fuel code
    /// </summary>
    public float GetSlopeFactor(short fuelCodeID, float slopeAngle, MoistureState moisture = MoistureState.Medium)
    {
        if (fuelCodeCache.TryGetValue(fuelCodeID, out FuelCodeData fuelCode))
        {
            return fuelCode.CalculateSlopeFactor(slopeAngle, moisture);
        }
        return 1f;
    }
    
    /// <summary>
    /// Get fuel code data by ID
    /// </summary>
    public FuelCodeData GetFuelCodeData(short fuelCodeID)
    {
        fuelCodeCache.TryGetValue(fuelCodeID, out FuelCodeData fuelCode);
        return fuelCode;
    }
}
```

**Key Implementation Details:**
- Cache fuel codes for fast runtime lookups
- Provide methods matching old project's API
- Support moisture state selection
- Integrate with GameManager for fire behavior calculations

**Integration with GameManager:**

Add to `GameManager.cs`:

```csharp
public FuelCodeManager fuelCodeManager;

public float GetROS(short fuelCode, float windSpeed)
{
    return fuelCodeManager.GetROS(fuelCode, windSpeed, moistureState);
}

public float GetFlameLength(short fuelCode, float windSpeed)
{
    return fuelCodeManager.GetFlameLength(fuelCode, windSpeed, moistureState);
}
```

This matches the old project's API and allows pixels to calculate ROS and flame length based on fuel code, wind speed, and moisture state.

## Phase 4: Unit System

### 4.1 Unit Data Structure

**Files to create:**

- `Scripts/Units/UnitData.cs` - ScriptableObject for unit definitions
- `Scripts/Units/UnitProperty.cs` - Unit property definitions
- `Scripts/Units/UnitLevel.cs` - Level data structure

**Unit Types (from requirements + old project):**

- Ground Crew
- Cut Crew  
- Hose Crew
- Brush Truck
- Engine
- Water Tanker
- Hotshots
- Dozers
- Helicopters
- Airplanes

**Unit Properties:**

- Movement speed (min/max)
- Water capacity
- Water in/out flow speed
- Cut speed
- Dig speed
- Hitpoints (HP)
- Energy/fuel levels
- Flying properties (for aircraft)

### 4.2 Unit Leveling System

**Files to create:**

- `Scripts/Units/UnitLeveling.cs` - Leveling logic
- `Scripts/Units/FirePoints.cs` - Experience point system

**Key Features:**

- Units earn "firepoints" by extinguishing burning pixels
- Firepoints can be used to:
  - Level up unit properties
  - Unlock new unit types
- Property-based unlock system:
  - Upgrade water properties → unlock Water Crew
  - Upgrade cutting properties → unlock Cut Crew, Dozers
  - Upgrade suppression properties → unlock Engine, Brush Truck
- Each level matches property level requirements

### 4.3 Unit Controller & Automation

Based on old project's `UnitController.cs` and `UnitLinepath.cs`:

**Files to create:**

- `Scripts/Units/UnitController.cs` - Main unit controller
- `Scripts/Units/UnitActions.cs` - Unit action system
- `Scripts/Units/UnitSuppression.cs` - Fire suppression logic

**Key Features:**

- Automated firefighting behavior
- Unit follows LinePaths automatically
- Resource management (water, fuel)
- Support unit mechanics (tankers resupply other units)

## Phase 5: LinePath System

### 5.1 LinePath Architecture

Based on old project's `LinePath.cs`, `LinePathPoint.cs`, `LinePathManager.cs`:

**Files to create:**

- `Scripts/LinePaths/LinePath.cs` - Line path container
- `Scripts/LinePaths/LinePathPoint.cs` - Individual path point
- `Scripts/LinePaths/LinePathManager.cs` - Manages all paths
- `Scripts/LinePaths/LinePathType.cs` - Path type enum

**LinePath Types:**

- Road
- OffRoad
- CutLine
- DozerLine
- HandLine
- HoserLine

### 5.2 LinePathPoint Indicators

Based on old project's indicator system:

**Indicator Types:**

- Water (near water pixels)
- Urban (near urban pixels)
- Fire (near fire pixels)
- Cut (for cutting operations)
- Dozer (for dozing operations)
- Point (near other LinePathPoints)

**Key Features:**

- Points detect nearby conditions (fire, water, urban, etc.)
- Indicators show/hide based on conditions
- Units automatically respond to indicator states
- Pathfinding between LinePaths and points

### 5.3 Pathfinding System

**Files to create:**

- `Scripts/LinePaths/Pathfinder.cs` - Pathfinding algorithm
- `Scripts/LinePaths/PathNode.cs` - Pathfinding graph node

**Key Features:**

- Shortest path calculation between LinePathPoints
- Support for intersecting LinePaths
- Path type permissions (units can only use certain path types)
- Dynamic path updates when conditions change

## Phase 6: Multiplayer Integration

### 6.1 Unity Netcode Setup

Replace PlayerIO with Unity Netcode for GameObjects:

**Files to create:**

- `Scripts/Core/NetworkManager.cs` - Netcode manager
- `Scripts/Network/NetworkTerrain.cs` - Networked terrain tiles
- `Scripts/Network/NetworkUnit.cs` - Networked unit component
- `Scripts/Network/NetworkLinePath.cs` - Networked LinePath component

**Key Features:**

- Client-server architecture
- Synchronize terrain changes
- Synchronize unit positions and states
- Synchronize LinePath creation/updates
- Use Unity's Relay service for connectivity

### 6.2 Database Integration

For GIS data storage (replacing PlayerIO BigDB):

**Options:**

- Unity Cloud Build with Cloud Save
- Firebase Realtime Database
- Custom backend API
- Addressables for static GIS data

## Phase 7: Mobile Optimization

### 7.1 Performance Optimizations

- LOD system for terrain tiles
- Reduced polygon counts for mobile
- Texture compression
- Occlusion culling
- Frustum culling
- Object pooling for units/effects

### 7.2 Mobile-Specific Features

- Touch input system
- Mobile UI/UX adjustments
- Battery optimization
- Reduced particle effects
- Lower quality graphics presets

## Implementation Order

1. **Phase 1** - Project setup and core infrastructure
2. **Phase 2** - GIS terrain system (foundation for everything)
3. **Phase 3** - Fuel code system (needed for fire behavior)
4. **Phase 4** - Unit system (core gameplay)
5. **Phase 5** - LinePath system (unit automation)
6. **Phase 6** - Multiplayer (network the systems)
7. **Phase 7** - Mobile optimization (polish for all platforms)

## Key Files to Reference from Old Project

- `Assets/MapData.cs` - Map data structure (adapt for new system)
- `Assets/Pixel.cs` - Pixel data structure (adapt)
- `Assets/Terrain/Scripts/TerrainGenerator.cs` - Terrain generation (reference)
- `Assets/Units/LinePath/LinePath.cs` - LinePath system (port with improvements)
- `Assets/Units/LinePath/LinePathPoint.cs` - LinePathPoint system (port)
- `Assets/Units/LinePath/UnitController.cs` - Unit automation (port)
- `Assets/Units/Unit.cs` - Unit behavior (reference)
- `fuelcodes_export.json` - Fuel code data structure (reference)

## Technical Considerations

1. **Scale**: Each pixel = 30m x 30m real world = 1m x 1m in-game
2. **Terrain Tiles**: Need efficient loading/unloading system
3. **Bezier Curves**: Implement custom bezier evaluation for fuel code graphs
4. **Network Replication**: Efficient synchronization of large terrain data
5. **Mobile Performance**: Aggressive optimization needed for large maps
6. **GIS Data Format**: Support GeoTiff (may need GDAL library) and PNG

## Additional Implementation Notes for Remaining Phases

### Phase 4: Unit System - Key Implementation Details

**Based on old project's `Unit.cs` and `UnitData.cs`:**

1. **UnitData Structure:**
   - Use ScriptableObject for unit definitions
   - Store properties as Dictionary<string, float> for flexibility (matches old project's `unitData.Get("propertyName")` pattern)
   - Properties include: moveSpeed, waterCapacity, waterInputAmount, waterOutputAmount, suppressionSpeed, cutSpeed, effectiveRadius, maxSlope
   - Store terrainSpeedFactorDBO and cutSpeedFactorDBO as Dictionary<short, float> (fuel code -> speed factor)
   - Support unit types: groundcrew, aircraft, etc.

2. **Unit Class:**
   - Port from old project's `Unit.cs` with improvements
   - Track currentAction (MOVE, SPRAY, SUPPRESS, CUT, FILL)
   - Manage water capacity (currentCapacity, maxCapacity)
   - Track closestPixelOnFire, closestWaterSource, closestUrban
   - Calculate terrainSpeedFactor based on pixelUnderUnit.fuelCode
   - Support task force system (taskForceLeader, taskForceManager)
   - Handle unit stacking at LinePathPoints

3. **UnitController:**
   - Port from old project's `UnitController.cs`
   - State machine: SeekResource, Refilling, SeekFire, Suppressing, SeekCut, Cutting, SeekDozer, Dozing
   - Pathfinding between LinePathPoints using LinePathManager
   - Respond to indicator toggles (water, urban, fire, cut, dozer)
   - Support unit stacking with easing animations
   - Handle cluster distribution for fire indicators

4. **Unit Leveling:**
   - Units earn "firepoints" by extinguishing burning pixels
   - Firepoints stored per unit or globally
   - Property upgrades: increase moveSpeed, waterCapacity, etc.
   - Unlock system: upgrade water properties → unlock Water Crew, etc.

### Phase 5: LinePath System - Key Implementation Details

**Based on old project's `LinePath.cs`, `LinePathPoint.cs`, and `LinePathManager.cs`:**

1. **LinePath Class:**
   - Port from old project with improvements
   - Support path types: Road, OffRoad, CutLine, DozerLine, HandLine, HoserLine
   - Store points as List<Vector3>
   - Create LinePathPoint GameObjects for each point
   - Use LineRenderer for visualization
   - Support hover state (white color, wider line)
   - Toggle path type on click

2. **LinePathPoint Class:**
   - Port from old project's `LinePathPoint.cs`
   - Indicator system: Water, Urban, Fire, Cut, Dozer, Point
   - Indicator states: Off, Group, Focus (3-state system)
   - Detect nearby conditions (fire pixels, water pixels, urban pixels, other points)
   - Stack indicators vertically when multiple are active
   - Support clicking to toggle indicators
   - Cache nearby pixels for performance

3. **LinePathManager:**
   - Manage all LinePaths in the scene
   - Register/unregister LinePathPoints
   - Pathfinding: FindShortestPathRecursive() between points
   - Track active indicators by type and state
   - Events: IndicatorToggled, IndicatorStateToggled
   - Visualize paths for units
   - Support path type permissions for units

4. **Pathfinding:**
   - Graph-based pathfinding between LinePathPoints
   - Support intersecting LinePaths
   - Consider path type permissions
   - Cache paths for performance
   - Update paths when indicators change

### Phase 6: Network Integration - Key Implementation Details

**Replacing PlayerIO with Unity Netcode for GameObjects:**

1. **NetworkManager:**
   - Setup Unity Netcode for GameObjects
   - Configure client-server architecture
   - Use Unity Relay service for connectivity
   - Handle connection/disconnection
   - Manage network sessions

2. **Networked Components:**
   - NetworkTerrain: Sync terrain tile changes (elevation, fuel codes)
   - NetworkUnit: Sync unit positions, states, actions
   - NetworkLinePath: Sync LinePath creation/updates
   - NetworkPixel: Sync pixel fire states (optional, may be too much data)

3. **Data Synchronization:**
   - Use NetworkVariable for frequently changing data
   - Use RPCs for one-time events (unit actions, path creation)
   - Batch terrain updates to reduce network traffic
   - Use compression for large data (terrain tiles)

4. **Database Replacement:**
   - Replace PlayerIO BigDB with:
     - Unity Cloud Save (for player data)
     - Firebase Realtime Database (for shared map data)
     - Addressables (for static GIS data)
   - Migrate existing data from PlayerIO if needed

### Phase 7: Mobile Optimization - Key Implementation Details

1. **Performance Optimizations:**
   - Reduce terrain tile LOD levels for mobile
   - Use lower resolution textures
   - Reduce particle effects
   - Object pooling for units, water droplets, effects
   - Occlusion culling for terrain tiles
   - Frustum culling (already implemented in TerrainGenerator)

2. **Mobile-Specific Features:**
   - Touch input system (replace mouse with touch)
   - Mobile UI/UX (larger buttons, touch-friendly)
   - Battery optimization (reduce update frequency when possible)
   - Quality presets (already configured in Phase 1)
   - Reduced shadow quality
   - Lower texture resolution

3. **Platform-Specific Settings:**
   - Android: Vulkan or OpenGL ES 3.0, API Level 24+
   - iOS: Metal, iOS 13+
   - Adjust quality settings per platform

## Migration Notes from Old Project

### Key Differences to Address:

1. **PlayerIO → Unity Netcode:**
   - Replace all `PlayerIOClient` references
   - Replace `Message` class with Netcode RPCs
   - Replace `DatabaseObject` with ScriptableObjects or NetworkVariables
   - Replace `BigDB` with Firebase or Unity Cloud Save

2. **Coordinate System:**
   - Old project uses negative Z: `-z` in many places
   - Maintain this convention for compatibility
   - World position: `new Vector3(x, elevation / 30f, -z)`

3. **Tile/Chunk Size:**
   - Old project uses `size = 122` for chunks
   - Maintain this for compatibility with existing data
   - Each chunk = 122x122 pixels = 3.66km x 3.66km real world

4. **Fuel Code Special Values:**
   - 7299 = Roads
   - 7298 = High density urban
   - 7297 = Medium density urban
   - 7296 = Low density urban
   - 99 = Cut line (created by units)
   - 98 = Default/No fuel
   - 91 = Water
   - Maintain these values for compatibility

5. **Unit Properties:**
   - Old project uses `unitData.Get("propertyName")` pattern
   - Consider keeping this for flexibility, or use strongly-typed properties
   - Terrain speed factors stored per fuel code: `terrainSpeedFactorDBO.GetFloat(fuelCode.ToString())`

6. **Fire Behavior Calculations:**
   - Fire intensity formula: `I = 259.833 * (L ^ 2.174) * 30m`
   - Flame length from intensity: `L = ((I / 30) / 259.833) ^ (1 / 2.174)`
   - Maintain these formulas for accuracy

## Testing Checklist

### Phase 2 (GIS Terrain):
- [ ] Load GIS data correctly
- [ ] Terrain tiles generate properly
- [ ] Elevation data displays correctly
- [ ] Fuel codes display correctly
- [ ] Terrain updates when painting
- [ ] Tiles load/unload based on viewer position
- [ ] LOD system works correctly

### Phase 3 (Fuel Code):
- [ ] Fuel codes load from JSON
- [ ] Bezier curves evaluate correctly
- [ ] ROS calculations match old project
- [ ] Flame length calculations match old project
- [ ] Graph editor works correctly
- [ ] Moisture states affect calculations

### Phase 4 (Unit System):
- [ ] Units spawn correctly
- [ ] Units move along paths
- [ ] Units detect fire/water/urban
- [ ] Units suppress fire correctly
- [ ] Units refill water correctly
- [ ] Unit leveling works
- [ ] Task force system works

### Phase 5 (LinePath):
- [ ] LinePaths create correctly
- [ ] LinePathPoints detect conditions
- [ ] Indicators show/hide correctly
- [ ] Pathfinding works between points
- [ ] Units follow paths correctly
- [ ] Path type permissions work

### Phase 6 (Network):
- [ ] Clients connect to server
- [ ] Terrain syncs between clients
- [ ] Units sync between clients
- [ ] LinePaths sync between clients
- [ ] Performance is acceptable

### Phase 7 (Mobile):
- [ ] Game runs on Android
- [ ] Game runs on iOS
- [ ] Performance is acceptable (30+ FPS)
- [ ] Touch input works
- [ ] UI is touch-friendly
- [ ] Battery usage is reasonable

## Conclusion

This rebuild plan provides comprehensive, detailed instructions for rebuilding the wildland firefighting simulator. Each phase builds on the previous one, and the implementation details are based on the actual old project code. Follow the phases in order, and reference the old project files when needed for specific implementation details not covered here.

The system is designed to be:
- **Accurate**: Based on Rothermel's fire behavior model
- **Performant**: LOD system, efficient data structures, mobile optimization
- **Extensible**: Modular design allows for future features
- **Maintainable**: Clean code structure, well-documented
- **Compatible**: Maintains compatibility with old project data formats where possible