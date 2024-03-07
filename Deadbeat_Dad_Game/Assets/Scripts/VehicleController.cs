using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

/* ORIGINAL SCRIPT
- Moves the vehicles forward along a straight path.
- Detects crashes and respawns in dedicated respawn points.
- Attempts to yield for other vehicles using raycasters.
*/
public class VehicleController : MonoBehaviour
{
    private Rigidbody controller;
    public float speed = 0f;
    public GameObject[] respawnPoints;
    public GameObject crashObject;

    //FMOD---------------------------------------------------------
        public FMODUnity.EventReference CrashEvent;
        FMOD.Studio.EventInstance crash;
        public int size = 0;
    //-------------------------------------------------------------

    private void Start()
    {
        controller = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 frameMovement = CheckAndGetMovement();
        gameObject.transform.Translate(frameMovement, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EndOfRoad"))
            Respawn();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Vehicle"))
        {
            Vector3 collisionPoint = collision.GetContact(0).point;
            GameObject copy = Instantiate(crashObject, collisionPoint, new Quaternion(0, 0, 0, 1));
            Respawn();
            Destroy(copy, 5.0f);
        }
    }

    private void Respawn()
    {
        // randomly pick from a table of road starts
        int randZone = UnityEngine.Random.Range(0, respawnPoints.Length);

        transform.position = respawnPoints[randZone].transform.position;
        transform.rotation = respawnPoints[randZone].transform.rotation;

        controller.velocity = Vector3.zero;
    }

    private Vector3 CheckAndGetMovement()
    {
        // calculate the movement translation (forward vector multiplied by the speed offset)
        Vector3 movement = controller.transform.forward.normalized * speed * Time.deltaTime;
        movement.y = 0;

        /* If the raycast hits a collider
         - Check if it's a vehicle, if so return a zero vector (no movement)
         - Otherwise, return the movement translation */
        RaycastHit hit;
        return (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 20)) ?
                                                                                                            (hit.collider.CompareTag("Vehicle")) ? Vector3.zero : movement
                                                                                                                 : movement;
    }
}
