using FishNet;
using FishNet.Connection;
using FishNet.Discovery;
using FishNet.Object;
using FishNet.Transporting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPlayer : NetworkBehaviour
{
	public override void OnStartClient()
	{
		if (!IsOwner) return;

		SaveData.ReadFromJson();
		StartCoroutine(DelayHostConnection());
	}
	IEnumerator DelayHostConnection()
	{
		yield return new WaitForSeconds(0.1f);
		FindFirstObjectByType<LobbyManager>().AddPlayer(InstanceFinder.ClientManager.Connection, SaveData.player.username, SaveData.player.title, SaveData.player.pfp, SaveData.player.bannerID, SaveData.player.medals, SaveData.player.playerCharacterData);
	}
}
