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
    [SerializeField] private Banner banner;
    [SerializeField] private Image leagueIcon;
    [SerializeField] private Image readyIcon;

    public void SetPlayerName(string name)
    {
        playerName.text = name;
    }

    public void SetPlayerTitle(string title)
    {
        playerTitle.text = title;
    }

    public void SetMedals(int medals)
    {
        ProgressData pData = Resources.Load<ProgressData>("ProgressData");

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

    public void SetBanner(int b)
    {
        banner.SetBanner(b);
    }

    public void SetProfilePicture(int pfp)
    {
        banner.SetProfilePicture(pfp);
    }

    public void SetReady(bool ready)
    {
        readyIcon.gameObject.SetActive(ready);
    }
    
}
