using FishNet;
using FishNet.Component.Spawning;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
	int readyPlayers;
	public List<Runner> runnerData;

	[SerializeField] public TextMeshProUGUI cadenceText;
	[SerializeField] public TextMeshProUGUI velocityText;
	[SerializeField] public TextMeshProUGUI positionText;
	[SerializeField] public TextMeshProUGUI countdownText;
	[SerializeField] Transform goal;
	[SerializeField] Transform[] spawnPoints;
    [SerializeField] NetworkObject playerPrefab;
    [SerializeField] NetworkObject botPrefab;
    [SerializeField] public Countdown countdown;
    [SerializeField] string sceneName;

    public static bool addBots = false;
    
    List<BaseRunner> runners = new List<BaseRunner>();

    int positionIndex = 0;

	public override void OnStartServer()
    {
		SceneManager.OnLoadEnd += OnSceneLoaded;
		StartCoroutine(WaitForPlayers());
	}

	private void OnSceneLoaded(SceneLoadEndEventArgs args)
	{
		readyPlayers++;
	}

	public override void OnStartClient()
	{
		SceneManager.OnLoadEnd += OnSceneLoaded;
		countdown.StartCountdown();
		//GetComponent<GameManager>().enabled = false;
	}

	void FreezeAllRunners()
	{
		for (int i = 0; i < runners.Count; i++)
		{
			runners[i].Freeze();
			runners[i].canMove = false;
		}
	}
    void UnfreezeAllRunners()
    {
        for(int i = 0; i < runners.Count; i++)
        {
			Debug.Log("Unfreezing runner");
			runners[i].UnFreeze();
			runners[i].canMove = true;
		}
    }

	void SpawnRunners()
    {
		for (int i = 0; i < runnerData.Count; i++)
		{
			runnerData[i].goalReached = false;
			NetworkObject runnerObject = Instantiate(runnerData[i].connection != null ? playerPrefab : botPrefab, spawnPoints[i].position, spawnPoints[i].rotation, transform);
            BaseRunner runner = runnerObject.GetComponent<BaseRunner>();
            Debug.Log("Adding runner: " + runnerData[i].characterData.characterName);
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
                    characterData = CharacterLoader.CreateRandomCharacterData(),
					name = "Runner #" + runnerData.Count.ToString().PadLeft(2, '0')
				}
            );
        }
    }

	[ServerRpc(RequireOwnership = false)]
    public void GoalReached(int id)
	{
		Debug.Log("Goal reached by " + id);
		Runner runner = runnerData.Find((r) => r.id == id);
		BaseRunner runnerObject = runners.Find((r) => r.GetId() == id);

		if(positionIndex == 0)
		{
			StartFinishCountdownRpc();
		}
		runner.goalReached = true;
		positionIndex++;
		try
		{
			(runnerObject as PlayerController).SetPosition(positionIndex, runnerData.Count);
		}
		catch (InvalidCastException)
		{
			Debug.Log("Runner is not a player");
		}
		if (positionIndex >= runnerData.Count)
		{
			StopAllCoroutines();
			positionIndex = 0;
			runnerData.Clear();

			FinishRace();
		}
	}

	void FinishRace()
	{
		LobbyManager.runnerData = new List<Runner>();

		SceneLoadData sld = new SceneLoadData("LobbyScene");
		SceneManager.LoadGlobalScenes(sld);

		SceneUnloadData sud = new SceneUnloadData(sceneName);
		SceneManager.UnloadGlobalScenes(sud);
	}

	[ObserversRpc(ExcludeServer = false)]
	void StartFinishCountdownRpc()
	{
		StartCoroutine(FinishCountdown());
	}

	IEnumerator FinishCountdown()
	{
		int countdown = 10;
		while (countdown > 0)
		{
			countdownText.text = countdown.ToString();
			yield return new WaitForSeconds(1);
			countdown--;
		}
		if (IsServerInitialized)
		{
			FreezeAllRunners();
			SortRunners();
			yield return new WaitForSeconds(1);
			FinishRace();
		}
	}

	void SortRunners()
	{
		List<Runner> sortedRunners = runnerData.OrderBy((runner) =>
		{
			BaseRunner runnerObject = runners.Find((r) => r.GetId() == runner.id);
			return Vector3.Distance(runnerObject.transform.position, goal.position);
		}).ToList();
		for (int i = 0; i < sortedRunners.Count; i++)
		{
			if (!sortedRunners[i].goalReached)
			{
				positionIndex++;
				BaseRunner runnerObject = runners.Find((r) => r.GetId() == sortedRunners[i].id);
				try
				{
					(runnerObject as PlayerController).SetPosition(positionIndex, runnerData.Count);
				}
				catch (InvalidCastException)
				{
					Debug.Log("Runner is not a player");
				}
			}
		}
	}

	IEnumerator WaitForPlayers()
	{
		yield return new WaitUntil(() => readyPlayers == 2);
		yield return new WaitForSeconds(1);

		Debug.Log("All players ready");

		runnerData = LobbyManager.runnerData;
		LobbyManager.runnerData = null;

		if (addBots)
			InitializeBots();
		Debug.Log("Initialized bots");
		SpawnRunners();

		yield return StartCoroutine(WaitForCountdown());
		Debug.Log("Countdown finished");
		UnfreezeAllRunners();

	}

	public IEnumerator WaitForCountdown()
    {
        yield return new WaitUntil(() => countdown.HasFinished());
	}
}
public class Runner
{
	public int id;
    public string name;
	public NetworkConnection connection;
    public CharacterData characterData;
	public bool goalReached = false;
}