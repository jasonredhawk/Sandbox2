using UnityEngine;

/// <summary>
/// Creates and wires a GameRoot at runtime if not present in scene.
/// Assign references if you have existing assets (layers/materials).
/// </summary>
public class GameRootSetup : MonoBehaviour
{
    [Header("Assets")]
    public ElevationLayer elevationLayer;
    public FuelCodeLayer fuelCodeLayer;
    public Material terrainMaterial;
    public FuelCodeSet fuelCodeSet;

    [Header("Dimensions")]
    public int xWidth = 256;    // pixels
    public int zWidth = 256;    // pixels
    public int startLongitudeMeter = 0;
    public int startLatitudeMeter = 0;
    public int longitudeMeterStep = 30;
    public int latitudeMeterStep = -30; // top-to-bottom

    void Awake()
    {
        // If a GameManager already exists, don't create another root.
        if (FindObjectOfType<GameManager>() != null) return;

        var root = new GameObject("GameRoot");
        var gm = root.AddComponent<GameManager>();
        var md = root.AddComponent<MapData>();
        var tg = root.AddComponent<TerrainGenerator>();
        var nm = root.AddComponent<NetworkManagerGO>();

        md.elevationLayer = elevationLayer;
        md.fuelCodeLayer = fuelCodeLayer;
        md.Initialize(xWidth, zWidth, startLongitudeMeter, startLatitudeMeter, longitudeMeterStep, latitudeMeterStep);

        tg.mapData = md;
        tg.elevationLayer = elevationLayer;
        tg.terrainMaterial = terrainMaterial;

        gm.mapData = md;
        gm.terrainGenerator = tg;
        if (gm.fuelCodeManager == null)
        {
            gm.fuelCodeManager = gm.gameObject.AddComponent<FuelCodeManager>();
        }
        gm.fuelCodeManager.fuelCodeSet = fuelCodeSet;

        // Fill defaults if layers are assigned
        short defaultFuel = 98;
        if (fuelCodeSet != null && fuelCodeSet.fuelCodes.Count > 0)
        {
            defaultFuel = fuelCodeSet.fuelCodes[0].fuelCodeID;
        }
        md.Fill(0, defaultFuel);

        // Fallback terrain material if missing
        if (tg.terrainMaterial == null)
        {
            var lit = Shader.Find("Universal Render Pipeline/Lit");
            var def = Shader.Find("Standard");
            tg.terrainMaterial = new Material(lit != null ? lit : def);
        }

        // Build tiles immediately so they exist before play
        tg.BuildAllTilesImmediate();
    }
}
