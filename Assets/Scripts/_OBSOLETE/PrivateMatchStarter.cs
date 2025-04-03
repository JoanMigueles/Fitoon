using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[Obsolete]
public class PrivateMatchStarter : MonoBehaviour
{
	[SerializeField] GameObject inputMenu;
    [SerializeField] TMP_InputField inputField;
    public void OpenMenu()
    {
        inputMenu.SetActive(true);
    }
    public void CloseMenu()
    {
        inputMenu.SetActive(false);
    }
    public void StartGame()
    {
        DiscoveryHandler.Passcode = inputField.text;
        SessionDataHolder.Reset();
		SceneManager.LoadScene("LobbyScene");
    }
    public void SetPasscode(string value)
    {
		DiscoveryHandler.Passcode = value;
	}
}
