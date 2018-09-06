using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainGenerator : MonoBehaviour
{
    
    public float mapSize = 128;
    public int cells = 128;

    // Helper variables
    private int cellsOff;
    private float cellSize;
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

        int[] tris = new int[triCount];

        // Apply new mesh to be the landscape GameObject mesh
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        int triOffset = 0;

        for (int i = 0; i <= cells; i++)
        {
            for (int j = 0; j <= cells; j++)
            {
                vertices[i * cellsOff + j] = new Vector3(-halfMap + j * cellSize, 0.0f, halfMap - i * cellSize);
                uvs[i * cellsOff + j] = new Vector2((float)i / cells, (float)j / cells);

                if (i < cells && j < cells)
                {
                    int topLeft = i * cellsOff + j;
                    int bottomLeft = (i + 1) * cellsOff + j;

                    tris[triOffset + 1] = topLeft;
                    tris[triOffset + 2] = topLeft + 1;
                    tris[triOffset] = bottomLeft + 1;

                    tris[triOffset + 4] = topLeft;
                    tris[triOffset + 5] = bottomLeft + 1;
                    tris[triOffset + 3] = bottomLeft;

                    triOffset += 6;
                }
            }
        }

        // First step of Diamond Square: set random values to four corners of map
        vertices = RandomizeCorners(vertices, maxHeight, cells);

        int iterations = (int)Mathf.Log(cells, 2);
        int numSquares = 1;
        int squareSize = cells;

        for (int i = 0; i <= iterations; i++)
        {
            int row = 0;

            for (int x = 0; x < numSquares; x++)
            {
                int col = 0;

                for (int y = 0; y < numSquares; y++)
                {
                    DiamondSquareAlgorithm(row, col, squareSize, maxHeight);
                    col += squareSize;
                }
                row += squareSize;
            }
            numSquares = numSquares * 2;
            squareSize = squareSize / 2;

            // may want to change this
            maxHeight *= 0.5f;
        }


        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = tris;


        MeshCollider meshc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        meshc.sharedMesh = mesh;

        // Instantiate water plane prefab slightly below average vertex height
        float avgH = AverageHeight(vertices);
        float waterLevel = avgH * 0.9f;
        Instantiate(waterSurface, new Vector3(0, waterLevel, 0), new Quaternion(0, 0, 0, 0));
        Instantiate(groundBlock, new Vector3(0, waterLevel + 0.2f, 0), new Quaternion(0, 0, 0, 0));

        // Set mesh colors
        mesh.colors = SetColors(vertices, waterLevel, MaxHeight(vertices));

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


    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    void DiamondSquareAlgorithm(int row, int col, int size, float offset)
    {
        int halfMap = (int)(size / 2);
        int topLeft = row * cellsOff + col;
        int botLeft = (row + size) * cellsOff + col;
        int mid = (int)(row + halfMap) * cellsOff + (int)(col + halfMap);


        DiamondStep(size, halfMap, topLeft, botLeft, mid, offset);

        SquareStep(size, halfMap, topLeft, botLeft, mid, offset);
    }

    void DiamondStep(int size, int halfMap, int topLeft, int botLeft, int mid, float offset)
    {
        float val = (vertices[topLeft].y + vertices[topLeft + size].y + vertices[botLeft].y + vertices[botLeft + size].y);
        vertices[mid].y = val * 0.25f + Random.Range(-offset, offset);
    }

    void SquareStep(int size, int halfMap, int topLeft, int botLeft, int mid, float offset)
    {
        vertices[topLeft + halfMap].y = CalcSquareHeight(vertices, topLeft, topLeft + size, mid, offset);
        vertices[mid - halfMap].y = CalcSquareHeight(vertices, topLeft, botLeft, mid, offset);
        vertices[mid + halfMap].y = CalcSquareHeight(vertices, topLeft + size, botLeft + size, mid, offset);
        vertices[botLeft + halfMap].y = CalcSquareHeight(vertices, botLeft, botLeft + size, mid, offset);
    }

    float CalcSquareHeight(Vector3[] vertices, int p1, int p2, int p3, float off)
    {
        return (vertices[p1].y + vertices[p2].y + vertices[p3].y) / 3 + Random.Range(-off, off);
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
