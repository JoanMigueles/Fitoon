using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string username;
    public int pfp;
    public int normalCoins;
    public int points;
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
    public Color hairColor, skinColor, topColor, bottomColor;
    public int shoes;
}

