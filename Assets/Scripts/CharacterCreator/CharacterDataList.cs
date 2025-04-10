using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the characters in the game.
/// </summary>
[CreateAssetMenu(fileName = "CharacterDataList", menuName = "ScriptableObjects/CharacterDataList", order = 5)]
[System.Serializable]
public class CharacterDataList : ScriptableObject
{
    public CharacterItem[] characters;
}
