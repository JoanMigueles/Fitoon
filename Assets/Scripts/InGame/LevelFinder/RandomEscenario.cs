using FishNet;
using FishNet.Managing.Scened;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RandomEscenario : NetworkBehaviour
{
    static List<EscenarioItem> s_EscenariosDisponibles;
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
        if(s_EscenariosDisponibles == null)
        {
            s_EscenariosDisponibles = new List<EscenarioItem>(escenarios);
        }
        image = container.GetComponentInChildren<Image>();
        text = container.GetComponentInChildren<TextMeshProUGUI>();
        StartCoroutine(CambiarImagenAleatoria());
    }

    private void Update()
    {
        if(timerActive) timer += Time.deltaTime;
    }

    [ObserversRpc]
    void SetFinalMap(int i)
    {
        StopAllCoroutines();

        EscenarioItem escenario = s_EscenariosDisponibles[i];
		image.sprite = escenario.imagenEscenario;
		text.text = escenario.nombreEscenario;
		s_EscenariosDisponibles.Remove(escenario);

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
            index = Random.Range(0, s_EscenariosDisponibles.Count);
			escenarioElegido = s_EscenariosDisponibles[index];
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
