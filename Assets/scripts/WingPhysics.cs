using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingPhysics : MonoBehaviour
{
    public float wingSpan = 13.56f;
    public float wingArea = 78.04f;

    private float aspectRatio;
    private Rigidbody rigidBody;

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
        var vTotalForce = vLift - vDrag;

        // Lift + Drag = Total Force
        Debug.Log(vLift + " + " + vDrag + " = " + vTotalForce);
        rigidBody.AddForce(vTotalForce);

        //theBird.AddForce(transform.forward * EnginePower);
    }

    // Start is called before the first frame update
    void Start()
    {
        aspectRatio = (wingSpan * wingSpan) / wingArea;
        rigidBody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {  
        calculateForces();
    }
}
