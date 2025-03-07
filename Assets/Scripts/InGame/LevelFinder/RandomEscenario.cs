using FishNet;
using FishNet.Managing.Scened;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RandomEscenario : MonoBehaviour
{
    [SerializeField] List<EscenarioItem> escenarios;
    [SerializeField] List<EscenarioItem> escenariosDisponibles;
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
        escenariosDisponibles = new List<EscenarioItem>(escenarios);
        GetAvaliableScenarios();
        StartCoroutine(CambiarImagenAleatoria());
    }

    private void Update()
    {
        if(timerActive) timer += Time.deltaTime;
    }

    IEnumerator CambiarImagenAleatoria()
    {
        while (timer < totalSeconds) 
        {
            yield return new WaitForSeconds(secondsToChange);
            int index = Random.Range(0, escenariosDisponibles.Count);
			escenarioElegido = escenariosDisponibles[index];
            image.sprite = escenarioElegido.imagenEscenario;
            text.text = escenarioElegido.nombreEscenario;
        }

		//timer = 0;
		// SaveData.player.scenesPlayed.Add(escenarioElegido);
		SaveData.SaveToJson();

        timerActive = false;
        countdownText.text = countdown.ToString();
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {       
        while (countdown != 0)
        {
            yield return new WaitForSeconds(1f);
            countdown--;
            if (countdown == 0)
            {
                SceneLoadData sceneData = new SceneLoadData(escenarioElegido.nombreEscenario);
                InstanceFinder.SceneManager.LoadGlobalScenes(sceneData);
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("FindingScenario");
			}
            else
            {
                countdownText.text = countdown.ToString();
            }
        }
    }

    private void GetAvaliableScenarios()
    {
        /*
		SaveData.ReadFromJson();
        foreach (EscenarioItem item in SaveData.player.scenesPlayed)
        {
            escenariosDisponibles.Remove(item);
        }*/
    }
}
