﻿using UnityEngine;
using System.Collections;

public class flyingCamera : MonoBehaviour
{
    public float speed;
    public float sensitivity;
    public float shiftMulti;

    private Vector3 prevMouse;

    private Rigidbody rb;

    private float rotateX = 0.0f;
    private float rotateY = 0.0f;


    void Start()
    {
        Screen.lockCursor = true;
        rb = GetComponent<Rigidbody>();
    }


    void Update()
    {

        // take in mouse input and CHANGE CAMERA PERSPECTIVE
        rotateX += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        rotateY += Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        rotateY = Mathf.Clamp(rotateY, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(rotateX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rotateY, Vector3.left);

        // take keyboard input and update position
        Vector3 pos = keyboardDir();



        if (Input.GetKey(KeyCode.LeftShift))
        {
            pos = pos * Time.deltaTime * speed * shiftMulti;
        }
        else
        {
            pos = pos * Time.deltaTime * speed;
        }

        transform.Translate(pos);
        //rb.AddForce(pos*speed);

        //rb.MovePosition(transform.position + pos* (speed * Time.deltaTime));


    }

    // take in keyboard input (both arrows and WASD)
    private Vector3 keyboardDir()
    {

        // ARROWS
        //Vector3 p_Velocity = new Vector3();

        float moveH = Input.GetAxis("Horizontal");
        float moveV = Input.GetAxis("Vertical");
        Vector3 p_Velocity = new Vector3(moveH, 0.0f, moveV);


        // UP and DOWN
        if (Input.GetKey(KeyCode.C))
        {
            p_Velocity += new Vector3(0, 1, 0);
        }

        if (Input.GetKey(KeyCode.Z))
        {
            p_Velocity += new Vector3(0, -1, 0);
        }

        return p_Velocity;
    }

}