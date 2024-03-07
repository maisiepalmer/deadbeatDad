using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

/* ORIGINAL SCRIPT
- Controls the player transform object as scenes are changed.
*/
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

    //FMOD---------------------------------------------------------
    public FMODUnity.EventReference CrashEvent;
    FMOD.Studio.EventInstance crash;
    //-------------------------------------------------------------
    
    void OnEnable()
    {
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
             if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 3))
            {
                if (SceneManager.GetActiveScene().name != "MainScene")
                {
                    if (hit.collider.name == exitDoor.name)
                    {
                        if (!stateHandler.GetExpositionComplete())
                        {
                            stateHandler.SetInnerMonologue("I feel like I need to speak to the bartender...");
                        }
                        else 
                        {
                            stateHandler.PlayClick();
                            SceneManager.LoadSceneAsync("MainScene");
                        }     
                    }  
                }
                else
                {
                    if (hit.collider.CompareTag("FastFood"))
                    {
                        if (!stateHandler.GetHasFood())
                        {
                            stateHandler.PlayClick();
                            SceneManager.LoadSceneAsync("FastFood");
                        }
                        else
                        {
                            stateHandler.SetInnerMonologue("I've been here already... I should go somewhere else");
                            stateHandler.TimePenalty();
                        }
                    }
                    else if (hit.collider.CompareTag("Pub"))
                    {
                        stateHandler.SetInnerMonologue("I really shouldn't... I've got to stay on task!");
                        stateHandler.TimePenalty();
                    }
                    else if (hit.collider.CompareTag("Building"))
                    {
                        stateHandler.SetInnerMonologue("I don't have time to go in here...");
                        stateHandler.TimePenalty();
                    }
                }
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetConnections();

        if (scene.name == "MainScene")
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Terrain", 0);

            if (prevScene == "Pub")
            {
                player.transform.SetPositionAndRotation(loadVector[1].transform.position, loadVector[1].transform.rotation);
                player.GetComponent<PlayerController>().SetMouseForward(20f);
                prevScene = "MainScene";
                
                stateHandler.StartTimer();
                arrowController.SetTarget("FastFood");

                stateHandler.SetTasksVisible(true);
                arrowController.SetVisible(true);
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
                
                stateHandler.SetTasksVisible(true);
                arrowController.SetVisible(true);
            }
            else
            {
                stateHandler.SetTasksVisible(true);
                arrowController.SetVisible(true);
            }
        }
        else if (scene.name == "FastFood")
        {
            player.transform.SetPositionAndRotation(loadVector[2].transform.position, loadVector[2].transform.rotation);
            player.GetComponent<PlayerController>().SetMouseForward(20f);
            prevScene = "FastFood";

            stateHandler.SetTasksVisible(false);
            exitDoor = GameObject.FindWithTag("ExitDoor");

            arrowController.SetVisible(false);
        }
        else if (scene.name == "Pub")
        {
            stateHandler.Reset();
            stateHandler.IncTasks();

            DontDestroyOnLoad(player);
            DontDestroyOnLoad(loadPlaces);
            DontDestroyOnLoad(GameObject.FindWithTag("Cursor"));
            
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

                if (reasonText.text == "You were hit by a car!")
                    FMODUnity.RuntimeManager.PlayOneShotAttached(CrashEvent,  GameObject.Find("Main Camera"));
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            stateHandler.DestroyItAll();
        }
    }

    private void SetConnections()
    {
        if(!stateHandler)
            stateHandler = GameObject.FindWithTag("State").GetComponent<StateHandler>();
    }
}