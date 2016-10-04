
using UnityEngine;
using System.Collections;


public class Water : MonoBehaviour
{

    private Mesh mesh;
    private Vector3[] normals;
    private Vector2[] uvs;
    public Vector3[] vertices;
    public bool[] boundVertex;
    public Vector3 location;
    public Vector2 size;
    public Vector2 resolution;
    public Vector2 step;
    private int nVertices;
    private int nBounded;


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
     
        
        Vector3[] newVertices = new Vector3[nVertices];
        int n = 0;

        // iterate through x and y and generate new vertices
        for (int i = 0; i < (int)resolution.x; i++)
        {
            for (int j = 0; j < (int)resolution.y; j++)
            {
                newVertices[n] = new Vector3(i * step.x - size.x / 2.0f, 0.0f, j * step.y - size.y / 2.0f);

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
            newVertices[i] = new Vector3(vertices[i].x, newHeightMap[iH, jH], vertices[i].z);
        }
        mesh.vertices = newVertices;
    }


    public Vector3[] getVertices()
    {
        return mesh.vertices;

    }





}
