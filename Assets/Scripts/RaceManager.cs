using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    [System.Serializable]
    public struct RaceBotsData
    {
        public bool isPlayer;
        public string botID;
        public int skinIndex, hairIndex, topIndex, bottomIndex, shoeIndex;

        public RaceBotsData(bool isPlayer, string botID, int skinIndex, int hairIndex, int topIndex, int bottomIndex, int shoeIndex)
        {
            this.isPlayer = isPlayer;
            this.botID = botID;
            this.skinIndex = skinIndex;
            this.hairIndex = hairIndex;
            this.topIndex = topIndex;
            this.bottomIndex = bottomIndex;
            this.shoeIndex = shoeIndex;
        }
    }

    public static RaceManager Instance;

    public int numberOfRace = 1;
    public int maxRaces = 3;
    public bool playerWon;
    public int[] playerPerRace;

    public List<RaceBotsData> raceBots;

    private void Awake()
    {

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        raceBots = new List<RaceBotsData>();
        playerPerRace = new int[3];
        playerPerRace[0] = 16;
        playerPerRace[1] = 8;
        playerPerRace[2] = 1;
    }

    public void Reset()
    {
        Debug.Log("RACE MANAGER RESET");
        numberOfRace = 1;
        maxRaces = 3;
        raceBots = new List<RaceBotsData>();
        playerWon = false;
		SaveData.ReadFromJson();
		//SaveData.player.scenesPlayed.Clear();
		SaveData.SaveToJson();
    }
}
