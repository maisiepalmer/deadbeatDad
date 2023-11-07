using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    private Rigidbody controller;
    public float speed = 0f;

    private void Start()
    {
        controller = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 move = controller.transform.forward.normalized;
        move.y = 0;
        gameObject.transform.Translate(move * speed, Space.World);
    }
}
