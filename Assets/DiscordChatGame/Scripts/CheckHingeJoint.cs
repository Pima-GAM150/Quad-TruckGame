using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckHingeJoint : MonoBehaviour {

    public HingeJoint hj;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

    void OnJointBreak(float breakforce)
    {
        Debug.Log("Joint broke!");
        hj.gameObject.transform.parent = null;
    }
}
