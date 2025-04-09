using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterItem", menuName = "ScriptableObjects/CharacterItem", order = 1)]
[System.Serializable]
public class CharacterItem : Item
{
    public GameObject characterPrefab;
    public Material hair, skin, top, bottom;
    public Color hairColor, skinColor, topColor, bottomColor;
    public ObjectItem shoes;
    public Sprite editorIcon;
}
