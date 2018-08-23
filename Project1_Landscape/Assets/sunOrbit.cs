using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sunOrbit : MonoBehaviour {

    public float CycleSpeed = 15f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(Vector3.zero, Vector3.right, CycleSpeed * Time.deltaTime);
        transform.LookAt(Vector3.zero);
	}
}
