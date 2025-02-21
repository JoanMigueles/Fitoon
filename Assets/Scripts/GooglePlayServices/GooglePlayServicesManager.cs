using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class GooglePlayServicesManager : MonoBehaviour
{
    public static GooglePlayServicesManager instance;

    private void Awake()
    {
        if(GooglePlayServicesManager.instance == null)
        {
            GooglePlayServicesManager.instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void SignIn()
    {
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(OnSignInResult);
    }

    private void OnSignInResult(SignInStatus signInStatus)
    {
        if(signInStatus == SignInStatus.Success) Debug.Log("Sign in success");
        else Debug.Log("Sign in failed");
    }
}