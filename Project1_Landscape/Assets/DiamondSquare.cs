using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondSquare : MonoBehaviour {

    public float mapSize = 128;
    public int cells = 128;

    // Helper variables
    private int cellsOff;
    private float cellSize;
    private float halfMap;

    // Terrain colour values
    public Color snowColour = new Color(1, 0.9f, 0.9f, 1);
    public Color grassColour = new Color(0.376f, 0.502f, 0.22f, 1);
    public Color sandColour = new Color(0.761f, 0.698f, 0.502f, 1);

    // Maximum POSSIBLE height for vertices on the map
    public float maxHeight;

    Vector3[] vertices;
    int vCount;

    // Plane for water
    public GameObject waterSurface;
    public GameObject groundBlock;

    // Use this for initialization
    void Start () {

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

                if ( i < cells && j < cells)
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

        // set all corners to random values
        vertices[0].y = Random.Range(-maxHeight, maxHeight);
        vertices[cells].y = Random.Range(-maxHeight, maxHeight);
        vertices[vertices.Length - 1].y = Random.Range(-maxHeight, maxHeight);
        vertices[vertices.Length - 1 - cells].y = Random.Range(-maxHeight, maxHeight);

        int iterations = (int)Mathf.Log(cells, 2);
        int numSquares = 1;
        int squareSize = cells;

        for (int i = 0; i <= iterations; i++)
        {
            int row = 0;

            for (int j=0; j < numSquares; j++)
            {
                int col = 0;

                for (int k=0; k < numSquares; k++)
                {
                    DiamondSquareAlgorithm(row, col, squareSize, maxHeight);
                    col += squareSize;
                }
                row += squareSize;
            }
            numSquares *= 2;
            squareSize /= 2;

            // may want to change this
            maxHeight *= 0.5f;
        }


        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = tris;
        


        MeshCollider meshc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        meshc.sharedMesh = mesh;

        // Instantiate WATER PLANE
        float avgH = AverageHeight(vertices);
        float waterLevel = avgH * 0.9f; // a littlesnowColor below halfway bc why not
      
        Instantiate(waterSurface, new Vector3(0, waterLevel, 0), new Quaternion(0, 0, 0, 0));
        Instantiate(groundBlock, new Vector3(0, waterLevel+0.2f, 0), new Quaternion(0, 0, 0, 0));

       


        mesh.colors = setColors(vertices, avgH, waterLevel);

        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
        
    }

    Color[] setColors(Vector3[] vertices, float avgHigh, float waterLevel)
    {
        Color[] colors = new Color[vertices.Length];

        float highestH = HighestHeight(vertices);

        float snowLevel = waterLevel + 16f;
        float sandLevel = waterLevel + 2f;

        for (int i=0; i < vertices.Length; i++)
        {
            if (vertices[i].y > snowLevel)
            {
                colors[i] = snowColour;
            } else if (vertices[i].y > sandLevel)
            {
                colors[i] = grassColour;
            } else
            {
                colors[i] = sandColour;
            }
        }
        return colors;
    }

    void DiamondSquareAlgorithm(int row, int col, int size, float offset)
    {
        int halfMap = (int)(size * 0.5f);
        int topLeft = row * cellsOff + col;
        int botLeft = (row + size) * cellsOff + col;

        int mid = (int)(row + halfMap) * cellsOff + (int)(col + halfMap);
        float val = (vertices[topLeft].y + vertices[topLeft + size].y + vertices[botLeft].y + vertices[botLeft + size].y);
        vertices[mid].y = val*0.25f + Random.Range(-offset, offset);

        // averaging step for SQUARE
        vertices[topLeft+halfMap].y = (vertices[topLeft].y + vertices[topLeft+size].y + vertices[mid].y)/ 3 + Random.Range(-offset, offset);
        vertices[mid - halfMap].y = (vertices[topLeft].y + vertices[botLeft].y + vertices[mid].y)/3 + Random.Range(-offset, offset);
        vertices[mid + halfMap].y = (vertices[topLeft + size].y + vertices[botLeft + size].y + vertices[mid].y) / 3 + Random.Range(-offset, offset);
        vertices[botLeft + halfMap].y = (vertices[botLeft].y + vertices[botLeft + size].y + vertices[mid].y) / 3 + Random.Range(-offset, offset);
    }

    float AverageHeight(Vector3[] vertices)
    {
        float avgH = 0;
        for (int i=0; i < vertices.Length; i++)
        {
            avgH += vertices[i].y;
        }

        return (avgH / vertices.Length);
    }

    float HighestHeight(Vector3[] vertices)
    {
        float highest = 0;
        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].y > highest)
            {
                highest = vertices[i].y;
            }
        }
        return highest;
    }
    
}
