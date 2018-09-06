using UnityEngine;
using System.Collections;

public class flyingCamera : MonoBehaviour
{
    public float speed = 8;
    public float sensitivity = 90;
    public float shiftMulti = 2;

    private Vector3 prevMouse;

    private Rigidbody rb;

    private float rotateX = 0.0f;
    private float rotateY = 0.0f;

    private float minX = -61.5f;
    private float minZ = -61.5f;
    private float maxX = 61.5f;
    private float maxZ = 61.5f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();

        Vector3 defaultPos = new Vector3(60.0f, 60.0f, -60.0f);

        // DEFAULT ROTATION NOT WORKING
        Quaternion defaultQuat = new Quaternion(45.0f, -50.0f, 0.0f, 0.0f);
        this.transform.SetPositionAndRotation(defaultPos, defaultQuat);
    }

    private void Update()
    {
        checkBoundary();
    }


    void FixedUpdate()
    {

        // Take in mouse input, and set limit on mouse rotation
        rotateX += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        rotateY += Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        rotateY = Mathf.Clamp(rotateY, -90, 90);

        // Apply rotation to the Rigidbody component
        rb.rotation = Quaternion.AngleAxis(rotateX, Vector3.up);
        rb.rotation *= Quaternion.AngleAxis(rotateY, Vector3.left);

        // Shift control for speed multiplier
        float outSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            outSpeed = speed * shiftMulti;
        }

        float mH = Input.GetAxis ("Horizontal");
        float mV = Input.GetAxis ("Vertical");
        float mUD = getUpDown();
        rb.velocity = new Vector3 (mH, mUD, mV) * outSpeed;
        rb.velocity = rb.rotation * rb.velocity;

        checkBoundary();
    }

    private float getUpDown()
    {
        float upDownVal = 0.0f;

        // UP and DOWN control
        if (Input.GetKey(KeyCode.C))
        {
            upDownVal = 1.0f;
        }

        else if (Input.GetKey(KeyCode.Z))
        {
            upDownVal = -1.0f;
        }
        return upDownVal;
    }

    private void checkBoundary()
    {
        float currX = this.transform.position.x;
        float currY = this.transform.position.y;
        float currZ = this.transform.position.z;

        // X boundary for the landscape
        if (rb.position.x < minX)
        {
            this.transform.position = new Vector3(minX, currY, currZ);
        }
        if (rb.position.x > maxX)
        {
            this.transform.position = new Vector3(maxX, currY, currZ);
        }
        if (rb.position.x < minX)
        {
            this.transform.position = new Vector3(minX, currY, currZ);
        }

        // Z boundary for the landscape
        if (rb.position.z < minZ)
        {
            this.transform.position = new Vector3(currX, currY, minZ);
        }
        if (rb.position.z > maxZ)
        {
            this.transform.position = new Vector3(currX, currY, maxZ);
        }
        if (rb.position.z < minZ)
        {
            this.transform.position = new Vector3(currX, currY, minZ);
        }
    }
}