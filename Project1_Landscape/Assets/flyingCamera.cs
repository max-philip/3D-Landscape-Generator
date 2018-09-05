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

    private float minX = -62.0f;
    private float minZ = -62.0f;
    private float maxX = 62.0f;
    private float maxZ = 62.0f;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        checkBoundary();
    }


    void FixedUpdate()
    {

        // take in mouse input
        rotateX += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        rotateY += Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        rotateY = Mathf.Clamp(rotateY, -90, 90);

        rb.rotation = Quaternion.AngleAxis(rotateX, Vector3.up);
        rb.rotation *= Quaternion.AngleAxis(rotateY, Vector3.left);

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

        // UP and DOWN
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

        if (rb.position.x < minX)
        {
            this.transform.position = new Vector3(minX, currY, currZ);
        }
        if (rb.position.x > maxX)
        {
            Vector3 temp = new Vector3(7.0f, 0, 0);
            this.transform.position = new Vector3(maxX, currY, currZ);
        }

        if (rb.position.z < minZ)
        {
            Vector3 temp = new Vector3(7.0f, 0, 0);
            this.transform.position = new Vector3(currX, currY, minZ);
        }
        if (rb.position.z > maxZ)
        {
            Vector3 temp = new Vector3(7.0f, 0, 0);
            this.transform.position = new Vector3(currX, currY, maxZ);
        }
    }

}