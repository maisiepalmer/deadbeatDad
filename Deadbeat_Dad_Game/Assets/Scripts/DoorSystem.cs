using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class DoorSystem : MonoBehaviour
{
    public GameObject exitDoor;

    public GameObject player;
    public GameObject loadPlaces;
    public Transform[] loadVector;
    public StateHandler stateHandler;
    public ArrowController arrowController;

    private string prevScene = "Start";

    public TextMeshProUGUI reasonText;
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        stateHandler = GameObject.FindWithTag("State").GetComponent<StateHandler>();
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
                        if (!stateHandler.GetExpositionComplete())
                            stateHandler.SetInnerMonologue("I feel like I need to speak to the bartender...");
                        else 
                            SceneManager.LoadSceneAsync("MainScene");
                    }  
                }
                else
                {
                    if (hit.collider.CompareTag("FastFood"))
                    {
                        SceneManager.LoadSceneAsync("FastFood");
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
            stateHandler.SetTasksVisible(true);
            arrowController.SetVisible(true);

            if (prevScene == "Pub")
            {
                SetConnections();

                player.transform.SetPositionAndRotation(loadVector[1].transform.position, loadVector[1].transform.rotation);
                prevScene = "MainScene";
                
                stateHandler.StartTimer();
                arrowController.SetTarget("FastFood");
            }    
            else if (prevScene == "FastFood")
            {
                player.transform.SetPositionAndRotation(loadVector[3].transform.position, loadVector[3].transform.rotation);
                prevScene = "MainScene";

                if (stateHandler.GetHasFood()) // arrow director pointing
                {
                    if (stateHandler.GetHasPresent())
                        arrowController.SetTarget("Wife");
                    else
                        arrowController.SetTarget("Sally");
                }
                else
                {
                    arrowController.SetTarget("FastFood");
                }
                
            }
        }
        else if (scene.name == "FastFood")
        {
            if (!stateHandler.GetHasFood())
            {
                player.transform.SetPositionAndRotation(loadVector[2].transform.position, loadVector[2].transform.rotation);
                prevScene = "FastFood";

                stateHandler.SetTasksVisible(false);
                exitDoor = GameObject.FindWithTag("ExitDoor");

                arrowController.SetVisible(false);
            }
            else
            {
                Debug.Log("I've been here already... I should go somewhere else");
                stateHandler.TimePenalty();
            } 
        }
        else if (scene.name == "Pub")
        {
            SetConnections();

            player.transform.SetPositionAndRotation(loadVector[4].transform.position, loadVector[4].transform.rotation);
            player.GetComponent<PlayerController>().LockMovement(false);
            prevScene = "Pub";
            exitDoor = GameObject.FindWithTag("ExitDoor");
            arrowController.SetVisible(false);
        }
        else if (scene.name == "GameOver" || scene.name == "Divorce" || scene.name == "YouWin")
        {
            if (scene.name == "GameOver")
            {
                GameObject text = GameObject.Find("ReasonText");
                reasonText = text.GetComponent<TextMeshProUGUI>();
                reasonText.text = stateHandler.GetReason();
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            stateHandler.DestroyItAll();
        }
    }

    private void SetConnections()
    {
        player = GameObject.FindWithTag("Player");
        loadPlaces = GameObject.FindWithTag("LoadPlaces");
        loadVector = loadPlaces.GetComponentsInChildren<Transform>();

        if(!stateHandler)
            stateHandler = GameObject.FindWithTag("State").GetComponent<StateHandler>();

        stateHandler.Reset();

        arrowController = GameObject.FindWithTag("Arrow").GetComponent<ArrowController>();

        DontDestroyOnLoad(player);
        DontDestroyOnLoad(loadPlaces);
        DontDestroyOnLoad(GameObject.FindWithTag("Cursor"));
    }
}