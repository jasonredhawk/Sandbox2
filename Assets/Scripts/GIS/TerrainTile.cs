using UnityEngine;

/// <summary>
/// Represents a terrain tile (chunk) and builds a mesh from elevation data.
/// Simplified: no LOD, one-time mesh build.
/// </summary>
public class TerrainTile
{
    public int tileX;
    public int tileZ;

    private readonly ElevationLayer elevationLayer;
    private readonly FuelCodeLayer fuelCodeLayer;
    private readonly MapData mapData;
    private readonly GameManager gameManager;
    private readonly FuelCodeSet fuelCodeSet;
    private readonly int tileSize;
    private readonly float heightScale; // meters to world units (30m -> 1m)

    private GameObject meshObject;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    public TerrainTile(int tileX, int tileZ, Transform parent, Material material, ElevationLayer elevationLayer, FuelCodeLayer fuelCodeLayer, MapData mapData, GameManager gameManager, FuelCodeSet fuelCodeSet)
    {
        this.tileX = tileX;
        this.tileZ = tileZ;
        this.elevationLayer = elevationLayer;
        this.fuelCodeLayer = fuelCodeLayer;
        this.mapData = mapData;
        this.gameManager = gameManager;
        this.fuelCodeSet = fuelCodeSet;
        this.tileSize = (elevationLayer != null && elevationLayer.tileSize > 0) ? elevationLayer.tileSize : 122;
        this.heightScale = 1f / 30f;

        CreateMeshObject(parent, material);
        BuildMesh();
    }

    private void CreateMeshObject(Transform parent, Material material)
    {
        meshObject = new GameObject($"Tile_{tileX}_{tileZ}");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();

        // Always use a vertex-color aware shader so fuel colors show.
        var vc = Shader.Find("Sprites/Default"); // supports vertex colors
        if (material == null || material.shader == null || material.shader.name.Contains("Universal Render Pipeline"))
        {
            var mat = new Material(vc != null ? vc : Shader.Find("Standard"));
            meshRenderer.sharedMaterial = mat;
        }
        else
        {
            // If a non-vertex-color shader is used, colors won't show. Replace with vertex color shader.
            if (!material.shader.name.Contains("Sprites/Default"))
            {
                var mat = new Material(vc != null ? vc : Shader.Find("Standard"));
                meshRenderer.sharedMaterial = mat;
            }
            else
            {
                meshRenderer.sharedMaterial = material;
            }
        }

        meshObject.transform.parent = parent;
        meshObject.transform.position = new Vector3(tileX * tileSize, 0f, -tileZ * tileSize);
    }

    /// <summary>
    /// Builds the mesh from the elevation tile data.
    /// </summary>
    public void BuildMesh()
    {
        if (elevationLayer == null)
        {
            meshFilter.sharedMesh = GenerateFlatMesh();
            return;
        }

        var tileData = elevationLayer.GetTileData(tileX, tileZ);
        short[,] fuelTile = fuelCodeLayer != null ? fuelCodeLayer.GetTileData(tileX, tileZ) : null;
        meshFilter.sharedMesh = GenerateMeshFromHeights(tileData, fuelTile);
    }

    private Mesh GenerateFlatMesh()
    {
        var flatHeights = new short[tileSize, tileSize];
        return GenerateMeshFromHeights(flatHeights, null);
    }

    private Mesh GenerateMeshFromHeights(short[,] heights, short[,] fuels)
    {
        int tileWidth = tileSize;
        int tileHeight = tileSize;
        if (mapData != null)
        {
            tileWidth = Mathf.Min(tileSize, mapData.xWidth - tileX * tileSize);
            tileHeight = Mathf.Min(tileSize, mapData.zWidth - tileZ * tileSize);
            if (tileWidth <= 0 || tileHeight <= 0) tileWidth = tileHeight = 1;
        }

        int vertsPerLineX = tileWidth + 1;
        int vertsPerLineZ = tileHeight + 1;
        Vector3[] vertices = new Vector3[vertsPerLineX * vertsPerLineZ];
        Vector2[] uvs = new Vector2[vertsPerLineX * vertsPerLineZ];
        Color[] colors = new Color[vertsPerLineX * vertsPerLineZ];
        int[] triangles = new int[tileWidth * tileHeight * 6];

        int v = 0;
        for (int z = 0; z < vertsPerLineZ; z++)
        {
            for (int x = 0; x < vertsPerLineX; x++)
            {
                int hx = Mathf.Clamp(x, 0, tileSize - 1);
                int hz = Mathf.Clamp(z, 0, tileSize - 1);
                float h = heights[hx, hz] * heightScale;
                vertices[v] = new Vector3(x, h, -z);
                uvs[v] = new Vector2((float)x / tileWidth, (float)z / tileHeight);
                colors[v] = GetFuelColor(fuels, hx, hz);
                v++;
            }
        }

        int t = 0;
        for (int z = 0; z < tileHeight; z++)
        {
            for (int x = 0; x < tileWidth; x++)
            {
                int i0 = z * vertsPerLineX + x;
                int i1 = i0 + 1;
                int i2 = i0 + vertsPerLineX;
                int i3 = i2 + 1;

                // Winding to face upward with negative Z in vertex positions
                triangles[t++] = i0;
                triangles[t++] = i1;
                triangles[t++] = i2;

                triangles[t++] = i1;
                triangles[t++] = i3;
                triangles[t++] = i2;
            }
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // safety for larger meshes
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    private Color GetFuelColor(short[,] fuels, int x, int z)
    {
        if (fuels == null) return new Color(0.6f, 0.7f, 0.6f, 1f);
        short fc = fuels[Mathf.Clamp(x, 0, fuels.GetLength(0) - 1), Mathf.Clamp(z, 0, fuels.GetLength(1) - 1)];
        // Special categories
        if (fc == 98) return new Color(0.6f, 0.7f, 0.6f, 1f);
        if (FuelCodeLayer.IsRoad(fc)) return Color.black;
        if (FuelCodeLayer.IsUrban(fc)) return new Color(0.5f, 0f, 0.6f, 1f);
        if (FuelCodeLayer.IsWater(fc)) return new Color(0.3f, 0.5f, 0.9f, 1f);

        // FuelCodeData base color
        Color baseColor = new Color(0.6f, 0.7f, 0.6f, 1f);
        FuelCodeData data = fuelCodeSet != null ? fuelCodeSet.GetFuelCode(fc) : null;
        if (data != null) baseColor = data.baseColor;

        // Saturation by flame length, brightness by ROS
        float wind = gameManager != null ? gameManager.windSpeed : 10f;
        MoistureState moisture = gameManager != null ? gameManager.moistureState : MoistureState.Medium;
        float flame = data != null ? data.CalculateFlameLength(wind, moisture) : 1f;
        float ros = data != null ? data.CalculateROS(wind, moisture) : 1f;
        float flameMax = data != null && data.GetFlameCurve(moisture) != null ? data.GetFlameCurve(moisture).max : 10f;
        float rosMax = data != null && data.GetROSCurve(moisture) != null ? data.GetROSCurve(moisture).max : 10f;
        float sat = Mathf.Clamp01(flameMax > 0 ? flame / flameMax : 0.5f);
        float val = Mathf.Clamp01(rosMax > 0 ? ros / rosMax : 0.5f);

        Color.RGBToHSV(baseColor, out float h, out float s, out float v);
        s = Mathf.Lerp(0.3f, 1f, sat);
        v = Mathf.Lerp(0.4f, 1f, val);
        return Color.HSVToRGB(h, s, v);
    }
}
