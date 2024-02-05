using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    public void PlayAgain()
    {
        SceneManager.LoadSceneAsync("Pub");
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
