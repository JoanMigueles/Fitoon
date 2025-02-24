using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class GooglePlayServicesManager : MonoBehaviour
{
    public static GooglePlayServicesManager instance;
    private ISavedGameMetadata currentSavedGame = null;
    private string savedGameFilename = "PlayerData";

    /// <summary>
    /// Singleton pattern
    /// </summary>
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

    /// <summary>
    /// Manually signs in the player, usually not needed
    /// </summary>
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

    public void SaveGame(string savedData)
    {
        if(currentSavedGame == null || !currentSavedGame.IsOpen)
        {
            OpenSavedGame();
        }

        var update = new SavedGameMetadataUpdate.Builder()
                .WithUpdatedDescription("Saved at " + DateTime.Now.ToString())
                .WithUpdatedPlayedTime(currentSavedGame.TotalTimePlayed.Add(TimeSpan.FromHours(1)))
                .Build();

        PlayGamesPlatform.Instance.SavedGame.CommitUpdate(
                currentSavedGame,
                update,
                System.Text.ASCIIEncoding.Default.GetBytes(savedData),
                (status, updated) => {return;});
    }

    public void LoadGame()
    {
        if(currentSavedGame == null || !currentSavedGame.IsOpen)
        {
            OpenSavedGame();
        }

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.ReadBinaryData(currentSavedGame, OnSavedGameDataRead);
    }

    private void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
    {
        if(status == SavedGameRequestStatus.Success)
        {
            string savedData = System.Text.ASCIIEncoding.Default.GetString(data);
            SaveData.ReceiveData(savedData);
        }
        else
        {
            Debug.Log("Error reading saved game data: " + status);
            SaveData.ReceiveData(null);
        }
    }

    /// <summary>
    /// Opens the saved game file in the cloud (if it doesn't exist, it creates it)
    /// </summary>
    public void OpenSavedGame()
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(savedGameFilename, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpened);
    }

    private void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if(status == SavedGameRequestStatus.Success) currentSavedGame = game;
        else Debug.Log("Error opening saved game: " + status);
    }

    public string GetPlayerUsername()
    {
        return PlayGamesPlatform.Instance.GetUserDisplayName();
    }
}