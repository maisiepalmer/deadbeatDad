using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/* ADAPTED FROM: https://www.sharpcoderblog.com/blog/unity-3d-fps-controller 
    With alterations:
    - Own script to control mousemovement relative to the camera
    - Unneded code blocks removed
    - Neatened and tidied up
*/
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    private float gravity = 20.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;

        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        moveDirection.y = (Input.GetButton("Jump") && canMove && characterController.isGrounded) ? jumpSpeed : movementDirectionY;

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        /* ADDED: My own code to allow player to enter necessary buildings */
        if (Input.GetKey(KeyCode.Mouse0))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10))
            {
                if (hit.collider.CompareTag("FastFood"))
                {
                    Debug.Log("Enter this building");
                    SceneManager.LoadScene("FastFood", LoadSceneMode.Single);
                    
                }
                else if (hit.collider.CompareTag("Building"))
                {
                    Debug.Log("Don't get off task! You have ... minutes remaining!");
                }
            }
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Vehicle"))
        {
            // GameOver()
        }
    }
}
