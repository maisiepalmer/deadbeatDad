using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ORIGINAL SCRIPT
- Prepares the state handler for when the user will interract with the wife.
*/
public class WifeDialogue : MonoBehaviour
{
    public StateHandler stateHandler;
    bool isEntered = false;

    void Start()
    {
        GameObject handler = GameObject.Find("StateHandler");
        stateHandler = handler.GetComponent<StateHandler>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isEntered)
            stateHandler.MeetWife();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isEntered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isEntered = false;
    }
}
