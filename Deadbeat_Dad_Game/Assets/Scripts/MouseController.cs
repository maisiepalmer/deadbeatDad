using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* My own code to make movement and camera follow the mouse */
public class MouseController : MonoBehaviour
{ 
    public float lookSpeed = 10.0f;
    private Vector3 rotation;
    private bool mouseLocked = true;

    public Transform playerBody;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (mouseLocked)
        {
            // move 360 direction relative to mouse
            rotation.y += Input.GetAxis("Mouse X");
            rotation.x -= Input.GetAxis("Mouse Y");
            playerBody.eulerAngles = (Vector2)rotation * lookSpeed;
        }
        else
        {
            // move 360 direction relative to mouse
            rotation.y = 0;
            rotation.x = 0;
            playerBody.eulerAngles = (Vector2)rotation;
        }
    }

    public void SetMouseLock(bool b)
    {
        mouseLocked = b;
        Cursor.visible = !b;
    }
}