using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ExitRoom : MonoBehaviour
{
    public GameObject exitDoor;
    
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 5))
            {
                if (hit.collider.CompareTag("Door"))
                {
                    SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
                }
            }
        }
    }
}
