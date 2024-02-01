using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StateHandler : MonoBehaviour
{
    private bool isDrunk = false;
    private bool hasFood = false;
    private bool hasPresent  = false;

    public GameObject checklist;
    public GameObject[] crossOut;
    public TextMeshProUGUI time;

    private float timeRemaining = 300;
    private bool timerIsRunning = false;

//---------------------------------------------------------------------------------
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(checklist);

        for (int i = 0; i < 3; i++)
        {
            crossOut[i].SetActive(false);
        }

        SetTasksVisible(false);
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;

                float timeToDisplay = timeRemaining + 1;
                float minutes = Mathf.FloorToInt(timeRemaining / 60);
                float seconds = Mathf.FloorToInt(timeRemaining % 60);
                time.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
                // send in text
                GameOver();
            }
        }
    }

    public void Reset()
    {
        timeRemaining = 300;
        timerIsRunning = false;
        isDrunk = false;
        hasFood = false;
        hasPresent  = false;
    }

//---------------------------------------------------------------------------------
    public void StartTimer()
    {
        timerIsRunning = true;
    }

//---------------------------------------------------------------------------------
    public void IsDrunk()
    {
        isDrunk = true;
        // penalty
    }

    public void HasFood()
    {
        hasFood = true;
        crossOut[0].SetActive(true);
    }

    public bool GetHasFood()
    {
        return hasFood;
    }

    public void HasPresent()
    {
        hasPresent = true;
        crossOut[1].SetActive(true);
    }

    public bool GetHasPresent()
    {
        return hasPresent;
    }

    public void MeetWife()
    {
        if (hasFood && hasPresent)
        {
            crossOut[2].SetActive(true);
            YouWin();
        }
        else
        {
            Divorce();
        }
    }

//---------------------------------------------------------------------------------
    public void SetTasksVisible(bool visible)
    {
        checklist.SetActive(visible);
    }

    public void TimePenalty()
    {
        // set inner monologue
        timeRemaining -= 10;
        Debug.Log("I shouldn't have done that...");
    }

//---------------------------------------------------------------------------------
    public void GameOver()
    {
        SceneManager.LoadSceneAsync("GameOver");
    }

    public void YouWin()
    {
        SceneManager.LoadSceneAsync("Win");
    }

    public void Divorce()
    {
        SceneManager.LoadSceneAsync("Divorce");
    }
}
