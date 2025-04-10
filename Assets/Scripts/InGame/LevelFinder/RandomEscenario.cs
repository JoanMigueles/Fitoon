using FishNet;
using FishNet.Managing.Scened;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is used to randomly select the map for the game. It will show a countdown and then load the selected map.
/// </summary>
public class RandomEscenario : NetworkBehaviour
{
    [SerializeField] List<EscenarioItem> escenarios = new List<EscenarioItem>();
    [SerializeField] GameObject container;
    [SerializeField] TextMeshProUGUI countdownText;
    [SerializeField] int countdown = 5;
    Image image;
    TextMeshProUGUI text;
    EscenarioItem escenarioElegido;

    public float secondsToChange;
    public float totalSeconds;
    float timer = 0;
    bool timerActive = true;

    void Start()
    {
		image = container.GetComponentInChildren<Image>();
        text = container.GetComponentInChildren<TextMeshProUGUI>();
        StartCoroutine(CambiarImagenAleatoria());
    }

    private void Update()
    {
        if(timerActive) timer += Time.deltaTime;
    }

	/// <summary>
	/// Sets the final map to be loaded. This function is called by the server when the countdown ends.
	/// </summary>
	[ObserversRpc]
    void SetFinalMap(int i)
    {
        Debug.Log("SetFinalMap: " + i.ToString() + " " + escenarios[i].nombreEscenario);
		StopAllCoroutines();

        EscenarioItem escenario = escenarios[i];
		image.sprite = escenario.imagenEscenario;
		text.text = escenario.nombreEscenario;
		escenarios.Remove(escenario);

		timerActive = false;
		countdownText.text = countdown.ToString();
		StartCoroutine(Countdown());
	}

    IEnumerator CambiarImagenAleatoria()
    {
        int index = 0;
        while (timer < totalSeconds) 
        {
            yield return new WaitForSeconds(secondsToChange);
			Random.InitState((int)System.DateTime.Now.Ticks);
			index = Random.Range(0, escenarios.Count);
			escenarioElegido = escenarios[index];
            image.sprite = escenarioElegido.imagenEscenario;
            text.text = escenarioElegido.nombreEscenario;
        }

        if (!IsServerInitialized)
            yield break;

        SetFinalMap(index) ;
		//timer = 0;
    }

    IEnumerator Countdown()
    {
		countdownText.text = countdown.ToString();
		while (countdown-- != 0)
        {
			yield return new WaitForSeconds(1f);
			countdownText.text = countdown.ToString();
			if (countdown == 0 && IsServerInitialized)
            {
                SceneLoadData sld = new SceneLoadData(escenarioElegido.nombreEscenario);
                SceneManager.LoadGlobalScenes(sld);
                SceneUnloadData sud = new SceneUnloadData("FindingScenario");
                SceneManager.UnloadGlobalScenes(sud);
			}
        }
    }
}
