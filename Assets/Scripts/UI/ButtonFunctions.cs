using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        GooglePlayServicesManager.instance.PostScore();
        GooglePlayServicesManager.instance.ShowLeaderboard();
        //SceneManager.LoadScene(sceneName);
    }

    public void ExitApp()
    {
        Application.Quit();
    }

}
