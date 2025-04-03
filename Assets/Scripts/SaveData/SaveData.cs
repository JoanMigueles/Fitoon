using System.Collections.Generic;
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
            ReadFromJson();
        }

        //Si est� vac�o poner un personaje default
        if (player.playerCharacterData is null)
        {
            player.username = GooglePlayServicesManager.instance.GetPlayerUsername();
            player.playerCharacterData = new CharacterData();
            player.playerCharacterData.characterName = "Juan";
            player.playerCharacterData.hairColor = new Color(77, 36, 19);
            player.playerCharacterData.skinColor = new Color(168, 116, 88);
            player.playerCharacterData.topColor = new Color(180, 102, 0);
            player.playerCharacterData.bottomColor = new Color(79, 47, 18);
            player.playerCharacterData.shoes = 0;
            player.playerCharacterData.prefabId = 0;
            Debug.Log("No hab�a datos. Creando personaje por defecto.");
        }
        string playerData = JsonUtility.ToJson(player);
        System.IO.File.WriteAllText(filePath, playerData);
        GooglePlayServicesManager.instance.SaveGame(playerData);
        Debug.Log("[SAVE] Datos guardados en " + filePath);
    }

    /// <summary>
    /// tries to read the player data from a json file, if not found, tries to load it from the cloud
    /// </summary>
    public static void ReadFromJson()
    {
        if(SceneManager.GetActiveScene().name == "LoggingIn")
        {
            GooglePlayServicesManager.instance.LoadGame();
            return;
        }

        try
        {
            string playerData = System.IO.File.ReadAllText(filePath);

            player = JsonUtility.FromJson<PlayerData>(playerData);
            Debug.Log("[SAVE] Datos leidos");
            if(SceneManager.GetActiveScene().name == "LoggingIn") SceneManager.LoadScene("Inicial");
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
            SaveToJson();
        }
        else
        {
            player = new PlayerData();
            SaveToJson();
        }
    }
}
