using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        {
            stateHandler.MeetWife();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isEntered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isEntered = false;
    }
}
