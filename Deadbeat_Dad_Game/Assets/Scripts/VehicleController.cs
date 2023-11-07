using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    private Rigidbody controller;
    public float speed = 0f;
    private Vector3 initPos;

    private void Start()
    {
        controller = GetComponent<Rigidbody>();
        initPos = gameObject.transform.position;
    }

    void Update()
    {
        Vector3 move = controller.transform.forward.normalized;
        move.y = 0;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 20))
        {
            if(hit.collider.CompareTag("Vehicle"))
            {
                gameObject.transform.Translate(Vector3.zero, Space.World);
            }
            else
            {
                gameObject.transform.Translate(move * speed, Space.World);
            }
        }
        else
        {
            gameObject.transform.Translate(move * speed, Space.World);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("EndOfRoad"))
            Respawn();
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Vehicle"))
        {
            Respawn();
        }
        else if (collision.collider.CompareTag("Intersection"))
        {
            WaitForAllClear();
        }
    }

    private void Respawn()
    {
        // randomly pick from a table of road starts
        transform.position = initPos;
        controller.velocity = Vector3.zero;
    }

    private void WaitForAllClear()
    {
        
    }
}
