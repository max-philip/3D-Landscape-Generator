using UnityEngine;
using System.Collections;

public class flyingCamera : MonoBehaviour
{
    public float speed = 8;
    public float shiftMulti = 2;
    public float sensitivity = 90;

    public float FOV = 60.0f;
    public Camera myCamera;

    private Rigidbody rb;

    private float rotateX = 0.0f;
    private float rotateY = 0.0f;

    // Terrain boundary values
    private float minX = -61.5f;
    private float minZ = -61.5f;
    private float maxX = 61.5f;
    private float maxZ = 61.5f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        myCamera.fieldOfView = FOV;
    }

    private void Update()
    {
        checkBoundary();
        myCamera.fieldOfView = FOV;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            } else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }


    void FixedUpdate()
    {
        // Mouse input
        rotateX += Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;
        rotateY += Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;

        // Bound on y axis of camera control
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

        // Keyboard input
        float mH = Input.GetAxis("Horizontal");
        float mV = Input.GetAxis("Vertical");
        float mUD = getUpDown();

        rb.velocity = new Vector3 (mH, mUD, mV) * outSpeed;
        rb.velocity = rb.rotation * rb.velocity;

        checkBoundary();
    }

    private float getUpDown()
    {
        float upDownVal = 0.0f;

        // Up and Down control. Makes movement in general a bit smoother
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

    // Discrete method of keeping the user within the terrain boundaries
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

    public void changeFOV(float newFOV)
    {
        FOV = newFOV;
    }

    public void changeSensitivity(float newSensitivity)
    {
        sensitivity = newSensitivity;
    }
}