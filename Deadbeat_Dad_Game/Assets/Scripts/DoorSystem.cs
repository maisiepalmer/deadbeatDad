using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DoorSystem : MonoBehaviour
{
    public GameObject exitDoor;
    public Transform playerTransform;

    public GameObject loadPlaces;
    public Transform[] loadVector;
    
    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        DontDestroyOnLoad(loadPlaces);
        loadVector = loadPlaces.GetComponentsInChildren<Transform>();
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "MainScene")
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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            Debug.Log("hello");
            playerTransform.transform.SetPositionAndRotation(loadVector[1].transform.position, loadVector[1].transform.rotation);
            Debug.Log(playerTransform.eulerAngles);
        }
    }
}