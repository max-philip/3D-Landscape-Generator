using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainGenerator : MonoBehaviour
{
    public int mapSize = 128;
    public int cells = 128;

    // Helper variables
    private int cellsOff;
    private int cellSize;
    private float halfMap;

    // Terrain color values
    public Color snowcolor = new Color(1, 0.9f, 0.9f, 1);
    public Color grasscolor = new Color(0.376f, 0.502f, 0.22f, 1);
    public Color sandcolor = new Color(0.761f, 0.698f, 0.502f, 1);

    public float grassOffset = 3.0f;

    // Maximum POSSIBLE height for vertices on the map
    public float maxHeight;

    Vector3[] vertices;
    int vCount;

    // Plane for water
    public GameObject waterSurface;
    public GameObject groundBlock;

    // Use this for initialization
    void Start()
    {
        GenerateTerrain();
    }

    // Generate the terrain
    void GenerateTerrain()
    {
        // Setting useful variables to help with setting vertex values
        cellSize = mapSize / cells;
        halfMap = mapSize * 0.5f;
        cellsOff = cells + 1;

        // Initialise 2D array of vertices
        vCount = cellsOff * cellsOff;
        vertices = new Vector3[vCount];

        // Initialise array to store UVs and an array to store triangles
        Vector2[] uvs = new Vector2[vCount];

        // Number of triangles on map is double the map size, and triangles
        // have 3 vertices
        int triCount = cells * cells * 2;
        triCount *= 3;

        int[] triangles = new int[triCount];

        int offset = 0;

        for (int x = 0; x <= cells; x++)
        {
            for (int y = 0; y <= cells; y++)
            {
                vertices[x * cellsOff + y] = new Vector3(-halfMap + y * cellSize, 0.0f, halfMap - x * cellSize);
                uvs[x * cellsOff + y] = new Vector2((x / cells), (y / cells));

                if ((x < cells) && (y < cells))
                {
                    int top_left = x * cellsOff + y;
                    int bottom_left = (x + 1) * cellsOff + y;

                    triangles[offset] = bottom_left + 1;
                    triangles[offset + 1] = top_left;
                    triangles[offset + 2] = top_left + 1;

                    triangles[offset + 3] = bottom_left;
                    triangles[offset + 4] = top_left;
                    triangles[offset + 5] = bottom_left + 1;

                    offset += 6;
                }
            }
        }

        // First step of Diamond Square: set random values to four corners of map
        vertices = RandomizeCorners(vertices, maxHeight, cells);

        // Do the Diamond Square Algorithm iterations
        DoIterations(cells, maxHeight);

        // Apply new mesh to be the landscape mesh
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Assign our vertex, uv and triangle arrays
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        // Instantiate water plane prefab slightly below average vertex height
        float avgH = AverageHeight(vertices);
        float waterLevel = avgH * 0.9f;
        Instantiate(waterSurface, new Vector3(0, waterLevel, 0), new Quaternion(0, 0, 0, 0));
        Instantiate(groundBlock, new Vector3(0, waterLevel + 0.2f, 0), new Quaternion(0, 0, 0, 0));

        // Set mesh colors
        mesh.colors = SetColors(vertices, waterLevel, MaxHeight(vertices));

        // Terrain collider
        MeshCollider mesh_collider = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        mesh_collider.sharedMesh = mesh;

        // Recalculate after all other operations are complete
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
    }

    Vector3[] RandomizeCorners(Vector3[] vertices, float maxHeight, int cells)
    {
        // Corners
        int TLeft = 0;
        int TRight = cells;
        int BLeft = vertices.Length - cells - 1;
        int BRight = vertices.Length - 1;

        // Initialise the corner values to random within the map range
        vertices[TLeft].y = Random.Range(-maxHeight, maxHeight);
        vertices[BLeft].y = Random.Range(-maxHeight, maxHeight);
        vertices[BRight].y = Random.Range(-maxHeight, maxHeight);
        vertices[TRight].y = Random.Range(-maxHeight, maxHeight);

        return vertices;
    }

    // Perform diamond square iterations
    void DoIterations(int cells, float maxHeight)
    {
        // Starting number of squares and each squares initial dimension
        int curr_squares = 1;
        int square_dims = cells;

        // Total iterations until completion
        int total_iter = (int) Mathf.Log(cells, 2);

        for (int curr_iter = 0; curr_iter <= total_iter; curr_iter++)
        {
            int row = 0;

            for (int x = 0; x < curr_squares; x++)
            {
                int col = 0;

                for (int y = 0; y < curr_squares; y++)
                {
                    DiamondSquareAlgorithm(row, col, square_dims, maxHeight);
                    col += square_dims;
                }
                row += square_dims;
            }
            // Recalculate values after each iteration
            curr_squares = curr_squares * 2;
            square_dims = square_dims / 2;
            maxHeight *= 0.5f;
        }
    }

    Color[] SetColors(Vector3[] vertices, float waterLevel, float maxHigh)
    {
        // Vertex colors array
        Color[] colors = new Color[vertices.Length];

        float snowLevel = (maxHigh + waterLevel) / 1.8f;
        float grassLevel = waterLevel + grassOffset;

        // Set vertex color based on height (snow, grass or sand)
        for (int i=0; i < vertices.Length; i++)
        {
            if (vertices[i].y > snowLevel)
            {
                colors[i] = snowcolor;
            } else if (vertices[i].y > grassLevel)
            {
                colors[i] = grasscolor;
            } else
            {
                colors[i] = sandcolor;
            }
        }
        return colors;
    }

    // Perform diamond and square steps of the algorithm
    void DiamondSquareAlgorithm(int row, int col, int size, float height_offset)
    {
        int halfMap = (int)(size / 2);
        int topLeft = row * cellsOff + col;
        int botLeft = (row + size) * cellsOff + col;
        int mid = (int)(row + halfMap) * cellsOff + (int)(col + halfMap);

        DiamondStep(size, halfMap, topLeft, botLeft, mid, height_offset);

        SquareStep(size, halfMap, topLeft, botLeft, mid, height_offset);
    }

    void DiamondStep(int size, int halfMap, int topLeft, int botLeft, int mid, float height_offset)
    {
        float val = (vertices[topLeft].y + vertices[topLeft + size].y + vertices[botLeft].y + vertices[botLeft + size].y);
        vertices[mid].y = val * 0.25f + Random.Range(-height_offset, height_offset);
    }

    void SquareStep(int size, int halfMap, int topLeft, int botLeft, int mid, float height_offset)
    {
        vertices[topLeft + halfMap].y = CalcSquareHeight(vertices, topLeft, topLeft + size, mid, height_offset);
        vertices[mid - halfMap].y = CalcSquareHeight(vertices, topLeft, botLeft, mid, height_offset);
        vertices[mid + halfMap].y = CalcSquareHeight(vertices, topLeft + size, botLeft + size, mid, height_offset);
        vertices[botLeft + halfMap].y = CalcSquareHeight(vertices, botLeft, botLeft + size, mid, height_offset);
    }

    float CalcSquareHeight(Vector3[] vertices, int p1, int p2, int p3, float height_offset)
    {
        return (vertices[p1].y + vertices[p2].y + vertices[p3].y) / 3 + Random.Range(-height_offset, height_offset);
    }

    // Get average height of all vertices in the generated terrain
    float AverageHeight(Vector3[] vertices)
    {
        float avgH = 0;
        for (int i=0; i < vertices.Length; i++)
        {
            avgH += vertices[i].y;
        }

        return (avgH / vertices.Length);
    }

    // Get height value of highest vertex in the terrain
    float MaxHeight(Vector3[] vertices)
    {
        float maxH = 0.0f;
        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].y > maxH)
            {
                maxH = vertices[i].y;
            }
        }

        return maxH;
    }
}
