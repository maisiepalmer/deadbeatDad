using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/* ORIGINAL SCRIPT
- Sets and gets the player's progress to determine the outcome of the game.
- Handles the timer.
- Displays warning messages and progress on the main canvas.
*/
public class StateHandler : MonoBehaviour
{
    private bool hasFood = false;
    private bool hasPresent  = false;
    private bool expositionComplete = false;

    public GameObject checklist;
    public GameObject[] crossOut;
    public TextMeshProUGUI time;
    public WobbleEffect wobbleEffect;
    public ArrowController arrowController;

    public TextMeshProUGUI innerMonologue;

    private float timeRemaining = 300;
    private bool timerIsRunning = false;
    private string reason = "";
    bool running = false;

    int tasks = 0;

    //FMOD---------------------------------------------------------
    public FMODUnity.EventReference MusicEvent;
    FMOD.Studio.EventInstance music;
    FMOD.Studio.PARAMETER_ID tasksCompletedId, drunkId;

    //--------------------------------------------------------------------
    public FMODUnity.EventReference AtmosEvent;
    FMOD.Studio.EventInstance atmos;

    //--------------------------------------------------------------------
    public FMODUnity.EventReference SuccessEvent, FailureEvent, PenaltyEvent, ClickEvent;
    FMOD.Studio.EventInstance success, failure, penalty, click;
    //-------------------------------------------------------------

//---------------------------------------------------------------------------------
    void Start()
    {
        music = FMODUnity.RuntimeManager.CreateInstance(MusicEvent);
        music.start();

        FMOD.Studio.EventDescription musicEventDescription;
        music.getDescription(out musicEventDescription);
        FMOD.Studio.PARAMETER_DESCRIPTION tasksParameterDescription, drunkParameterDescription;

        musicEventDescription.getParameterDescriptionByName("TasksCompleted", out tasksParameterDescription);
        tasksCompletedId = tasksParameterDescription.id;

        musicEventDescription.getParameterDescriptionByName("Drunkness", out drunkParameterDescription);
        drunkId = drunkParameterDescription.id;

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(checklist);

        for (int i = 0; i < 2; i++)
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
                reason = "Time has run out!";
                timeRemaining = 0;
                timerIsRunning = false;
                GameOver();
            }
        }
    }

    public void Reset()
    {
        timerIsRunning = false;
        hasFood = false;
        hasPresent  = false;
        timeRemaining = 300;
        SetTasksVisible(false);
    }

//---------------------------------------------------------------------------------
    public void StartTimer()
    {
        timerIsRunning = true;
    }

    public void DestroyItAll()
    {
        Destroy(GameObject.FindWithTag("Player"));
        Destroy(GameObject.FindWithTag("LoadPlaces"));
        Destroy(GameObject.FindWithTag("Cursor"));

    }

//---------------------------------------------------------------------------------
    public void IsDrunk()
    {
        wobbleEffect = GameObject.FindWithTag("MainCamera").GetComponent<WobbleEffect>();
        wobbleEffect.StartWobble();
        music.setParameterByID(drunkId, 1);
    }

    public void HasFood()
    {
        hasFood = true;
        crossOut[0].SetActive(true);
        IncTasks();
    }

    public bool GetHasFood()
    {
        return hasFood;
    }

    public void HasPresent()
    {
        hasPresent = true;
        crossOut[1].SetActive(true);

        arrowController = GameObject.FindWithTag("Arrow").GetComponent<ArrowController>();

        if (hasFood)
            arrowController.SetTarget("Wife");
        else
            arrowController.SetTarget("FastFood");


        IncTasks();
    }

    public bool GetHasPresent()
    {
        return hasPresent;
    }

    public void MeetWife()
    {
        if (hasFood && hasPresent)
            YouWin();
        else
            Divorce();
    }

//---------------------------------------------------------------------------------
    public void SetTasksVisible(bool visible)
    {
        checklist.SetActive(visible);
    }

    public void TimePenalty()
    {
        timeRemaining -= 1;
    }

    public void SetInnerMonologue(string text)
    {
        PlayPenalty();
        innerMonologue.text = text;
        
        if(!running)
            StartCoroutine(FadeTextToFullAlpha());

        running = true;
    }

    public IEnumerator FadeTextToFullAlpha()
    {
        innerMonologue.color = new Color(innerMonologue.color.r, innerMonologue.color.g, innerMonologue.color.b, 0);
        while (innerMonologue.color.a < 1f)
        {
            innerMonologue.color = new Color(innerMonologue.color.r, innerMonologue.color.g, innerMonologue.color.b, innerMonologue.color.a + (Time.deltaTime / 2f));
            yield return null;
        }

        innerMonologue.color = new Color(innerMonologue.color.r, innerMonologue.color.g, innerMonologue.color.b, 1);
        while (innerMonologue.color.a > 0f)
        {
            innerMonologue.color = new Color(innerMonologue.color.r, innerMonologue.color.g, innerMonologue.color.b, innerMonologue.color.a - (Time.deltaTime / 2f));
            yield return null;
        }

        running = false;
    }

//---------------------------------------------------------------------------------
    public bool GetExpositionComplete()
    {
        return expositionComplete;
    }

    public void SetExpositionComplete()
    {
        expositionComplete = true;
    }

    public void SetReason(string text)
    {
        reason = text;
    }

    public string GetReason()
    {
        return reason;
    }

    public void PlaySuccess()
    {
        success = FMODUnity.RuntimeManager.CreateInstance(SuccessEvent);
        success.start();
    }

    public void PlayFailure()
    {
        failure = FMODUnity.RuntimeManager.CreateInstance(FailureEvent);
        failure.start();
    }

    public void PlayPenalty()
    {
        penalty = FMODUnity.RuntimeManager.CreateInstance(PenaltyEvent);
        penalty.start();
    }

    public void PlayClick()
    {
        click = FMODUnity.RuntimeManager.CreateInstance(ClickEvent);
        click.start();
    }

//---------------------------------------------------------------------------------
    public void StartGame()
    {
        expositionComplete = false;

        atmos = FMODUnity.RuntimeManager.CreateInstance(AtmosEvent);
        atmos.start();
        
        SceneManager.LoadSceneAsync("Pub");
    }
    
    public void GameOver()
    {
        SceneManager.LoadSceneAsync("GameOver");
    }

    public void YouWin()
    {
        SceneManager.LoadSceneAsync("Win");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Divorce()
    {
        SceneManager.LoadSceneAsync("Divorce");
    }

//---------------------------------------------------------------------------------
    public void HandlePlayerReactions(int choice, int id)
    {
        PlayClick();

        if (id == 16 && choice == 1)
            IsDrunk();
    } 

//---------------------------------------------------------------------------------
    public void IncTasks()
    {
        tasks++;
        music.setParameterByID(tasksCompletedId, tasks);
    } 
}
