using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    private float gravity = 20.0f;
    public MouseController mouse;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;

    [HideInInspector]
    public bool canMove = true;
    private bool moveable = true;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        characterController = GetComponent<CharacterController>();

        mouse.SetForward(33.0f);
    }

    /* ADAPTED FROM: https://www.sharpcoderblog.com/blog/unity-3d-fps-controller 
    With alterations:
    - Unneded code blocks removed
    - Neatened and tidied up
    - Extra parameters added where needed
    */
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

        if (!moveable)
        {
            // We are grounded, so recalculate move direction based on axes
            moveDirection = Vector3.zero;

            // Move the controller
            characterController.Move(moveDirection);
        }
        else
        {
            // Move the controller
            characterController.Move(moveDirection * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Vehicle"))
            SceneManager.LoadSceneAsync("GameOver");
            // play sound ?
            // send in text
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    public void LockMovement(bool state)
    {
        canMove = !state;
        moveable = !state;
        mouse.CanMove(!state);
    }
}
