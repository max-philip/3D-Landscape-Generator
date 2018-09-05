using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sunOrbit : MonoBehaviour {

    public float CycleSpeed = 15f;
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(Vector3.zero, Vector3.right, CycleSpeed * Time.deltaTime);
        transform.LookAt(Vector3.zero);

        // Dunno ABOUT THIS
        // Shader.SetGlobalVector("_SunPosition", transform.position);
    }
}
