using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sunOrbit : MonoBehaviour {

    private float dayCycleSpeed = 15f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(Vector3.zero, Vector3.right, dayCycleSpeed * Time.deltaTime);
        transform.LookAt(Vector3.zero);
	}
}
