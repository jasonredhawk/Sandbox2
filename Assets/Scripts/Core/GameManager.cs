using UnityEngine;
#if UNITY_TEXTMESHPRO
using TMPro;
#endif

/// <summary>
/// Main game manager for the wildland firefighting simulator
/// Manages core game state, references to systems, and global settings
/// Based on old project's GameManager.cs
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [Header("Core Systems")]
    public MapData mapData;
    public TerrainGenerator terrainGenerator;
    public FuelCodeManager fuelCodeManager;
    public LinePathManager linePathManager;
    
    [Header("Game Settings")]
    public float gameSpeed = 1f; // Time scale multiplier
    public float windSpeed = 10f; // Wind speed in mph or km/h
    public MoistureState moistureState = MoistureState.Medium;
    
    [Header("Fire Behavior Settings")]
    public float litresPerKilowatt = 0.1f; // Liters of water needed per kilowatt of fire intensity
    
    [Header("Map Settings")]
    public int pixelsPerMap = 122; // Pixels per map tile (chunk size)
    public int tileWidth = 1; // Number of tiles wide
    public int tileHeight = 1; // Number of tiles tall
    public int mapTileWidth = 1;
    public int mapTileHeight = 1;
    public int joinGameWidth = 0;
    public int joinGameHeight = 0;
    
    [Header("Geographic Settings")]
    public int leftMostEdge = 0;
    public int topMostEdge = 0;
    public int longitudeMeterStep = 30; // 30m per pixel
    public int latitudeMeterStep = 30;
    public int isCaliforniaMap = 0; // 1 if California map, 0 otherwise
    
    [Header("UI")]
#if UNITY_TEXTMESHPRO
    public TMPro.TextMeshProUGUI consoleLog;
#else
    public UnityEngine.UI.Text consoleLog;
#endif
    public Camera mainCamera;
    
    [Header("Game State")]
    public bool isLoadFromDB = false;
    public bool isResetCameraToMap = false;
    public bool isLoadingData = false;
    
    [Header("Time")]
    public float elapsedTime = 0f;
    
    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Initialize systems
        if (fuelCodeManager == null)
        {
            fuelCodeManager = GetComponent<FuelCodeManager>();
            if (fuelCodeManager == null)
            {
                fuelCodeManager = gameObject.AddComponent<FuelCodeManager>();
            }
        }
        
        if (terrainGenerator == null)
        {
            terrainGenerator = GetComponent<TerrainGenerator>();
        }
    }
    
    void Update()
    {
        elapsedTime += Time.deltaTime * gameSpeed;
    }
    
    /// <summary>
    /// Get ROS (Rate of Spread) for a fuel code
    /// Delegates to FuelCodeManager
    /// </summary>
    public float GetROS(short fuelCode, float windSpeed)
    {
        if (fuelCodeManager != null)
        {
            return fuelCodeManager.GetROS(fuelCode, windSpeed, moistureState);
        }
        return 0f;
    }
    
    /// <summary>
    /// Get flame length for a fuel code
    /// Delegates to FuelCodeManager
    /// </summary>
    public float GetFlameLength(short fuelCode, float windSpeed)
    {
        if (fuelCodeManager != null)
        {
            return fuelCodeManager.GetFlameLength(fuelCode, windSpeed, moistureState);
        }
        return 0f;
    }
}
