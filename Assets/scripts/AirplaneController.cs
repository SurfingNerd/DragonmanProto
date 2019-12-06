using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneController : MonoBehaviour
{

    public float maxRotation = 45;
    //public float minRotation = -45;

    public UnityEngine.Camera[] cameras;

    public UnityEngine.UI.Slider leftControl;
    public UnityEngine.UI.Slider rightControl;
    // Start is called before the first frame update
    void Start()
    {
        leftControl.minValue = 0;
        leftControl.maxValue = maxRotation * 2;
        rightControl.minValue = 0;
        rightControl.maxValue = maxRotation * 2;
        
        rightControl.value = maxRotation;
        leftControl.value = maxRotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetCurrentRotationX() 
    {
        Debug.Log("Rotation:" +  leftControl.value);
        return leftControl.value;
    }
}
