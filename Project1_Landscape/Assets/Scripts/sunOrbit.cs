using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sunOrbit : MonoBehaviour
{
    public float CycleSpeed = 15.0f;

    void Update()
    {
        Vector3 origin = new Vector3(0.0f, 0.0f, 0.0f);

        // Object rotates around in an orbit. LookAt is used since this will be
        // applied to a direcitonal light
        transform.LookAt(origin);
        transform.RotateAround(origin, Vector3.right, Time.deltaTime * CycleSpeed);
    }

    public void changeOrbit(float NewCycle)
    {
        CycleSpeed = NewCycle;
    }
}
