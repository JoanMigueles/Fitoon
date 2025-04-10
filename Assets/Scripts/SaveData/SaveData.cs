using System;
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
            if(!Application.isEditor) player.username = GooglePlayServicesManager.instance.GetPlayerUsername();
            else player.username = "Fitooner";
            player.playerCharacterData = new CharacterData();
            player.playerCharacterData.characterName = "Cap Guy";
            player.playerCharacterData.hairColor = new Color(77/255.0f, 36/255.0f, 19/255.0f);
            player.playerCharacterData.skinColor = new Color(168/255.0f, 116/255.0f, 88/255.0f);
            player.playerCharacterData.topColor = new Color(180/255.0f, 102/255.0f, 0/255.0f);
            player.playerCharacterData.bottomColor = new Color(79/255.0f, 47/255.0f, 18/255.0f);
            player.playerCharacterData.shoes = 0;
            Debug.Log("No hab�a datos. Creando personaje por defecto.");
        }
        string playerData = JsonUtility.ToJson(player);
        System.IO.File.WriteAllText(filePath, playerData);
        if(!Application.isEditor)GooglePlayServicesManager.instance.SaveGame(playerData);
        Debug.Log("[SAVE] Datos guardados en " + filePath);
        DatabaseManager.instance.UpdatePlayerData();
    }

    /// <summary>
    /// tries to read the player data from a json file, if not found, tries to load it from the cloud
    /// </summary>
    public static void ReadFromJson()
    {
        if(!Application.isEditor && SceneManager.GetActiveScene().name == "LoggingIn")
        {
            GooglePlayServicesManager.instance.LoadGame();
            return;
        }
        try
        {
            string playerData = System.IO.File.ReadAllText(filePath);
            Debug.Log("[LOAD] Datos leidos desde " + filePath);
            player = JsonUtility.FromJson<PlayerData>(playerData);
            if(player.playerCharacterData.hairColor.r > 1 || player.playerCharacterData.hairColor.g > 1 || player.playerCharacterData.hairColor.b > 1)
            {
                player.playerCharacterData.hairColor = new Color(player.playerCharacterData.hairColor.r / 255.0f, player.playerCharacterData.hairColor.g / 255.0f, player.playerCharacterData.hairColor.b / 255.0f);
            }
            if(player.playerCharacterData.skinColor.r > 1 || player.playerCharacterData.skinColor.g > 1 || player.playerCharacterData.skinColor.b > 1)
            {
                player.playerCharacterData.skinColor = new Color(player.playerCharacterData.skinColor.r / 255.0f, player.playerCharacterData.skinColor.g / 255.0f, player.playerCharacterData.skinColor.b / 255.0f);
            }
            if(player.playerCharacterData.topColor.r > 1 || player.playerCharacterData.topColor.g > 1 || player.playerCharacterData.topColor.b > 1)
            {
                player.playerCharacterData.topColor = new Color(player.playerCharacterData.topColor.r / 255.0f, player.playerCharacterData.topColor.g / 255.0f, player.playerCharacterData.topColor.b / 255.0f);
            }
            if(player.playerCharacterData.bottomColor.r > 1 || player.playerCharacterData.bottomColor.g > 1 || player.playerCharacterData.bottomColor.b > 1)
            {
                player.playerCharacterData.bottomColor = new Color(player.playerCharacterData.bottomColor.r / 255.0f, player.playerCharacterData.bottomColor.g / 255.0f, player.playerCharacterData.bottomColor.b / 255.0f);
            }
            Debug.Log("[SAVE] Datos leidos");
            if(SceneManager.GetActiveScene().name == "LoggingIn") SceneManager.LoadScene("Inicial");
        }
        catch (System.Exception)
        {
            Debug.Log("No existe JSON: Creandolo buscando en la nube");
            if(!Application.isEditor) GooglePlayServicesManager.instance.LoadGame();
            else ReceiveData(null);
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
            Debug.Log("[CHAR]" + player.playerCharacterData.hairColor + " " + player.playerCharacterData.skinColor + " " + player.playerCharacterData.topColor + " " + player.playerCharacterData.bottomColor);
            SaveToJson();
        }
        else
        {
            player = new PlayerData();
            SaveToJson();
        }
    }

    public static void ChangeUsername(string username, Action<bool> callback)
    {
        DatabaseManager.instance.CheckUsername(username, (result) =>
        {
           if(!result)
           {
                DatabaseManager.instance.DeletePlayerData();
                player.username = username;
                SaveToJson();
                callback(true);
              }
              else
              {
                callback(false);
              }
        });
    }
}
