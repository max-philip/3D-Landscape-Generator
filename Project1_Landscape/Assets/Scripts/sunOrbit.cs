using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sunOrbit : MonoBehaviour
{

    public float cycleSpeed = 20.0f;

    // Update is called once per frame
    void Update()
    {
        Vector3 origin = new Vector3(0.0f, 0.0f, 0.0f);

        transform.RotateAround(origin, Vector3.right, cycleSpeed * Time.deltaTime);
        transform.LookAt(origin);
    }

    public void changeOrbit(float newCycle)
    {
        cycleSpeed = newCycle;
    }
}
