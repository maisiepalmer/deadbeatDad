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
    public StateHandler stateHandler;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;

    public FMODUnity.EventReference FootstepsEvent;
    public FMODUnity.EventReference EffortsEvent;
    float timer = 0.0f;
    [SerializeField]
    float footstepSpeed = 0.5f;

    [HideInInspector]
    public bool canMove = true;
    private bool moveable = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        stateHandler = GameObject.FindWithTag("State").GetComponent<StateHandler>();
        mouse.SetForward(77.0f);
    }

    /* ADAPTED FROM: https://www.sharpcoderblog.com/blog/unity-3d-fps-controller 
    With alterations:
    - Unneded code blocks removed
    - Neatened and tidied up
    - Extra parameters added where needed
    - FMOD integration
    */
    void Update()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        footstepSpeed = isRunning ? 0.3f : 0.5f;

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

            // FMOD - play footstep
            if(characterController.velocity != Vector3.zero)
            {
                if (characterController.isGrounded)
                {
                    if (timer > footstepSpeed)
                    {
                        PlayFootstep();
                        timer = 0.0f;
                    }

                    timer += Time.deltaTime;
                }
                else
                {
                    Scene scene = SceneManager.GetActiveScene();
                    if (!(scene.name == "GameOver" || scene.name == "Divorce" || scene.name == "YouWin"))
                        PlayEffort();
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Vehicle"))
        {
            stateHandler.SetReason("You were hit by a car!");
            stateHandler.GameOver();
        }   
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

    public void SetMouseForward(float x)
    {
        mouse.SetForward(x);
    }

    private void PlayFootstep() 
    {
        FMOD.Studio.EventInstance foosteps = FMODUnity.RuntimeManager.CreateInstance(FootstepsEvent);
        foosteps.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        foosteps.start();
        foosteps.release();
    }

    private void PlayEffort() 
    {
        FMOD.Studio.EventInstance effort = FMODUnity.RuntimeManager.CreateInstance(EffortsEvent);
        effort.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        effort.start();
        effort.release();
    }
}
