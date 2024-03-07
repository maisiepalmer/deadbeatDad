using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ORIGINAL SCRIPT
- Makes player movement and the camera follow the mouse position.
*/
public class MouseController : MonoBehaviour
{ 
    public float lookSpeed = 10.0f;
    private Vector3 rotation;
    private bool canMove = true;

    public float forward = 0f;

    public Transform playerBody;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (canMove)
        {
            // move 360 direction relative to mouse
            rotation.y += Input.GetAxis("Mouse X");
            rotation.x -= Input.GetAxis("Mouse Y");
            playerBody.eulerAngles = (Vector2)rotation * lookSpeed;
        }
        else
        {
            // move 360 direction relative to mouse
            rotation.y = forward;
            rotation.x = 0;
            playerBody.eulerAngles = (Vector2)rotation;
        }
    }

    public void CanMove(bool state)
    {
        canMove = state;
    }

    public void SetForward(float val)
    {
        forward = val;
    }
}