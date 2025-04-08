using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIManager : UIManager
{
    [Header("Gym Data")]
    [SerializeField] private TextMeshProUGUI gymNameText;
    [SerializeField] private TextMeshProUGUI gymCodeText;
    [SerializeField] private Image gymIconImage;
    [SerializeField] private GameObject buttonUnlinkGym;
    [SerializeField] private GameObject buttonLinkGym;
    private void Start()
    {
        SaveData.ReadFromJson();
        UpdateAllUI();
    }

    public void UpdateAllUI()
    {
        if(SaveData.player.gymKey != -1)
        {
            if (gymNameText != null)
            {
                DatabaseManager.instance.GetGymMedals(SaveData.player.gymKey, (result) =>
                {
                    MainThreadDispatcher.instance.Enqueue(() =>
                    {
                        gymNameText.text = result.Item1;
                    });
                });
            }
        }
        else
        {
            if (gymNameText != null) gymNameText.text = "No Gym";
        }
        if (gymCodeText != null)
        {
            if (SaveData.player.gymKey == -1) gymCodeText.text = "";
            else gymCodeText.text = "#" + SaveData.player.gymKey.ToString();
        }
        if (buttonUnlinkGym != null) buttonUnlinkGym.SetActive(SaveData.player.gymKey != -1);
        if (buttonLinkGym != null) buttonLinkGym.SetActive(SaveData.player.gymKey == -1);

        //if (gymIconImage != null) gymIconImage.sprite = ;
    }

    public void UnlinkGym()
    {
        SaveData.player.gymKey = -1;
        SaveData.SaveToJson();
        UpdateAllUI();
    }
}
