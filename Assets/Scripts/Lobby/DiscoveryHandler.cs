using FishNet.Discovery;
using FishNet.Managing;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class DiscoveryHandler : MonoBehaviour
{
	public static DiscoveryHandler Instance;
	public static string Passcode;
	NetworkDiscovery netDiscovery;
	[SerializeField] NetworkManager networkManager;
	[SerializeField] ushort port = 7077;

	void Start()
	{
		if (!SessionDataHolder.lookForLobby)
		{
			return;
		}
		netDiscovery = networkManager.GetComponent<NetworkDiscovery>();
		if (Passcode != null)
		{
			netDiscovery.ChangeSecret(Passcode);
		}
		Debug.Log(Passcode);
		netDiscovery.ServerFoundCallback += ConnectToServer;
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		BeginSearch();
	}

	void OnDestroy()
	{
		netDiscovery.ServerFoundCallback -= ConnectToServer;
	}

	public void BeginSearch()
	{
		Debug.Log("Searching. . . ");
		netDiscovery.SearchForServers();
		StartCoroutine(SearchTimeOut());
	}

	private void ConnectToServer(IPEndPoint point)
	{
		StopAllCoroutines();
		netDiscovery.StopSearchingOrAdvertising();
		networkManager.ClientManager.StartConnection(point.Address.ToString(), port);
	}

	IEnumerator SearchTimeOut()
	{
		yield return new WaitForSeconds(Random.Range(2, 3f));
		if (netDiscovery.IsSearching)
		{
			netDiscovery.ServerFoundCallback -= ConnectToServer;
			netDiscovery.StopSearchingOrAdvertising();
			networkManager.ServerManager.StartConnection(port);
			netDiscovery.AdvertiseServer();
		}
	}
}
