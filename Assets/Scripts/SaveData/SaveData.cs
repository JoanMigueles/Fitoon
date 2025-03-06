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
            ResetPlayerData();
        }

        //Si está vacío poner un personaje default
        if (player.playerCharacterData is null)
        {
            player.playerCharacterData = new CharacterData() {
                characterName = "Juan",
                hairColor = "#4D2413",
                skinColor = "#A87458",
                topColor = "#B46600",
                bottomColor = "#4F2F12",
                shoes = 0,
                prefabId = 0
            };
            Debug.Log("No había datos. Creando personaje por defecto.");
        }

        string playerData = JsonUtility.ToJson(player);
        System.IO.File.WriteAllText(filePath, playerData);
        Debug.Log("[SAVE] Datos guardados en " + filePath);
    }

    public static void ReadFromJson()
    {
        if (File.Exists(filePath)) {
            try {
                string playerData = System.IO.File.ReadAllText(filePath);
                player = JsonUtility.FromJson<PlayerData>(playerData);
                Debug.Log("[SAVE] Datos leidos");
            }
            catch (System.Exception) {
                Debug.Log("Error obteniendo datos");
            }
        }
        else {
            Debug.Log("No Json found, creating...");
            SaveToJson();
            ReadFromJson();
        }
    }

    public static void ResetPlayerData()
    {
        player = new PlayerData() {
            username = "Fitooner",
            purchasedSkins = new List<int>(),
            purchasedShoes = new List<int>(),
            purchasedColors = new List<int>()
        };
    }
}
