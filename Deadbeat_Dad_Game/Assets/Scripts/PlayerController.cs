using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private float speed = 0.1f;
    private Rigidbody playerController;
    public Vector3 movement;

    private bool isCollided;

    void Start()
    {
        playerController = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
    }

    private void FixedUpdate()
    {
        playerController.MovePosition(transform.position + (movement * speed));
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided");
    }
}
