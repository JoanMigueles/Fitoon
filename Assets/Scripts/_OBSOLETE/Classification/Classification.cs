using FishNet;
using FishNet.Managing.Scened;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Obsolete]
public class Classification : MonoBehaviour
{
    [SerializeField] int countdown;
    [SerializeField] TextMeshProUGUI countdownText;
	void Start()
    {
        StartCoroutine(CountDown());
    }

    IEnumerator CountDown()
    {
        while (countdown >= 0)
        {
            countdownText.text = countdown.ToString();
            yield return new WaitForSeconds(1);
            countdown--;
        }
        SceneLoadData sld = new SceneLoadData("FindingScenario");
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }
}
