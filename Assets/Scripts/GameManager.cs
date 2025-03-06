using FishNet;
using FishNet.Component.Spawning;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
	[SerializeField] Transform[] spawnPoints;
    [SerializeField] PlayerController playerPrefab;
    [SerializeField] BotRunner botPrefab;
    [SerializeField] Countdown countdown;
    [SerializeField] int m_numberOfRunners;
    

    public static List<Runner> runnerData;
    static List<Runner> winners;
    static List<BaseRunner> runners;
	public static int numberOfRunners;
	public static int numberOfPlayers = 0;
    static int currentRace = 0;
    static int[] s_numberOfWinners = { 16, 8, 1 };

    static bool classified = false;

    static int playerIndex = 0;
    public override void OnStartServer()
    {
        if(IsServerInitialized)
        {
			InstanceFinder.NetworkManager.gameObject.GetComponent<PlayerSpawner>().Spawns = spawnPoints;
			Debug.Log(InstanceFinder.NetworkManager.gameObject.GetComponent<PlayerSpawner>().Spawns);

			if (currentRace != 0)
			{
				playerIndex = 0;
				SpawnBots();
				return;
			}

			if (runners == null)
			{
				ResetStaticValues();
				Debug.Log("Reset Static values");
			}

			Debug.Log("Current race != 0");
			InitializeBots();
            Debug.Log("Initialized bots");
			SpawnBots();
		}
		
		countdown.StartCountdown();
		StartCoroutine(WaitForCountdown());
	}

    void UnfreezeAllRunners()
    {
        for(int i = 0; i < runners.Count; i++)
        {
            runners[i].UnFreeze();
        }
    }

	void SpawnBots()
    {
		for (int i = 0; i < runnerData.Count; i++)
		{
            if (runnerData[i].isPlayer)
            {
                continue;
            }

			BotRunner runner = Instantiate(botPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
            runner.SetId(runnerData[i].id);
            runner.LoadCharacter(runnerData[i].character);
            runners.Add(runner);
            Spawn(runner.gameObject);
		}
	}

	public static void ResetStaticValues()
	{
        runners = new List<BaseRunner>();
		runnerData = new List<Runner>();
		winners = new List<Runner>();
		currentRace = 0;
		classified = false;
        playerIndex = 0;
        numberOfRunners = s_numberOfWinners[0] * 2;
	}

    void InitializeBots()
    {
        for (int i = numberOfPlayers; i < numberOfRunners; i++)
        {
            runnerData.Add
            (
                new Runner
                {
                    id = i,
                    character = CharacterLoader.CreateRandomCharacter(),
                    isPlayer = false,
                }
            );
        }
	}

	public static void GoalReached(BaseRunner baseRunner)
    {
		if (runnerData.Count == 0)
		{
			return;
		}

		if (baseRunner.GetComponent<PlayerController>() != null)
        {
            classified = true;
        }

		Runner runner = new Runner();

		try
        {
			runner = runnerData.Find((r) => r.id == baseRunner.GetId());
        }
        catch(ArgumentNullException)
        {
            Debug.LogError("Runner not found");
            return;
        }

        winners.Add(runner);
        if(winners.Count >= s_numberOfWinners[currentRace])
        {
            runnerData = winners;
            winners = new List<Runner>();
            runners = new List<BaseRunner>();
			currentRace++;

            if(!classified)
            {
                AssessResult();
                return;
            }

            if (currentRace < s_numberOfWinners.Length)
            {
                SceneLoadData sld = new SceneLoadData("Classified");
                InstanceFinder.SceneManager.LoadGlobalScenes(sld);
            }
            else
            {
                AssessResult();
            }
        }
    }

    public static void AssessResult()//Esto habrá que cambiarlo para networking
    {
        if(runnerData.Count == 0)
        {
            return;
        }

        if(currentRace == s_numberOfWinners.Length)
        {
            if (classified)
            {
                currentRace = 0;
                SceneLoadData sldw = new SceneLoadData("YouWin");
                InstanceFinder.SceneManager.LoadGlobalScenes(sldw);
                return;
            }
        }
        currentRace = 0;
		SceneLoadData sld = new SceneLoadData("YouLose");
		InstanceFinder.SceneManager.LoadGlobalScenes(sld);
	}

    public static void AddPlayer()
    {
        numberOfPlayers++;
    }

    public static void RemovePlayer()
    {
        numberOfPlayers--;
    }

    public static void AddPlayerToRace(PlayerController player)//Probablemente habrá que cambiarlo para networking
    {
        if(runners == null)
        {
			ResetStaticValues();
			Debug.Log("Reset Static values");
		}
        Debug.Log("PlayerAdded");
        runners.Add(player);
		if (runnerData.Count == 0)
		{
			return;
		}

		player.SetId(playerIndex);
        runnerData.Add(
            new Runner()
            {
                id = playerIndex++,
                isPlayer = true
            });
    }

    public static void RemoveRunner(BaseRunner runner)
    {
        runnerData.RemoveAt(runnerData.FindIndex((r) => r.id == runner.GetId()));
        runners.RemoveAt(runners.FindIndex((r) => r.GetId() == runner.GetId()));
        try
        {
            winners.RemoveAt(winners.FindIndex((r) => r.id == runner.GetId()));
		}
        catch(Exception) { }
    }

    public IEnumerator WaitForCountdown()
    {
        yield return new WaitUntil(() => countdown.HasFinished());
        UnfreezeAllRunners();
    }

    
}
public struct Runner
{
	public int id;
	public Character character;
	public bool isPlayer;
}