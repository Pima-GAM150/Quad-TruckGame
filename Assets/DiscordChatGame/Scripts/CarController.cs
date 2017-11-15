using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {

    //public List<AxleInfo> axleInfos;
    public float maxMotorTorque = 1f;
    public float maxSteeringAngle = 1f;

    public WheelCollider wc;
    public GameObject wheel;

    // Use this for initialization
    void Start () {
		
	}

    public void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        wc.steerAngle = steering;

        Vector3 position;
        Quaternion rotation;
        wc.GetWorldPose(out position, out rotation);
        //rotation.z = 90f;
        //rotation.w = 0f;
        rotation.y = Mathf.Clamp(rotation.y, -maxSteeringAngle, maxSteeringAngle);
        //wheel.transform.rotation = rotation;
        rotation.eulerAngles = new Vector3(0f, Mathf.Clamp(steering, -maxSteeringAngle, maxSteeringAngle), 0f);
        wheel.transform.rotation = rotation;
        //wheel.transform.rotation = new Quaternion(wheel.transform.rotation.x, Mathf.Clamp(steering, -maxSteeringAngle, maxSteeringAngle), wheel.transform.rotation.z, wheel.transform.rotation.w);
        Debug.Log(wc.steerAngle + ", " + rotation);

        //foreach (AxleInfo axleInfo in axleInfos)
        //{
        //    if (axleInfo.steering)
        //    {
        //        axleInfo.leftWheel.steerAngle = steering;
        //        axleInfo.rightWheel.steerAngle = steering;
        //    }
        //    if (axleInfo.motor)
        //    {
        //        axleInfo.leftWheel.motorTorque = motor;
        //        axleInfo.rightWheel.motorTorque = motor;
        //    }
        //}
    }
}
