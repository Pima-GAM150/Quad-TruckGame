using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMirror : MonoBehaviour {

    public Camera main;
    public GameObject mirror;
    public bool leftSide;
    public float x = 0f;
    public float y = 0f;
    public float z = 0f;

    private float cameraPixelWidth;
    private Vector3 mirrorPosition;

	// Use this for initialization
	void Start () {
        cameraPixelWidth = main.rect.width;
        Debug.Log(cameraPixelWidth);

        mirrorPosition = main.WorldToViewportPoint(transform.position);
        //Debug.Log(main.WorldToViewportPoint(transform.position));

        //var v3Pos = new Vector3(0.0f, 0.0f, -0.25f);
        //Debug.Log(main.ViewportToWorldPoint(v3Pos));
        //transform.position = main.ViewportToWorldPoint(v3Pos);

    }
	
	// Update is called once per frame
	void Update () {
        Debug.Log(main.WorldToViewportPoint(transform.position));
        if(leftSide)
        {
            transform.position = main.ViewportToWorldPoint(new Vector3(0.0f, mirrorPosition.y, mirrorPosition.z));
            transform.position = new Vector3(transform.position.x + (gameObject.GetComponent<MeshRenderer>().bounds.size.x / 2), transform.position.y, transform.position.z);
        } else
        {
            transform.position = main.ViewportToWorldPoint(new Vector3(1.0f, mirrorPosition.y, mirrorPosition.z));
            transform.position = new Vector3(transform.position.x - (gameObject.GetComponent<MeshRenderer>().bounds.size.x / 2), transform.position.y, transform.position.z);
        }
        
        
        //Vector3 v3Pos = new Vector3(x, y, z);
        //transform.localPosition = main.ScreenToWorldPoint(v3Pos);


    }
}
