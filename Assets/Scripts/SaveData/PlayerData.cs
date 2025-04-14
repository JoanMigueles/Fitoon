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
    public int bannerID;
    public int gymKey;
    public string title;
    public List<int> purchasedSkins;
    public List<int> purchasedShoes;
    public List<int> purchasedColors;
    public List<int> unlockedBanners;
    public List<int> unlockedIcons;
    public List<string> unlockedTitles;
    public CharacterData playerCharacterData;

    public PlayerData()
    {
        purchasedSkins = new List<int> { 0 };
        purchasedShoes = new List<int> { 0 };
        purchasedColors = new List<int> { 0, 2, 4, 7, 10, 13, 14, 16 };

        unlockedBanners = new List<int> { 0, 1, 2 };
        unlockedIcons = new List<int> { 0, 1, 2, 3 };
        unlockedTitles = new List<string> { "Novice Runner", "Fitooner" };
        title = "Novice Runner";
    }
}

[System.Serializable]
public class CharacterData
{
    public string characterName;
    public Color hairColor, skinColor, topColor, bottomColor;
    public int shoes;
}

