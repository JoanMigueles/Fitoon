using FishNet.Connection;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
	public static List<Runner> runnerData;

	[SerializeField] Transform[] spawnPoints;
    [SerializeField] PlayerController playerPrefab;
    [SerializeField] BotRunner botPrefab;
    [SerializeField] Countdown countdown;
    [SerializeField] string sceneName;

    public static bool addBots = true;
    
    List<BaseRunner> runners;

    static int positionIndex = 1;

	private void Start()
	{
		if(Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
	}

	public override void OnStartNetwork()
    {
        if(IsServerInitialized)
        {
			Debug.Log("Current race != 0");
            if(addBots)
			    InitializeBots();
            Debug.Log("Initialized bots");
			SpawnRunners();

			countdown.StartCountdown();
			StartCoroutine(WaitForCountdown());
		}
        else
        {
			countdown.StartCountdown();
		}
	}

    [ServerRpc]
    void UnfreezeAllRunners()
    {
        for(int i = 0; i < runners.Count; i++)
        {
            runners[i].UnFreeze();
        }
    }

	void SpawnRunners()
    {
		for (int i = 0; i < runnerData.Count; i++)
		{
            if (runnerData[i].connection != null)
            {
                PlayerController player = Instantiate(playerPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
                player.SetId(runnerData[i].id);
                player.LoadCharacter(runnerData[i].character);
                runners.Add(player);
                Spawn(player.gameObject, runnerData[i].connection);
			}
            else
            {
                BotRunner runner = Instantiate(botPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
                runner.SetId(runnerData[i].id);
                runner.LoadCharacter(runnerData[i].character);
                runners.Add(runner);
                Spawn(runner.gameObject);
            }
		}
	}

    void InitializeBots()
    {
        while (runnerData.Count < 32)
        {
            runnerData.Add
            (
                new Runner
                {
                    id = runnerData.Count,
                    character = CharacterLoader.CreateRandomCharacter(),
                    connection = null,
                    position = -1
                }
            );
        }
    }

	[ServerRpc]
    public void GoalReached(BaseRunner baseRunner)
	{
		Runner runner = new Runner();
		try
		{
			runner = runnerData.Find((r) => r.id == baseRunner.GetId());
		}
		catch (ArgumentNullException)
		{
			Debug.LogError("Runner not found");
			return;
		}
		runner.position = positionIndex++;
		if (positionIndex > runnerData.Count)
		{
			positionIndex = 1;
			runnerData.Clear();

			//GO TO GAME OVER SCENE
		}
	}

	[ServerRpc]
    public void AddPlayer(NetworkConnection connection, Character character)
    {
        runnerData.Add(new Runner()
        {
            id = connection.ClientId,
            character = character,
            connection = connection,
            position = -1
        });
    }

    [ServerRpc]
    public void RemovePlayer(NetworkConnection connection)
    {
        runnerData.Remove(runnerData.Find((Runner r) => r.id == connection.ClientId));
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
	public NetworkConnection connection;
    public int position;
}