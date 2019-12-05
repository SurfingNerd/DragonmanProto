using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingPhysics : MonoBehaviour
{
    public float wingSpan = 13.56f;
    public float wingArea = 78.04f;

    //public Material forceDebugMaterial;

    public bool showForceDebug = true;

    public GameObject debugCube;




    private float aspectRatio;
    private Rigidbody rigidBody;

    private Vector3 lastWingForce;

    private Vector3 debugCubeInitialPosition;
    private Vector3 debugCubeInitialScale;

    //private GameObject forceDebugVisual;

    private static readonly int[] triangles = {
            0, 2, 1, //face front
            0, 3, 2,
            2, 3, 4, //face top
            2, 4, 5,
            1, 2, 5, //face right
            1, 5, 6,
            0, 7, 4, //face left
            0, 4, 3,
            5, 4, 7, //face back
            5, 7, 6,
            0, 6, 7, //face bottom
            0, 1, 6
        };
    private void calculateForces()
    {
        // *flip sign(s) if necessary*
        var localVelocity = transform.InverseTransformDirection(rigidBody.velocity);
        var angleOfAttack = Mathf.Atan2(-localVelocity.y, localVelocity.z);

        // α * 2 * PI * (AR / AR + 2)
        var inducedLift = angleOfAttack * (aspectRatio / (aspectRatio + 2f)) * 2f * Mathf.PI;

        // CL ^ 2 / (AR * PI)
        var inducedDrag = (inducedLift * inducedLift) / (aspectRatio * Mathf.PI);

        // V ^ 2 * R * 0.5 * A
        var pressure = rigidBody.velocity.sqrMagnitude * 1.2754f * 0.5f * wingArea;

        var lift = inducedLift * pressure;
        var drag = (0.021f + inducedDrag) * pressure;

        // *flip sign(s) if necessary*
        var dragDirection = rigidBody.velocity.normalized;
        var liftDirection = Vector3.Cross(dragDirection, transform.right);

        var vLift = liftDirection * lift;
        var vDrag = dragDirection * drag;
        lastWingForce = (vLift - vDrag) * Time.deltaTime * 10;

        // Lift + Drag = Total Force
        //Debug.Log(vLift + " + " + vDrag + " = " + vTotalForce);
        Debug.Log("Last Force: " + lastWingForce.magnitude);

        //Debug.DrawLine(rigidBody.transform.position ,rigidBody.transform.position + lastWingForce * 100000000, Color.green, 1);
        if (debugCube != null) 
        {
            debugCube.transform.localPosition = debugCubeInitialPosition + (lastWingForce * 10);
        }
        
        rigidBody.AddForce(lastWingForce);

        

        //theBird.AddForce(transform.forward * EnginePower);
    }

    void OnPostRender()
    {
        // Debug.Log("Entering OnPostRender");
        // if (showForceDebug)
        // {
        //     if (!forceDebugMaterial)
        //     {
        //         Debug.LogError("Please Assign a material on the inspector");
        //         return;
        //     }
        //     GL.PushMatrix();
        //     forceDebugMaterial.SetPass(0);
        //     GL.LoadOrtho();
        //     GL.Begin(GL.LINES);
        //     GL.Color(Color.red);
        //     GL.Vertex(rigidBody.position);
        //     GL.Vertex(rigidBody.position + lastWingForce * 1000);
        //     GL.End();
        //     GL.PopMatrix();
        // }
        
        
        // float xOffset = 10000000;
        // float yOffset = 10000000;
        // float zOffset = 10000000;

        // Vector3[] vertices = {
        //     new Vector3(rigidBody.position.x, rigidBody.position.y, rigidBody.position.z),
        //     new Vector3(rigidBody.position.x + xOffset, rigidBody.position.y, rigidBody.position.z),
        //     new Vector3(rigidBody.position.x + xOffset, rigidBody.position.y + yOffset, rigidBody.position.z),
        //     new Vector3(rigidBody.position.x, rigidBody.position.y + yOffset, rigidBody.position.z),
        //     new Vector3 (rigidBody.position.x, rigidBody.position.y + yOffset, rigidBody.position.z + zOffset),
        //     new Vector3 (rigidBody.position.x + xOffset, rigidBody.position.y + yOffset, rigidBody.position.z + zOffset),
        //     new Vector3 (rigidBody.position.x + xOffset, rigidBody.position.y, rigidBody.position.z + zOffset),
        //     new Vector3 (rigidBody.position.x, rigidBody.position.y, rigidBody.position.z + zOffset)
        // };


        // const float s = 10000;

        // Vector3[] vertices = {
        //     new Vector3 (0, 0, 0),
        //     new Vector3 (s, 0, 0),
        //     new Vector3 (s, s, 0),
        //     new Vector3 (0, s, 0),
        //     new Vector3 (0, s, s),
        //     new Vector3 (s, s, s),
        //     new Vector3 (s, 0, s),
        //     new Vector3 (0, 0, s),
        // };

        // //Mesh mesh = Mesh.

        // Mesh mesh = new Mesh();
        // mesh.vertices = vertices;
        // mesh.triangles = triangles;

        // Graphics.DrawMesh( mesh, rigidBody.position, Quaternion.identity, forceDebugMaterial, 14, null, 0, null, false, false, false); 

    }

    

    // void OnDrawGizmos()
    // {
    //     if (rigidBody)
    //     {
    //         Gizmos.DrawLine(rigidBody.centerOfMass, lastWingForce * 100000) ;
    //     }
    // }

    // Start is called before the first frame update
    void Start()
    {
        aspectRatio = (wingSpan * wingSpan) / wingArea;
        rigidBody = this.GetComponent<Rigidbody>();

        if (debugCube) 
        {
            debugCubeInitialPosition = debugCube.transform.position;
            debugCubeInitialScale = debugCube.transform.localScale;
        }

        // if (showForceDebug) {
        //     //forceDebugVisual = GameObject.CreatePrimitive(PrimitiveType.) CreatePrimitive(PrimitiveType.Capsule);
        //     GameObject go = new GameObject("WingForceDebug");
            

        //     Destroy(forceDebugVisual.GetComponent<Collider>());
        //     forceDebugVisual.transform.parent = rigidBody.transform;
        //     //forceDebugVisual.transform.position = new Vector3(2, 1, 0);
        // }
    }

    // Update is called once per frame
    void Update()
    {  
        calculateForces();
        //OnPostRender();
    }
}
