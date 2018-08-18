using UnityEngine;
using System.Collections;

public class flyingCamera : MonoBehaviour
{
    public float speed;
    public float sensitivity;
    public float shiftMulti;

    private Vector3 prevMouse;

    void Update()
    {

        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;

        //Camera.main.fieldOfView = 60.0f;

        // take in mouse input and CHANGE CAMERA PERSPECTIVE
        prevMouse = Input.mousePosition - prevMouse;
        prevMouse = new Vector3(-prevMouse.y * sensitivity, prevMouse.x * sensitivity, 0);
        prevMouse = new Vector3(transform.eulerAngles.x + prevMouse.x, transform.eulerAngles.y + prevMouse.y, 0);
        transform.eulerAngles = prevMouse;
        prevMouse = Input.mousePosition;

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


    }

    // take in keyboard input (both arrows and WASD)
    private Vector3 keyboardDir()
    {

        // ARROWS
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            p_Velocity += new Vector3(1, 0, 0); 
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            p_Velocity += new Vector3(0, 0, 1);
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            p_Velocity += new Vector3(0, 0, -1);
        }
        
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