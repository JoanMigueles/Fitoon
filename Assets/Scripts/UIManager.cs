using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class UIManager : MonoBehaviour
{
    [Header("Menu Window Dark Background")]
    [SerializeField] private GameObject darkenBackground;

    // --------------------------------------------------------------------------------------------------------------------------------------------------
    // Menu and scene management
    // --------------------------------------------------------------------------------------------------------------------------------------------------

    public void OpenMenu(GameObject menu)
    {
        if (menu != null) {
            menu.SetActive(true);
            if (darkenBackground != null) darkenBackground.SetActive(true);
        }
    }

    public void CloseMenu(GameObject menu)
    {
        if (menu != null) {
            menu.SetActive(false);
            if (darkenBackground != null) darkenBackground.SetActive(false);
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
