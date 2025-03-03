using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveData
{
    public static PlayerData player;
    static string filePath = Path.Combine(Application.persistentDataPath, "PlayerData.json");

    /// <summary>
    /// Saves the player data to a json file, if its the first time, tries to load a Json before saving
    /// </summary>
    public static void SaveToJson()
    {
        if (player == null)
        {
            player = new PlayerData();
            ReadFromJson();
        }

        string playerData = JsonUtility.ToJson(player);
        System.IO.File.WriteAllText(filePath, playerData);
        //Si est� vac�o poner un personaje default
        if (player.playerCharacterData is null)
        {
            player.playerCharacterData = new CharacterData();
            player.playerCharacterData.characterName = "Juan";
            player.playerCharacterData.hairColor = "#4D2413";
            player.playerCharacterData.skinColor = "#A87458";
            player.playerCharacterData.topColor = "#B46600";
            player.playerCharacterData.bottomColor = "#4F2F12";
            player.playerCharacterData.shoes = 0;
            player.playerCharacterData.prefabId = 0;
            playerData = JsonUtility.ToJson(player);
            Debug.Log("No hab�a datos. Creando personaje por defecto.");
        }
        player.username = GooglePlayServicesManager.instance.GetPlayerUsername();
        player.normalCoins = 0;
        player.points = 0;
        System.IO.File.WriteAllText(filePath, playerData);
        GooglePlayServicesManager.instance.SaveGame(playerData);
        Debug.Log("[SAVE] Datos guardados en " + filePath);
    }

    /// <summary>
    /// tries to read the player data from a json file, if not found, tries to load it from the cloud
    /// </summary>
    public static void ReadFromJson()
    {
        try
        {
            string playerData = System.IO.File.ReadAllText(filePath);

            player = JsonUtility.FromJson<PlayerData>(playerData);
            Debug.Log("[SAVE] Datos leidos");
            GooglePlayServicesManager.instance.SaveGame(playerData);
        }
        catch (System.Exception)
        {
            Debug.Log("No existe JSON: Creandolo buscando en la nube");
            GooglePlayServicesManager.instance.LoadGame();
        }

    }

    /// <summary>
    /// Recieves the player data from the cloud, if none is found, creates a new one
    /// </summary>
    /// <param name="playerData"></param>
    public static void ReceiveData(string playerData)
    {
        if(playerData != null)
        {
            player = JsonUtility.FromJson<PlayerData>(playerData);
            Debug.Log("[LOAD] Datos recibidos desde la nube");
            ReadFromJson();
        }
        else
        {
            player = new PlayerData();
            SaveToJson();
            ReadFromJson();
        }
    }
}
