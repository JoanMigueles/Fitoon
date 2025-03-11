using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
	int readyPlayers;
	public static GameManager Instance;
	public static List<Runner> runnerData;

	[SerializeField] Transform[] spawnPoints;
    [SerializeField] NetworkObject playerPrefab;
    [SerializeField] NetworkObject botPrefab;
    [SerializeField] Countdown countdown;
    [SerializeField] string sceneName;

    public static bool addBots = true;
    
    List<BaseRunner> runners = new List<BaseRunner>();

    static int positionIndex = 1;

	private void Start()
	{
		if(Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
	}

	public override void OnStartClient()
	{
		AmReady();
	}
	[ServerRpc(RequireOwnership = false)]
	void AmReady()
	{
		readyPlayers++;
	}
	public override void OnStartServer()
    {
		StartCoroutine(WaitForPlayers());
	}

    [ServerRpc(RequireOwnership = false)]
    void UnfreezeAllRunners()
    {
        for(int i = 0; i < runners.Count; i++)
        {
            runners[i].UnFreeze();
        }
    }

	[ObserversRpc]
	void StartCountDown()
	{
		countdown.StartCountdown();
	}

	void SpawnRunners()
    {
		if (!IsServerInitialized)
		{
			return;
		}
		for (int i = 0; i < runnerData.Count; i++)
		{
            NetworkObject runnerObject = Instantiate(runnerData[i].connection != null ? playerPrefab : botPrefab, spawnPoints[i].position, spawnPoints[i].rotation, transform);
            BaseRunner runner = runnerObject.GetComponent<BaseRunner>();
            Debug.Log("Adding runner: " + runnerData[i].characterData.characterName);
			runner.characterData = runnerData[i].characterData;
			runners.Add(runner);
			runner.SetId(runnerData[i].id);
            Debug.Log("RunnerConnection: " + runnerData[i].connection);
			Spawn(runnerObject, runnerData[i].connection);
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
                    connection = null,
                    position = -1,
                    characterData = CharacterLoader.CreateRandomCharacterData(),
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

	IEnumerator WaitForPlayers()
	{
		runnerData = LobbyManager.runnerData;
		yield return new WaitWhile(() => readyPlayers < runnerData.Count);

		Debug.Log("Current race != 0");
		if (addBots)
			InitializeBots();
		Debug.Log("Initialized bots");
		SpawnRunners();
		StartCountDown();
		StartCoroutine(WaitForCountdown());
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
    public string name;
	public NetworkConnection connection;
    public CharacterData characterData;
    public int position;
}