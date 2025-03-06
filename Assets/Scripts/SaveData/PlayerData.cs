using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string username;
    public int normalCoins;
    public int expPoints;
    public int streak;
    public int runnedDistance;
    public int medals;
    public List<int> purchasedSkins;
    public List<int> purchasedShoes;
    public List<int> purchasedColors;
    public CharacterData playerCharacterData;
    public List<EscenarioItem> scenesPlayed;
}

[System.Serializable]
public class CharacterData
{
    public string characterName;
    public int prefabId;
    public string hairColor, skinColor, topColor, bottomColor; //hex color
    public int shoes;
}

