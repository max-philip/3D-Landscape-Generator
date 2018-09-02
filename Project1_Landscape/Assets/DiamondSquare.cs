using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondSquare : MonoBehaviour {

    public float cellSize;
    public int cells;

    public Color snowColor = new Color(1, 0.9f, 0.9f, 1);
    public Color grassColor = new Color(0.376f, 0.502f, 0.22f, 1);
    public Color sandColor = new Color(0.761f, 0.698f, 0.502f, 1);

    public float maxHeight;

    Vector3[] vertices;
    int vCount;

    // Plane for water
    public GameObject waterSurface;
    public GameObject groundBlock;

    // Use this for initialization
    void Start () {

        vCount = (cells + 1) * (cells + 1);
        vertices = new Vector3[vCount];

        Vector2[] uvs = new Vector2[vCount];
        int[] tris = new int[cells * cells * 6];

        float halfSize = cellSize * 0.5f;
        float divisionSize = cellSize / cells;

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        int triOffset = 0;

        for (int i = 0; i <= cells; i++)
        {
            for (int j = 0; j <= cells; j++)
            {
                vertices[i * (cells + 1) + j] = new Vector3(-halfSize + j * divisionSize, 0.0f, halfSize - i * divisionSize);
                uvs[i * (cells + 1) + j] = new Vector2((float)i / cells, (float)j / cells);

                if ( i < cells && j < cells)
                {
                    int topLeft = i * (cells + 1) + j;
                    int bottomLeft = (i + 1) * (cells + 1) + j;

                    tris[triOffset] = topLeft;
                    tris[triOffset + 1] = topLeft + 1;
                    tris[triOffset + 2] = bottomLeft + 1;

                    tris[triOffset + 3] = topLeft;
                    tris[triOffset + 4] = bottomLeft + 1;
                    tris[triOffset + 5] = bottomLeft;

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
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();


        MeshCollider meshc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        meshc.sharedMesh = mesh;

        // Instantiate WATER PLANE
        float avgH = AverageHeight(vertices);
        float waterLevel = avgH * 0.9f; // a little below halfway bc why not
      
        Instantiate(waterSurface, new Vector3(0, waterLevel, 0), new Quaternion(0, 0, 0, 0));
        Instantiate(groundBlock, new Vector3(0, waterLevel+0.2f, 0), new Quaternion(0, 0, 0, 0));




        mesh.colors = setColors(vertices, avgH, waterLevel);

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
                colors[i] = snowColor;
            } else if (vertices[i].y > sandLevel)
            {
                colors[i] = grassColor;
            } else
            {
                colors[i] = sandColor;
            }
        }
        return colors;
    }

    void DiamondSquareAlgorithm(int row, int col, int size, float offset)
    {
        int halfSize = (int)(size * 0.5f);
        int topLeft = row * (cells + 1) + col;
        int botLeft = (row + size) * (cells + 1) + col;

        int mid = (int)(row + halfSize) * (cells + 1) + (int)(col + halfSize);
        float val = (vertices[topLeft].y + vertices[topLeft + size].y + vertices[botLeft].y + vertices[botLeft + size].y);
        vertices[mid].y = val*0.25f + Random.Range(-offset, offset);



        // averaging step for SQUARE
        vertices[topLeft+halfSize].y = (vertices[topLeft].y + vertices[topLeft+size].y + vertices[mid].y)/ 3 + Random.Range(-offset, offset);
        vertices[mid - halfSize].y = (vertices[topLeft].y + vertices[botLeft].y + vertices[mid].y)/3 + Random.Range(-offset, offset);
        vertices[mid + halfSize].y = (vertices[topLeft + size].y + vertices[botLeft + size].y + vertices[mid].y) / 3 + Random.Range(-offset, offset);
        vertices[botLeft + halfSize].y = (vertices[botLeft].y + vertices[botLeft + size].y + vertices[mid].y) / 3 + Random.Range(-offset, offset);
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
