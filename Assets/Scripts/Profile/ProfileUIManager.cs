using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        SaveData.ReadFromJson();
        UpdateAllUI();
    }

    public void SaveUsername(string value)
    {
        SaveData.player.username = value;
        SaveData.SaveToJson();
        UpdateProfile();
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
        //TODO
        //if (gymLeaderboardText != null) gymLeaderboardText.text = ;
        //if (gymMedalText != null) gymMedalText.text = ;
        //if (gymNameText != null) gymNameText.text = ;
        //if (gymIconImage != null) gymIconImage.sprite = ;
    }

    public void UpdateProfile()
    {
        if (playerNameText != null) playerNameText.text = SaveData.player.username;
        if (playerNameField != null) playerNameField.text = SaveData.player.username;
        //if (banner != null) banner...;
        //if (profileIconImage != null) profileIconImage.sprite = ;
    }
}
