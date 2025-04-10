using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIManager : UIManager
{
    [Header("Gym Data")]
    [SerializeField] private TextMeshProUGUI gymNameText;
    [SerializeField] private TextMeshProUGUI gymKeyText;
    [SerializeField] private Image gymIconImage;
    [SerializeField] private GameObject linkGymMenu;
    [SerializeField] private GameObject createGymMenu;
    [SerializeField] private GameObject buttonUnlinkGym;
    [SerializeField] private GameObject buttonLinkGym;
    [SerializeField] private TMP_InputField gymKeyInputField;
    [SerializeField] private TMP_InputField gymNameInputField;
    [SerializeField] private TMP_InputField authKeyInputField;
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
        if (gymKeyText != null)
        {
            if (SaveData.player.gymKey == -1) gymKeyText.text = "";
            else gymKeyText.text = "#" + SaveData.player.gymKey.ToString();
        }
        if (buttonUnlinkGym != null) buttonUnlinkGym.SetActive(SaveData.player.gymKey != -1);
        if (buttonLinkGym != null) buttonLinkGym.SetActive(SaveData.player.gymKey == -1);

        //if (gymIconImage != null) gymIconImage.sprite = ;
    }

    public void LinkGym()
    {
        int gymKey;
        if(int.TryParse(gymKeyInputField.text, out gymKey))
        {
            DatabaseManager.instance.CheckGymKey(gymKey, (result) =>
            {
                if (result)
                {
                    DatabaseManager.instance.CheckGymStatus(gymKey, (status) =>
                    {
                        if(status)
                        {
                            MainThreadDispatcher.instance.Enqueue(() =>
                            {
                                SaveData.player.gymKey = gymKey;
                                SaveData.SaveToJson();
                                UpdateAllUI();
                                CloseMenu(linkGymMenu);
                            });
                        }
                        else
                        {
                            MainThreadDispatcher.instance.Enqueue(() =>
                            {
                                StartCoroutine(ShowGymKeyError("This gym is no longer active."));
                            });
                        }
                    });
                }
                else
                {
                    MainThreadDispatcher.instance.Enqueue(() =>
                    {
                        StartCoroutine(ShowGymKeyError("Gym key not found."));
                    });
                }
            });
        }
        else
        {
            StartCoroutine(ShowGymKeyError("Gym keys must be numbers."));
        }
    }

    public void UnlinkGym()
    {
        SaveData.player.gymKey = -1;
        SaveData.SaveToJson();
        UpdateAllUI();
    }

    public void CreateGym()
    {
        string gymName = gymNameInputField.text;
        int authKey;

        if(string.IsNullOrEmpty(gymName))
        {
            StartCoroutine(ShowGymNameError("Cannot be empty"));
            return;
        }

        if (int.TryParse(authKeyInputField.text, out authKey))
        {
            DatabaseManager.instance.RegisterGym(gymName, authKey, (result) =>
            {
                if (result.Item1 == 0)
                {
                    MainThreadDispatcher.instance.Enqueue(() =>
                    {
                        SaveData.player.gymKey = result.Item2;
                        SaveData.SaveToJson();
                        UpdateAllUI();
                        CloseMenu(createGymMenu);
                    });
                }
                else if (result.Item1 == 1)
                {
                    MainThreadDispatcher.instance.Enqueue(() =>
                    {
                        StartCoroutine(ShowGymNameError("Gym name already taken"));
                    });
                }
                else if (result.Item1 == 2)
                {
                    MainThreadDispatcher.instance.Enqueue(() =>
                    {
                        StartCoroutine(ShowAuthKeyError("Wrong auth key"));
                    });
                }
            });
        }
        else
        {
            StartCoroutine(ShowAuthKeyError("Must be a number"));
        }
    }

    IEnumerator ShowGymKeyError(string errorMessage)
    {
        gymKeyInputField.textComponent.color = Color.red;
        gymKeyInputField.text = errorMessage;
        yield return new WaitForSeconds(1f);
        gymKeyInputField.textComponent.color = Color.white;
        gymKeyInputField.text = "";
    }

    IEnumerator ShowGymNameError(string errorMessage)
    {
        gymNameInputField.textComponent.color = Color.red;
        gymNameInputField.text = errorMessage;
        yield return new WaitForSeconds(1f);
        gymNameInputField.textComponent.color = Color.white;
        gymNameInputField.text = "";
    }

    IEnumerator ShowAuthKeyError(string errorMessage)
    {
        authKeyInputField.textComponent.color = Color.red;
        authKeyInputField.text = errorMessage;
        yield return new WaitForSeconds(1f);
        authKeyInputField.textComponent.color = Color.white;
        authKeyInputField.text = "";
    }
}
