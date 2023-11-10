using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* My own code to make movement and camera follow the mouse */
public class MouseController : MonoBehaviour
{ 
    public float lookSpeed = 2.0f;
    private Vector3 rotation;

    public Transform playerBody;

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // move 360 direction relative to mouse
        rotation.y += Input.GetAxis("Mouse X");
        rotation.x -= Input.GetAxis("Mouse Y");
        playerBody.eulerAngles = (Vector2)rotation * lookSpeed;
    }
}
