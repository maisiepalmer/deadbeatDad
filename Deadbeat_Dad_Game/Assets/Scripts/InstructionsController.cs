using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ORIGINAL SCRIPT
- Makes the instruction text appear above character's heads and on doors when the player enters their trigger zone.
*/
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
            instructions.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            instructions.SetActive(false);
    }
}
