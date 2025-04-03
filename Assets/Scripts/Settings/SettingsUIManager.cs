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
    private void Start()
    {
        SaveData.ReadFromJson();
        UpdateAllUI();
    }

    public void UpdateAllUI()
    {
        //if (gymNameText != null) gymNameText.text = ;
        //if (gymCodeText != null) gymNameText.text = ;
        //if (gymIconImage != null) gymIconImage.sprite = ;
    }
}
