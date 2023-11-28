using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ExitRoom : MonoBehaviour
{
    public GameObject exitDoor;

    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

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
                if (hit.collider.name == exitDoor.name)
                {
                    Debug.Log(hit.collider.name);

                    if (SceneManager.GetActiveScene().name == "Pub")
                    {
                        SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);
                    }
                }
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            Debug.Log("hello");
        }
    }
}
