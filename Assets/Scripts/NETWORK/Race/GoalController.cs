using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalController : MonoBehaviour
{
    SaveData saveData;
    public TextMeshProUGUI currentPositionTextMesh;

    [SerializeField] private int finishedPlayers = 0;
    [SerializeField] private int playerPosition = 0;
    [SerializeField] private bool playerFinished = false;
    [SerializeField] private List<Transform> players;
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI infoText;
    int maxPlayers = 32;
    bool finish = false;

    //Evento cuando acaba la carrera. Se manda a todos los participantes (bots incluidos)
    public delegate void OnRaceFinish();
    public static event OnRaceFinish onRaceFinishEvent;
    private void Awake()
    {
        saveData = FindAnyObjectByType<SaveData>();
    }
    private void Start()
    {
        Reset();
    }

    public void UpdatePosition()
    {
        //if (players.Contains(player)) playerPosition = finishedPlayers + (players.IndexOf(player) + 1);

        //currentPositionTextMesh.text = playerPosition + "/" + (players.Count + finishedPlayers);

        //En lugar de la posicion mostramos los clasificados
        currentPositionTextMesh.text = finishedPlayers + "/" + (maxPlayers);
    }


    public void AddPlayerToList(Transform player)
    {
        if (!players.Exists(p => p == player))
        {
            players.Add(player);
        }
    }


    public void RemovePlayerFromList(Transform player)
    {
        if(finishedPlayers < maxPlayers)
        {
            finishedPlayers++;
            players.RemoveAll(p => p == null || p == player);
            UpdatePosition();

            if (player.GetChild(0).gameObject.tag == "Character")
            {
                //Ha entrado el jugador
                PlayerFinish();
                FindObjectOfType<PlayerControl>().StopCharacterOnFinish();
                FindObjectOfType<PlayerControl>().LockMovement(true);
                FindObjectOfType<FinishController>().Finish(); //Animacion de acabar. To do: que diga You win!

                //A�adirlo a una lista en race manager para la siguiente ronda
                RaceManager.Instance.raceBots.Add(new RaceManager.RaceBotsData(true, saveData.player.username, 0, 0, 0, 0, 0));
            }

            else
            {
                //Ha entrado otro
                

                //A�adirlo a una lista en race manager para la siguiente ronda
                BotSkins botSkins = player.GetComponentInChildren<BotSkins>();

                int botSkin = botSkins.botSkinData["Skin"];
                int botHair = botSkins.botSkinData["Hair"];
                int botShirt = botSkins.botSkinData["Shirt"];
                int botPants = botSkins.botSkinData["Pants"];
                int botShoes = botSkins.botSkinData["Shoes"];
                RaceManager.Instance.raceBots.Add(new RaceManager.RaceBotsData(false, player.name, botSkin, botHair, botShirt, botPants, botShoes));

                //Hacer que desaparezca el bot
                player.gameObject.SetActive(false);
            }
        }

        if(finishedPlayers == maxPlayers && !finish)
        {
            //Fin de carrera
            EndRace();
        }
    }

    private void EndRace()
    {
        finish = true;

        //Lanzar evento fin de carrera a los que queden
        if (onRaceFinishEvent != null)
        {
            onRaceFinishEvent();
        }

        FindObjectOfType<FinishController>().Finish(); //Animacion de acabar
        print("numb of race" + RaceManager.Instance.numberOfRace);
        print("max races" + RaceManager.Instance.maxRaces);
        if (RaceManager.Instance.numberOfRace == RaceManager.Instance.maxRaces)
        {
            EndGame();
        }
        else
        {
            if (playerFinished)
            {
                //Ha ganado, puede pasar a siguiente nivel aleatorio
                StartCoroutine(NextLevel());
            }
            else
            {
                //Ha perdido, vuelve al inicio
                //infoText.text = "You lost... Press exit to go back";
                exitButton.gameObject.SetActive(true);
                exitButton.onClick.AddListener(delegate { FindObjectOfType<ButtonFunctions>().LoadScene("YouLose"); });
            }
        }
    }

    private void EndGame()
    {
        Debug.LogWarning("fin de PARTIDA");

        if (playerFinished)
        {
            //Gan� el jugador
            infoText.text = "You win!!! Press exit to go back";
            RaceManager.Instance.playerWon = true;
            exitButton.gameObject.SetActive(true);
            exitButton.onClick.AddListener(delegate { FindObjectOfType<ButtonFunctions>().LoadScene("YouWin"); });
        }
        else
        {
            //Perdi�
            infoText.text = "You lost!!! Press exit to go back";
            exitButton.gameObject.SetActive(true);
            exitButton.onClick.AddListener(delegate { FindObjectOfType<ButtonFunctions>().LoadScene("YouLose"); });
        } 
    }

    IEnumerator NextLevel()
    {
        infoText.text = "You win! Next level...";
        exitButton.gameObject.SetActive(false) ;
        yield return new WaitForSeconds(5f);
        FindObjectOfType<ButtonFunctions>().LoadScene("Classified");
    }


    public void PlayerFinish()
    {
        playerFinished = true;
    }


    public void Reset()
    {
        players.Clear();
        finishedPlayers = 0;
        playerPosition = 0;
        playerFinished = false;
        finish = false;
        currentPositionTextMesh.text = playerPosition + "/" + players.Count;

        if (RaceManager.Instance != null)
        {
            int ronda = RaceManager.Instance.numberOfRace;
            switch (ronda)
            {
                case 1:
                    maxPlayers = RaceManager.Instance.playerPerRace[0];
                    break;
                case 2:
                    maxPlayers = RaceManager.Instance.playerPerRace[1];
                    break;
                case 3:
                    maxPlayers = RaceManager.Instance.playerPerRace[2];
                    break;
            }
            roundText.text = "RONDA " + ronda;
        }
        
        else Debug.LogError("Error: No race manager");
    }
}
