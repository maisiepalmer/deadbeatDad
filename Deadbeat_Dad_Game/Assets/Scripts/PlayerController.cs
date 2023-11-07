using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody playerController;

    Vector3 rotation = Vector3.zero;
    public float lookSpeed = 10f;

    private Vector3 lastValue;

    void Start()
    {
        playerController = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // move 360 direction relative to mouse
        rotation.y += Input.GetAxis("Mouse X");
        rotation.x -= Input.GetAxis("Mouse Y");
        transform.eulerAngles = (Vector2)rotation * lookSpeed;
    }

    private void FixedUpdate()
    {
        if (Keyboard.KeyCount > 0)
            MovePlayerRelativeToCamera();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided");
    }

    // ADAPTED FROM: https://youtu.be/7kGCrq1cJew?si=hDGWI_3S8jt2mnoo
    private void MovePlayerRelativeToCamera()
    {
        // get player input
        float playerVerticalInput = Input.GetAxis("Vertical");
        float playerHorizontalInput = Input.GetAxis("Horizontal");

        // get camera-normalised directional vectors
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        // create direction-relative input vectors
        Vector3 forwardVerticalInput = playerVerticalInput * forward;
        Vector3 rightVerticalInput = playerHorizontalInput * right;

        // create camera-relative movement
        Vector3 cameraRelativeMovement = (forwardVerticalInput + rightVerticalInput) * moveSpeed;

        // apply to player with smoothing
        playerController.velocity = (cameraRelativeMovement * 0.9f) + (lastValue * 0.1f);
        lastValue = cameraRelativeMovement;
    }
}
