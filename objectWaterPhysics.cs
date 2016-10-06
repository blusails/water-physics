using UnityEngine;
using System.Collections;

public class objectWaterPhysics : MonoBehaviour {

    public Water water;
    public interactiveWaves iwaves;
    public Vector3 nearestVert;
    public int nearestVertInd;
    public float displacement;
    public Vector2 waterPos;
    public Vector2 currentPos;
    public Vector2 previousPos;
    public float yPos;
    private Vector2[] searchQuad;
    private int[] searchQuadInds;
    public float density;
    public Rigidbody Rb;
    public float sourceFactor;

    // Use this for initialization
    void Start () {

        GameObject waterPlaneGO = GameObject.Find("waterPlane");
        water = waterPlaneGO.GetComponent<Water>();
        iwaves = waterPlaneGO.GetComponent<interactiveWaves>();
        // get global position of waterPlane
        waterPos = new Vector2(water.location.x,water.location.z);
        Rb = GetComponent<Rigidbody>();



    }
	
	// Update is called once per frame
	void FixedUpdate () {
        
        // update object position
        currentPos = new Vector2(transform.position.x, transform.position.z);
        // recalculation nearest vertex if object has moved more than one dx 
        //bool recalculateNearestVertex = Vector2.Distance(currentPos, previousPos) > water.step.x;
        bool recalculateNearestVertex = true; 
        if (recalculateNearestVertex)
        {
            identifyNearestVertex();
        }


        // measure vertical displacement
        displacement = transform.position.y - nearestVert.y;

        // apply water perturbation
        iwaves.zeroSource();

        if (displacement<0)
        {
            displacement = displacement * sourceFactor;
            int iH = Mathf.FloorToInt(nearestVertInd / (int)water.resolution.y);
            int jH = nearestVertInd - iH * (int)water.resolution.y;
            for (int k = 1; k < iwaves.P + 1; k++)
            {
                for (int l = 1; l < iwaves.P + 1; l++)
                {
                    if ((iH + k) < iwaves.xRes && (iH - k) > 0 && (jH + l) < iwaves.xRes && (jH - l) > 0)
                    {
                        iwaves.source[iH-k, jH-l] = iwaves.source[iH-k, jH-l] + displacement * iwaves.kernel[k, l];
                        iwaves.source[iH + k, jH + l] = iwaves.source[iH + k, jH + l] + displacement * iwaves.kernel[k, l];
                    }
                }

            }

            iwaves.source[iH, jH] = iwaves.source[iH, jH]+displacement;

            
            Rb.AddForce(Vector3.up * Mathf.Abs(displacement/ sourceFactor) * Mathf.Abs(displacement/sourceFactor) * density);
            Rb.drag = 1.5f;
        }
        else
        {
            Rb.drag = .5f;
        }
             


        previousPos = currentPos;

    }
    void identifyNearestVertex()
    {

        // determine occupied quadrant of waterPlan using relative positions
        // 1st quadrant includes +x and +y axis boundaries  (read x and z - thinking old school cartesian)
        // 3rd quadrant includes -x and -y axis boundaries  (read x and z - thinking old school cartesian)

        bool quad1 = currentPos.x >= waterPos.x && currentPos.y >= waterPos.y;
        bool quad2 = currentPos.x < waterPos.x && currentPos.y > waterPos.y;
        bool quad3 = currentPos.x <= waterPos.x && currentPos.y <= waterPos.y;
        bool quad4 = currentPos.x > waterPos.x && currentPos.y < waterPos.y;

        // determine corresponding vertex indices
       
        if (quad1)
        {
            searchQuad = water.getVertexQuadrant(1);
            searchQuadInds = water.getQuadrantInds(1);
        }
        if (quad2)
        {
            searchQuad = water.getVertexQuadrant(2);
            searchQuadInds = water.getQuadrantInds(2);
        }
        if (quad3)
        {
            searchQuad = water.getVertexQuadrant(3);
            searchQuadInds = water.getQuadrantInds(3);
        }
        if (quad4)
        {
            searchQuad = water.getVertexQuadrant(4);
            searchQuadInds = water.getQuadrantInds(4);
        }








        // identify nearest vertex
        int minInd=-1;
        float zero = 0;
        float minDist = 1.0f/zero;
        float currentDist;

        for (int i=0;  i<searchQuad.Length; i++)
        {
            currentDist = Vector2.Distance(currentPos, searchQuad[i]);
            if (currentDist<minDist)
            {
                minDist = currentDist;
                minInd = i;
            }
        }
        //print(searchQuad.Length);

        nearestVertInd = searchQuadInds[minInd];
        nearestVert = water.vertices[nearestVertInd];
       


    }


    


}


