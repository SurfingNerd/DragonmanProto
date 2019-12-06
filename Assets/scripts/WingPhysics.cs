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

    public KeyCode moveWingUpKey = KeyCode.None;
    public KeyCode moveWingDownKey = KeyCode.None;

    //public float maxRotation = 0.3;

    public GameObject[] forceDebugBaloons;
    public float[] forceDebugBaloonsMultipliers = new float[] { 10, 5, 3, 1};



    private float aspectRatio;
    private Rigidbody rigidBody;

    private Vector3 lastWingForce;

    private Vector3 debugCubeInitialPosition;
    private Vector3 debugCubeInitialScale;

    private Vector3[] debugBaloonInitialPosition;

    private AirplaneController airplaneController; 

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
        
        rigidBody.AddForce(lastWingForce);
        
        //theBird.AddForce(transform.forward * EnginePower);
    }

    void OnPostRender()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        aspectRatio = (wingSpan * wingSpan) / wingArea;
        rigidBody = this.GetComponent<Rigidbody>();

        if (debugCube) 
        {
            debugCubeInitialPosition = debugCube.transform.localPosition;
            debugCubeInitialScale = debugCube.transform.localScale;
        }

        if (forceDebugBaloons.Length > 0) {
            debugBaloonInitialPosition = new Vector3[forceDebugBaloons.Length];
            for (int i = 0; i < forceDebugBaloons.Length; i++)
            {
                
                debugBaloonInitialPosition[i] =  forceDebugBaloons[i].transform.localPosition;
            }
        }

        // if (showForceDebug) {
        //     //forceDebugVism
        //     Destroy(forceDebugVisual.GetComponent<Collider>());
        //     forceDebugVisual.transform.parent = rigidBody.transform;
        //     //forceDebugVisual.transform.position = new Vector3(2, 1, 0);
        // }

        airplaneController = FindAirplaneController();
    }

    // Update is called once per frame
    void Update()
    {
        if (debugCube != null) 
        {
            debugCube.transform.localPosition = debugCubeInitialPosition + (lastWingForce * 10);
        }

        int counter = 0;
        foreach (var debugBaloon in forceDebugBaloons)
        {
            Debug.Log(counter + " " + forceDebugBaloons[counter].transform.position);
            debugBaloon.transform.localPosition = debugBaloonInitialPosition[counter] + (lastWingForce * forceDebugBaloonsMultipliers[counter]);
            counter++;
        }

        if (airplaneController)
        {
            //this.transform.Rotate()
            Debug.Log("Updating Rotation: " + airplaneController.GetCurrentRotationX());
            this.transform.rotation.Set(airplaneController.GetCurrentRotationX(), this.transform.rotation.y, this.transform.rotation.z, this.transform.rotation.w);
        }


        //if (Input.GetKeyDown(KeyCode.A))
        //FindAirplaneController()
        
    }

    private AirplaneController FindAirplaneController() {
        AirplaneController result = null;
        foreach(var go in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            result = go.GetComponent<AirplaneController>();
            if (result)
            {
                return result;
            }
        }
        Debug.LogError("Expected to find a AirplaneController on one of the Root Objects.");
        return null;
    }
    void FixedUpdate()
    {
       calculateForces();
    }
}
