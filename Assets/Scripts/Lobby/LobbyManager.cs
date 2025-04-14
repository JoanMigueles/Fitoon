using FishNet;
using FishNet.Connection;
using FishNet.Discovery;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Object.Synchronizing.Internal;
using FishNet.Transporting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// This class is used to manage the lobby. It handles player entries, ready states, and starting the game.
/// </summary>
public class LobbyManager : NetworkBehaviour
{
	/// <summary>
	/// This class is used to store the data necessary to render each player's banner in the lobby.
	/// </summary>
	public class PlayerCard
	{
		public string name;
		public string title;
		public bool ready;
		public int score;
		public int pfp;
		public int banner;
	}

	public static List<Runner> runnerData = new List<Runner>();

	readonly SyncDictionary<NetworkConnection, PlayerCard> playerEntries = new SyncDictionary<NetworkConnection, PlayerCard>();

	[SerializeField] GameObject cardPrefab;
	[SerializeField] ScrollRect scrollRect;
	[SerializeField] public GameObject content;
	[SerializeField] TextMeshProUGUI countDownText;
	[SerializeField] TextMeshProUGUI playerCountText;
	[SerializeField] GameObject lobbyPlayerPrefab;

	List<LeaderboardField> cardList = new List<LeaderboardField>();

	bool starting = false;

	private void Awake()
	{
		runnerData.Clear();
	}

	private void OnApplicationQuit()
	{
		InstanceFinder.NetworkManager.ClientManager.StopConnection();
		InstanceFinder.NetworkManager.ServerManager.StopConnection(true);
		Destroy(InstanceFinder.NetworkManager.gameObject);
	}

	private void Update()
	{
		// The update loop is used to render the player cards client-side.
		List<PlayerCard> playerCards = playerEntries.Values.ToList();
		playerCards = playerCards.OrderByDescending(p => p.score).ToList();
		int i;
		for(i = 0; i  < playerCards.Count; i++)
		{
			if(i >= cardList.Count)
			{
				LeaderboardField card = Instantiate(cardPrefab, content.transform).GetComponent<LeaderboardField>();
                cardList.Add(card);
			}

			cardList[i].SetBanner(playerCards[i].banner);
			cardList[i].SetPlayerName(playerCards[i].name);
			cardList[i].SetReady(playerCards[i].ready);
			cardList[i].SetProfilePicture(playerCards[i].pfp);
			cardList[i].SetMedals(playerCards[i].score);
			cardList[i].SetPlayerTitle(playerCards[i].title);
		}
		for(int j = cardList.Count - 1; j >= i; j--)
		{
			Destroy(cardList[j].gameObject);
			cardList.RemoveAt(j);
		}
	}

	public override void OnStartServer()
	{
		InstanceFinder.NetworkManager.ClientManager.StartConnection();
		InstanceFinder.ServerManager.OnRemoteConnectionState += OnRemoteConnectionState;
		playerEntries.OnChange += OnChangePlayerEntries;
		Debug.Log("I'm a Server!");
	}

	public override void OnStartClient()
	{
		Debug.Log("Score: " + SaveData.player.medals);

		if (!SessionDataHolder.lookForLobby)
		{
			SpawnPlayer(InstanceFinder.ClientManager.Connection);
		}
		SessionDataHolder.lookForLobby = false;
	}

	[ServerRpc(RequireOwnership = false)]
	void SpawnPlayer(NetworkConnection connection)
	{
		GameObject player = Instantiate(lobbyPlayerPrefab);
		Spawn(player, connection);
	}

	private void OnRemoteConnectionState(NetworkConnection connection, RemoteConnectionStateArgs args)
	{
		if(args.ConnectionState == RemoteConnectionState.Stopped)
		{
			playerEntries.Remove(connection);
			runnerData.Remove(runnerData.Find((r) => r.connection == connection));
		}
	}

	private void OnChangePlayerEntries(SyncDictionaryOperation op, NetworkConnection key, PlayerCard value, bool asServer)
	{
		if(!IsServerInitialized)
		{
			return;
		}
		SetPlayerNumber(playerEntries.Count);
		CheckReady();
	}

	public void ReadyButton()
	{
		SetReady(InstanceFinder.ClientManager.Connection);
	}

	public void ExitButton()
	{
		if (InstanceFinder.NetworkManager != null)
		{
			InstanceFinder.NetworkManager.ClientManager.StopConnection();
			InstanceFinder.NetworkManager.ServerManager.StopConnection(true);
		}
		Destroy(InstanceFinder.NetworkManager.gameObject);
		UnityEngine.SceneManagement.SceneManager.LoadScene(1);
	}

	[ServerRpc(RequireOwnership = false)]
	public void AddPlayer(NetworkConnection key, string name, string title, int pfp, int banner, int score, CharacterData characterData)
	{
		PlayerCard card = new PlayerCard()
		{
			name = name,
			ready = false,
			pfp = pfp,
			score = score,
			banner = banner,
			title = title
		};
		playerEntries.Add(key, card);
		playerEntries.Dirty(key);

		runnerData.Add(new Runner()
		{
			id = key.ClientId,
			name = name,
			connection = key,
			characterData = characterData
		});
	}

	[ServerRpc(RequireOwnership = false)]
	void SetReady(NetworkConnection connection)
	{
		PlayerCard card;
		if (playerEntries.TryGetValue(connection, out card))
			card.ready = !card.ready;
		playerEntries.Dirty(connection);
	}

	[ServerRpc(RequireOwnership = false)]
	public void CheckReady()
	{
		if (!IsServerInitialized)
			return;
		int ready = 0;
		foreach (PlayerCard card in playerEntries.Values)
		{
			if (card.ready)
			{
				ready++;
			}
		}
		if (ready / (float)playerEntries.Count >= 0.6f)
		{
			if (starting)
				return;

			InstanceFinder.NetworkManager.GetComponent<NetworkDiscovery>().StopSearchingOrAdvertising();
			starting = true;
			StartCoroutine(StartGameCountdown());
		}
		else
		{
			ChangeCountdownText("WAITING FOR PLAYERS");
			InstanceFinder.NetworkManager.GetComponent<NetworkDiscovery>().AdvertiseServer();
			starting = false;
		}
	}

	[ObserversRpc]
	public void ChangeCountdownText(string s)
	{
		countDownText.text = s;
	}

	[ObserversRpc]
	public void SetPlayerNumber(int i)
	{
		playerCountText.text = i.ToString() + "/32";
	}

	IEnumerator StartGameCountdown()
	{
		if (!starting)
			yield break;
		for (int i = 5; i >= 0; i--)
		{


			ChangeCountdownText("STARTING IN " + i.ToString());
			Debug.Log("Countdown: " + i.ToString());
			yield return new WaitForSeconds(1);
			if (!starting)
				yield break;
		}
		SceneLoadData sld = new SceneLoadData("FindingScenario");
		SceneManager.LoadGlobalScenes(sld);

		SceneUnloadData sud = new SceneUnloadData("LobbyScene");
		SceneManager.UnloadGlobalScenes(sud);
	}
}
