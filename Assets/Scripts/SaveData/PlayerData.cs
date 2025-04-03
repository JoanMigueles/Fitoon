using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string username;
    public int pfp;
    public int normalCoins;
    public int expPoints;
    public int streak;
    public int runnedDistance;
    public int medals;
    public int wins;
    public List<int> purchasedSkins;
    public List<int> purchasedShoes;
    public List<int> purchasedColors;
    public CharacterData playerCharacterData;

    public PlayerData()
    {
        purchasedSkins = new List<int>();
        purchasedShoes = new List<int>();
        purchasedColors = new List<int>();
    }
}

[System.Serializable]
public class CharacterData
{
    public string characterName;
    public Color hairColor, skinColor, topColor, bottomColor;
    public int shoes;
}

