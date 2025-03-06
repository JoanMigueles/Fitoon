using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Android;

public static class SaveData
{
    public static PlayerData player;
    static string filePath = Path.Combine(Application.persistentDataPath, "PlayerData.json");
    public static void SaveToJson()
    {
        if (player == null)
        {
            player = new PlayerData();
            ReadFromJson();
        }

        string playerData = JsonUtility.ToJson(player);
        System.IO.File.WriteAllText(filePath, playerData);
        //Si está vacío poner un personaje default
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
            Debug.Log("No había datos. Creando personaje por defecto.");
        }
        /*
        player.username = "Username";
        player.normalCoins = 0;
        player.expPoints = 0;*/
        System.IO.File.WriteAllText(filePath, playerData);
        Debug.Log("[SAVE] Datos guardados en " + filePath);
    }

    public static void ReadFromJson()
    {
        try
        {
            string playerData = System.IO.File.ReadAllText(filePath);

            player = JsonUtility.FromJson<PlayerData>(playerData);
            Debug.Log("[SAVE] Datos leidos");
        }
        catch (System.Exception)
        {
            Debug.Log("No existe JSON: Creandolo...");
            SaveToJson();
            ReadFromJson();
        }

    }
}
