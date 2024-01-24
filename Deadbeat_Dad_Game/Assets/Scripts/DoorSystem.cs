using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEditor;

public class DoorSystem : MonoBehaviour
{
    public GameObject exitDoor;
    public Transform playerTransform;

    public GameObject loadPlaces;
    public Transform[] loadVector;

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
            if (prevScene == "Pub")
            {
                Debug.Log("out of pub");
                playerTransform.transform.SetPositionAndRotation(loadVector[1].transform.position, loadVector[1].transform.rotation);
                prevScene = "MainScene";
            }    
            else if (prevScene == "FastFood")
            {
                Debug.Log("out of fast food");
                playerTransform.transform.SetPositionAndRotation(loadVector[3].transform.position, loadVector[3].transform.rotation);
                prevScene = "MainScene";
            }   
        }
        else if (scene.name == "FastFood")
        {
            Debug.Log("in fast food");
            playerTransform.transform.SetPositionAndRotation(loadVector[2].transform.position, loadVector[2].transform.rotation);
            prevScene = "FastFood";
            exitDoor = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Packages/Brick Project Studio/Fast Food Restaurant Kit/_Prefabs/Fast Food Build Kit/Int_Ext/ExtInt_FFK_Entrance_02_01.prefab");
        }
    }
}