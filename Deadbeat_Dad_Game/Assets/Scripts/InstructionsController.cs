using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionsController : MonoBehaviour
{
    public GameObject instructions;

    void Start()
    {
        instructions.SetActive(false);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            instructions.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        instructions.SetActive(false);
    }
}
