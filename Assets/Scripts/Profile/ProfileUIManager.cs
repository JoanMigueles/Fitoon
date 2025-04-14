using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
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
    [SerializeField] private Banner banner;

    [Header("Gym Data")]
    [SerializeField] private TextMeshProUGUI gymLeaderboardText;
    [SerializeField] private TextMeshProUGUI gymMedalText;
    [SerializeField] private TextMeshProUGUI gymNameText;
    [SerializeField] private TextMeshProUGUI gymCodeText;
    [SerializeField] private Image gymIconImage;
    [SerializeField] private GameObject buttonUnlinkGym;

    [Header("Unlocked Profiles and Banners")]
    [SerializeField] private GameObject iconsContent;
    [SerializeField] private GameObject bannersContent;
    [SerializeField] private GameObject iconButtonPrefab;
    [SerializeField] private GameObject bannerButtonPrefab;
    [SerializeField] private TMP_Dropdown titleDropdown;

    private void Start()
    {
        SaveData.ReadFromJson();
        ReadUnlockedElements();
        UpdateAllUI();
    }

    public static void RefreshLayoutGroupsImmediateAndRecursive(GameObject root)
    {
        foreach (var layoutGroup in root.GetComponentsInChildren<LayoutGroup>()) {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        }
    }

    private void ReadUnlockedElements()
    {
        // Icons
        foreach (int iconID in SaveData.player.unlockedIcons) {
            GameObject icon = Instantiate(iconButtonPrefab, iconsContent.transform);
            Button button = icon.GetComponent<Button>();
            
            button.transform.GetChild(0).GetComponent<Image>().sprite = AssetLoader.LoadPFP(iconID);
            button.onClick.AddListener(() => SavePFP(iconID));
        }

        // Banners
        foreach (int bannerID in SaveData.player.unlockedBanners) {
            GameObject banner = Instantiate(bannerButtonPrefab, bannersContent.transform);
            Button button = banner.GetComponent<Button>();

            button.GetComponent<Banner>().SetBanner(bannerID);
            button.onClick.AddListener(() => SaveBanner(bannerID));
        }

        // Titles
        foreach (string title in SaveData.player.unlockedTitles) {
            titleDropdown.options.Add(new TMP_Dropdown.OptionData(title));
            SelectOptionByText(SaveData.player.title);
        }
    }

    private void SelectOptionByText(string optionText)
    {
        // Find the index of the option with matching text
        for (int i = 0; i < titleDropdown.options.Count; i++) {
            if (titleDropdown.options[i].text == optionText) {
                titleDropdown.value = i;
                titleDropdown.RefreshShownValue();
                return;
            }
        }

        Debug.LogWarning("Option not found: " + optionText);
    }

    public void SaveTitle(int titleIndex)
    {
        SaveData.player.title = titleDropdown.options[titleIndex].text;
        UpdateTitle();
        SaveData.SaveToJson();
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

    public void SaveBanner(int bannerID)
    {
        SaveData.player.bannerID = bannerID;
        UpdateBanner();
        SaveData.SaveToJson();
    }

    public void SavePFP(int profileID)
    {
        SaveData.player.pfp = profileID;
        Debug.Log("pfp changed");
        UpdateBanner();
        SaveData.SaveToJson();
    }

    public void UpdateAllUI()
    {
        if (medalText != null) medalText.text = SaveData.player.medals.ToString();
        if (streakText != null) streakText.text = SaveData.player.streak.ToString();
        if (winsText != null) winsText.text = SaveData.player.wins.ToString();
        if (distanceText != null) distanceText.text = SaveData.player.runnedDistance.ToString();
        UpdateProfile();
        UpdateBanner();
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
        UpdateTitle();
    }

    public void UpdateTitle()
    {
        if (playerTitleText != null) playerTitleText.text = SaveData.player.title;
    }

    public void UpdateBanner()
    {
        if (banner != null) {
            banner.SetBanner(SaveData.player.bannerID);
            banner.SetProfilePicture(SaveData.player.pfp);
        }
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
