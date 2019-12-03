﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlightControl1 : MonoBehaviour
{
    //USER CONTROLS***************************
    public float rollSpeed;
    public float pitchSpeed;
    public float yawSpeed;

    //PHYSICS VARIABLES******************************************************

    //The birds mass
    public Rigidbody theBird;

    //Upward and forward thrust generated by wing flaps
    public float thrustPower;
    public float upThrust;
    public float forwardThrust;

    //Air physics - Drag Resistance Applied Inverse to Velocity
    public float radius; //Radius of the flight object -- Higher equals more drag
    private float dragArea; //Area of the flight object -- in this case a sphere
    private Vector3 currentDirection;
    private float angle;
    private float resistanceFactor; // Resistance per k/m^3. Try to keep below 1
    private float maxResistance; //Per cubic meter
    private float velocityFactor;

    //Air Physics - Lift Force Inverse to Velocity
    //public float requiredAirSpeedForFlight;
    //private float lift;

    //***************************************************************************


    public float wingSpan = 13.56f;
    public float wingArea = 78.04f;

    private float aspectRatio;

    private void Awake()
    {
        theBird.drag = Mathf.Epsilon;
        aspectRatio = (wingSpan * wingSpan) / wingArea;
    }

    private void calculateForces()
    {
        // *flip sign(s) if necessary*
        var localVelocity = transform.InverseTransformDirection(theBird.velocity);
        var angleOfAttack = Mathf.Atan2(-localVelocity.y, localVelocity.z);

        // α * 2 * PI * (AR / AR + 2)
        var inducedLift = angleOfAttack * (aspectRatio / (aspectRatio + 2f)) * 2f * Mathf.PI;

        // CL ^ 2 / (AR * PI)
        var inducedDrag = (inducedLift * inducedLift) / (aspectRatio * Mathf.PI);

        // V ^ 2 * R * 0.5 * A
        var pressure = theBird.velocity.sqrMagnitude * 1.2754f * 0.5f * wingArea;

        var lift = inducedLift * pressure;
        var drag = (0.021f + inducedDrag) * pressure;

        // *flip sign(s) if necessary*
        var dragDirection = theBird.velocity.normalized;
        var liftDirection = Vector3.Cross(dragDirection, transform.right);

        var vLift  =  liftDirection * lift;
        var vDrag = dragDirection * drag;
        var vTotalForce = vLift - vDrag;

        // Lift + Drag = Total Force
        Debug.Log(vLift + " + " +  vDrag + " = " + vTotalForce);
        theBird.AddForce(vTotalForce);

        //theBird.AddForce(transform.forward * EnginePower);
    }

    void Start()
    {
        SetInitialReferences();
        Debug.Log("FlightControl1 Started");

    }

    void FixedUpdate()
    {
        LiftOff();
        DirectionalControl();

        if (Input.GetKeyDown(KeyCode.R))
        {
            Scene scene = SceneManager.GetActiveScene(); 
            SceneManager.LoadScene(scene.name);
        }
    }

    void LateUpdate()
    {
        ApplyForce();
        calculateForces();
    }

    void SetInitialReferences()
    {
        theBird = GetComponent<Rigidbody>();
    }

    void DirectionalControl()
    {
        float h = Input.GetAxis("Horizontal") * rollSpeed * Time.deltaTime;
        float v = Input.GetAxis("Vertical") * pitchSpeed * Time.deltaTime;
        float y = Input.GetAxis("Yaw") * yawSpeed * Time.deltaTime;

        Debug.Log("Applying Torque " + h + " " + v + " " + y);

        theBird.AddTorque(transform.forward * -h);
        theBird.AddTorque(transform.right * v);
        theBird.AddTorque(transform.up * y);
    }


    void LiftOff()
    {
        if (Input.GetButtonDown("Jump"))
            theBird.AddRelativeForce(0, upThrust, forwardThrust, ForceMode.Impulse);

    }

    float FindVelocityFactor()
    {

        return theBird.velocity.magnitude;

    }

    float FindResistanceFactor()
    {
        //Get the constant area and maximum resistance
        dragArea = Mathf.PI * Mathf.Pow(radius, 2);
        maxResistance = dragArea * Mathf.Pow(resistanceFactor, 3);
        currentDirection = theBird.velocity;               //Birds direction
        angle = Vector3.Angle(transform.forward, currentDirection);    //Angle between bird direction and velocity direction
        return Mathf.Abs(Mathf.Sin(angle));                        //Return the resistance factor
        return theBird.velocity.magnitude;                        //Gets and returns the birds velocity magnitude
    }

    void ApplyForce()
    {
        float magnitude = maxResistance * resistanceFactor * velocityFactor; //Magnitude of air resistance
        Vector3 direction = transform.forward.normalized * -1;  //calculate the direction
        theBird.AddRelativeForce(direction * magnitude);        //Add the force to the rigidbody
    }

    //    float LiftEquation()
    //    {
    //        if (airSpeed >= requiredAirSpeedForFlight) 
    //        {
    //            theBird.AddRelativeForce (0, lift, 0);
    //        }
    //    }
    //
    //    float DefineLift()
    //    {
    //        
    //    }
    //
    //    float FindAirSpeed()
    //    {
    //        
    //    }


}
