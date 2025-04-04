using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardField : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI playerTitle;
    [SerializeField] private TextMeshProUGUI medals;
    [SerializeField] private TextMeshProUGUI position;
    [SerializeField] private GameObject banner;
    [SerializeField] private Image profileIcon;
    [SerializeField] private Image leagueIcon;

    public void SetPlayerName(string name)
    {
        playerName.text = name;
    }

    public void SetPlayerTitle(string title)
    {
        playerName.text = title;
    }

    public void SetMedals(int medals, ProgressData pData)
    {
        this.medals.text = medals.ToString();
        int rank = medals / pData.rankMedalInterval;
        if(rank >= pData.rankTitles.Count) {
            rank = pData.rankTitles.Count - 1;
        }
        if (leagueIcon != null) {
            leagueIcon.sprite = pData.rankSprites[rank];
        }

    }

    public void SetPosition(int pos)
    {
        position.text = "#" + pos.ToString();
    }

    public void SetBanner(int bannerID)
    {

    }

    public void SetProfilePicture(int pfpID)
    {

    }
    
}
