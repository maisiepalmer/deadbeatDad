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
    public StateHandler stateHandler;

    private string prevScene = "Pub";
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        DontDestroyOnLoad(loadPlaces);
        loadVector = loadPlaces.GetComponentsInChildren<Transform>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            RaycastHit hit;
             if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 5))
            {
                if (SceneManager.GetActiveScene().name != "MainScene")
                {
                    if (hit.collider.name == exitDoor.name)
                    {
                        SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);
                    }
                }
                else
                {
                    if (hit.collider.CompareTag("FastFood"))
                    {
                        SceneManager.LoadSceneAsync("FastFood", LoadSceneMode.Single);
                    }
                    else if (hit.collider.CompareTag("Jewellery"))
                    {
                        SceneManager.LoadSceneAsync("Jewellery", LoadSceneMode.Single);
                    }
                    else if (hit.collider.CompareTag("Pub"))
                    {
                        Debug.Log("I really shouldn't... I've got to stay on task!");
                        stateHandler.TimePenalty();
                    }
                    else if (hit.collider.CompareTag("Building"))
                    {
                        Debug.Log("I don't have time for this...");
                        stateHandler.TimePenalty();
                    }
                }
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            if (prevScene == "Pub")
            {
                Debug.Log("out of pub");
                playerTransform.transform.SetPositionAndRotation(loadVector[1].transform.position, loadVector[1].transform.rotation);
                prevScene = "MainScene";
                
                stateHandler.StartTimer();
            }    
            else if (prevScene == "FastFood")
            {
                Debug.Log("out of fast food");
                playerTransform.transform.SetPositionAndRotation(loadVector[3].transform.position, loadVector[3].transform.rotation);
                prevScene = "MainScene";
            }   
            else if (prevScene == "Jewellery")
            {
                Debug.Log("out of jewellery");
                playerTransform.transform.SetPositionAndRotation(loadVector[5].transform.position, loadVector[5].transform.rotation);
                prevScene = "MainScene";
            } 

            stateHandler.SetTasksVisible(true);
        }
        else if (scene.name == "FastFood")
        {
            if (!stateHandler.GetHasFood())
            {
                Debug.Log("in fast food");
                playerTransform.transform.SetPositionAndRotation(loadVector[2].transform.position, loadVector[2].transform.rotation);
                prevScene = "FastFood";

                stateHandler.SetTasksVisible(false);
                exitDoor = GameObject.FindWithTag("ExitDoor");
            }
            else
            {
                Debug.Log("I've been here already... I should go somewhere else");
                stateHandler.TimePenalty();
            }
           
        }
        else if (scene.name == "Jewellery")
        {
            if (!stateHandler.GetHasPresent())
            {
                Debug.Log("in jewellery");
                playerTransform.transform.SetPositionAndRotation(loadVector[4].transform.position, loadVector[4].transform.rotation);
                prevScene = "Jewellery";

                stateHandler.SetTasksVisible(false);
                exitDoor = GameObject.FindWithTag("ExitDoor");
                playerTransform.gameObject.GetComponent<PlayerController>().mouse.SetForward(-180f);
            }
            else
            {
                Debug.Log("I've been here already... I should go somewhere else");
                stateHandler.TimePenalty();
            }
        }
        else if (scene.name == "Pub")
        {
            playerTransform.transform.SetPositionAndRotation(loadVector[6].transform.position, loadVector[6].transform.rotation);
            playerTransform.gameObject.GetComponent<PlayerController>().LockMovement(true);
        }
    }
}