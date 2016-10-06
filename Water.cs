
using UnityEngine;
using System.Collections;


public class Water : MonoBehaviour
{

    private Mesh mesh;
    private Vector3[] normals;
    private Vector2[] uvs;
    public Vector3[] vertices;
    public int[] vertexQuad;
    public bool[] boundVertex;
    public Vector3 location;
    public Vector2 size;
    public Vector2 resolution;
    public Vector2 step;
    private int nVertices;
    private int nBounded;
    public Vector2[] quad1Verts;
    public int[] quad1Inds;
    public Vector2[] quad2Verts;
    public int[] quad2Inds;
    public Vector2[] quad3Verts;
    public int[] quad3Inds;
    public Vector2[] quad4Verts;
    public int[] quad4Inds;


    // Use this for initialization
    void Start()
    {
        step.x = size.x / resolution.x;
        step.y = size.y / resolution.y;
        nVertices = (int)resolution.x * (int)resolution.y;
        mesh = GetComponent<MeshFilter>().mesh;
        location = transform.position;
        resetMesh();


    }


    // build vertices array based on resoltuion and size
    // additionally 
    void assignTriangles()
    {
        int nTriangles = (nVertices - (int)resolution.x-(int)resolution.y+1)*2;
        int[] newTriangles = new int[nTriangles*3];
        int iTri = 0;
        for (int i = 0; i < (nVertices - (int)resolution.x -1); i++)
        {
            if (((i +1)% (int)resolution.x ) != 0 || i < 2)
                
            {
                
                // get vertex indices for 1st triangle
                newTriangles[iTri] = i;
                iTri++;
                newTriangles[iTri] = i + 1;
                iTri++;
                newTriangles[iTri] = i + (int)resolution.x + 1;
                iTri++;

                // get indices for second triangle
                newTriangles[iTri] = i;
                iTri++;
                newTriangles[iTri] = i + (int)resolution.x + 1;
                iTri++;
                newTriangles[iTri] = i + (int)resolution.x;
                iTri++;

            }

        }
        mesh.triangles = newTriangles;


    }

    void assignVertices()
    {
        // reset vertex counters
        nBounded = 0;
        bool quad1;
        bool quad2;
        bool quad3;
        bool quad4;
     
        
        Vector3[] newVertices = new Vector3[nVertices];
        vertexQuad = new int[nVertices];
        int n = 0;

        // iterate through x and y and generate new vertices
        for (int i = 0; i < (int)resolution.x; i++)
        {
            for (int j = 0; j < (int)resolution.y; j++)
            {
                newVertices[n] = new Vector3(i * step.x - size.x / 2.0f, 0.0f, j * step.y - size.y / 2.0f);

                quad1 = (i + 1) / resolution.x > 0.5f && (j + 1) / resolution.y > 0.5f;
                quad2 = (i + 1) / resolution.x < 0.5f && (j + 1) / resolution.y > 0.5f;
                quad3 = (i + 1) / resolution.x < 0.5f && (j + 1) / resolution.y < 0.5f;
                quad4 = (i + 1) / resolution.x > 0.5f && (j + 1) / resolution.y < 0.5f;

                if (quad1)
                {
                    vertexQuad[n] = 1;
                }
                if (quad2)
                {
                    vertexQuad[n] = 2;
                }
                if (quad3)
                {
                    vertexQuad[n] = 3;
                }
                if (quad4)
                {
                    vertexQuad[n] = 4;
                }
                // test if a vertex is on a boundary of the object
                if (i == 0 || j == 0 || i == ((int)resolution.x - 1) || j == ((int)resolution.y - 1))
                {
                    nBounded++;
                }

                n++;

            }
        }
        mesh.vertices = newVertices;
    }

    void resetMesh()
    {

        assignVertices();
        assignTriangles();
        mesh.RecalculateNormals();
        separateVertices();


    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void setHeightMap(float[,] newHeightMap)
    {
        vertices = mesh.vertices;
        nVertices = vertices.Length;
        Vector3[] newVertices = new Vector3[nVertices];
        for (int  i=0;i< nVertices; i++)
        {
            int iH = Mathf.FloorToInt(i / (int)resolution.y);
            int jH = i - iH * (int)resolution.y;
            float val = newHeightMap[iH, jH];
            newVertices[i] = new Vector3(vertices[i].x, val, vertices[i].z);
        }
        mesh.vertices = newVertices;
        mesh.RecalculateNormals();
    }


    public Vector3[] getVertices()
    {
        return mesh.vertices;

    }

    public Vector2[] getVertexQuadrant(int quad)
    {
        //print(quad1Verts.Length);
        switch (quad)
        {
            
            case 1:
                return quad1Verts;
            case 2:
                return quad2Verts;
            case 3:
                return quad3Verts;
            case 4:
                return quad4Verts;
            default:
                return null;

        }
    }

    public int[] getQuadrantInds(int quad)
    {
        switch (quad)
        {
            case 1:
                return quad1Inds;
            case 2:
                return quad2Inds;
            case 3:
                return quad3Inds;
            case 4:
                return quad4Inds;
            default:
                return null;
        }
    }


    void separateVertices()
    {
        vertices = mesh.vertices;
        int nVertices = vertices.Length;
        int nPerQuad = Mathf.CeilToInt(nVertices / 4.0f);
        //print(nVertices);
        int n1 = 0;
        int n2 = 0;
        int n3 = 0;
        int n4 = 0;
        quad1Verts = new Vector2[nPerQuad];
        quad2Verts = new Vector2[nPerQuad];
        quad3Verts = new Vector2[nPerQuad];
        quad4Verts = new Vector2[nPerQuad];
        quad1Inds = new int[nPerQuad];
        quad2Inds = new int[nPerQuad];
        quad3Inds = new int[nPerQuad];
        quad4Inds = new int[nPerQuad];

        for (int i = 1; i < nVertices; i++)
        {
            if (vertexQuad[i] == 1)
            {
                quad1Inds[n1] = i;
                quad1Verts[n1] = new Vector2(vertices[i].x, vertices[i].z);
                n1++;
            }

            if (vertexQuad[i] == 2)
            {
                quad2Inds[n2] = i;
                quad2Verts[n2] = new Vector2(vertices[i].x, vertices[i].z);
                n2++;
            }

            if (vertexQuad[i] == 3)
            {
                quad3Inds[n3] = i;
                quad3Verts[n3] = new Vector2(vertices[i].x, vertices[i].z);
                n3++;
            }

            if (vertexQuad[i] == 4)
            {
                quad4Inds[n4] = i;
                quad4Verts[n4] = new Vector2(vertices[i].x, vertices[i].z);
                n4++;
            }
        }
        
    }




}
