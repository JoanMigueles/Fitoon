using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUIManager : UIManager
{
    [Header("Player Resources And Stats")]
    [SerializeField] private TextMeshProUGUI medalText;
    [SerializeField] private TextMeshProUGUI streakText;
    [SerializeField] private TextMeshProUGUI winsText;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI leaderboardText;

    [Header("Player Data")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TMP_InputField playerNameField;
    [SerializeField] private TextMeshProUGUI playerTitleText;
    [SerializeField] private GameObject banner;
    [SerializeField] private Image profileIconImage;

    [Header("Gym Data")]
    [SerializeField] private TextMeshProUGUI gymLeaderboardText;
    [SerializeField] private TextMeshProUGUI gymMedalText;
    [SerializeField] private TextMeshProUGUI gymNameText;
    [SerializeField] private TextMeshProUGUI gymCodeText;
    [SerializeField] private Image gymIconImage;
    [SerializeField] private GameObject buttonUnlinkGym;

    private void Start()
    {
        SaveData.ReadFromJson();
        UpdateAllUI();
    }

    public void SaveUsername(string value)
    {
        if(value == "" || value == SaveData.player.username) return;
        SaveData.ChangeUsername(value, (result) =>
        {
            MainThreadDispatcher.instance.Enqueue(() =>
            {
                UpdateProfile(result);
            });
        });
    }

    public void UpdateAllUI()
    {
        if (medalText != null) medalText.text = SaveData.player.medals.ToString();
        if (streakText != null) streakText.text = SaveData.player.streak.ToString();
        if (winsText != null) winsText.text = SaveData.player.wins.ToString();
        if (distanceText != null) distanceText.text = SaveData.player.runnedDistance.ToString();
        UpdateProfile();
        UpdateLeaderboardAndGym();
    }

    public void UpdateLeaderboardAndGym()
    {
        if (gymLeaderboardText != null)
        {
            if(SaveData.player.gymKey == -1) gymLeaderboardText.text = "No Gym";
            else
            {
                DatabaseManager.instance.GetGymPosition(SaveData.player.gymKey, (position) =>
                {
                    MainThreadDispatcher.instance.Enqueue(() =>
                    {
                        gymLeaderboardText.text = "# " + position.ToString();
                    });
                });
            }
        };

        if (gymMedalText != null && gymNameText != null)
        {
            if (SaveData.player.gymKey == -1)
            {
                gymMedalText.text = "0";
                gymNameText.text = "No Gym";
            }
            else
            {
                DatabaseManager.instance.GetGymMedals(SaveData.player.gymKey, (medals) =>
                {
                    MainThreadDispatcher.instance.Enqueue(() =>
                    {
                        gymNameText.text = medals.Item1;
                        gymMedalText.text = medals.Item2.ToString();
                    });
                });
            }
        }

        if(gymCodeText != null)
        {
            if (SaveData.player.gymKey == -1) gymCodeText.text = "";
            else gymCodeText.text = "#" + SaveData.player.gymKey.ToString();
        }

        if(leaderboardText != null)
        {
            DatabaseManager.instance.GetGlobalPosition((position) =>
            {
                MainThreadDispatcher.instance.Enqueue(() =>
                {
                    leaderboardText.text = "# " + position.ToString();
                });
            });
        }

        if (buttonUnlinkGym != null) buttonUnlinkGym.SetActive(SaveData.player.gymKey != -1);
        // if (gymIconImage != null) gymIconImage.sprite = ;
    }

    public void UpdateProfile(bool usernameNotTaken = false)
    {
        if (playerNameText != null) playerNameText.text = SaveData.player.username;
        if (playerNameField != null)
        {
            if (!usernameNotTaken) StartCoroutine(ShowError());
            else playerNameField.text = SaveData.player.username;
        }
        // if (banner != null) banner...;
        // if (profileIconImage != null) profileIconImage.sprite = ;
    }

    public void UnlinkGym()
    {
        SaveData.player.gymKey = -1;
        SaveData.SaveToJson();
        UpdateAllUI();
    }

    IEnumerator ShowError()
    {
        playerNameField.textComponent.color = Color.red;
        playerNameField.text = "Already taken!";
        yield return new WaitForSeconds(1f);
        playerNameField.textComponent.color = Color.white;
        playerNameField.text = SaveData.player.username;
    }
}
