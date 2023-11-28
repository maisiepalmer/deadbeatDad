using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* ADDED: My own class to allow player to enter necessary buildings */
public class EnterBuilding : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10))
            {
                if (hit.collider.CompareTag("FastFood"))
                {
                    Debug.Log("Enter this building");
                    SceneManager.LoadScene("FastFood", LoadSceneMode.Single);
                    gameObject.transform.position = new Vector3(-1.7f, 1, 2.1f);
                }
                else if (hit.collider.CompareTag("Pub"))
                {
                    Debug.Log("I really shouldn't... I've got to stay on task!");
                    // TimePenalty();
                }
                else if (hit.collider.CompareTag("Building"))
                {
                    Debug.Log("I don't have time for this...");
                    // TimePenalty();
                }
            }
        }
    }
}
