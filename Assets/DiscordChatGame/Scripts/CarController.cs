using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {

    //public List<AxleInfo> axleInfos;
    public float maxMotorTorque = 1f;
    public float maxSteeringAngle = 1f;
    public bool rotateOnly = false;
    public float xRotation = 0f;
    public float yRotation = 0f;
    public float zRotation = 0f;

    public WheelCollider wc;
    public GameObject wheel;

    private float currentSpeed = 0f;

    // Use this for initialization
    void Start () {
		
	}

    public void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        wc.steerAngle = steering;
        wc.motorTorque = motor;

        Vector3 position;
        Quaternion rotation;
        wc.GetWorldPose(out position, out rotation);
        if(rotateOnly)
        {
            rotation.eulerAngles = new Vector3(wc.rpm, 0f, 0f);
            wheel.transform.localRotation = rotation;
        } else
        {
            rotation.eulerAngles = new Vector3(wc.rpm, -Mathf.Clamp(steering, -maxSteeringAngle, maxSteeringAngle), 0f);
            wheel.transform.localRotation = rotation;
        }

        
    }
}
