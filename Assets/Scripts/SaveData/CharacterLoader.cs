using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLoader : Object
{
    static CharacterDataList characterDataList;
    public static Character GetCharacter(CharacterData data)
    {
        Character characterStruct = new Character
        {
            prefab = null,
            name = "Error",
        };
		if (characterDataList == null)
        {
            characterDataList = Resources.Load<CharacterDataList>("CharacterDataList");
            if(characterDataList == null ) 
            {
                Debug.LogError("Character Data List Not Found");
                return characterStruct;
            }
		}
        foreach(CharacterItem character in characterDataList.characters)
        {
            if(character.itemID == data.prefabId)
            {
                characterStruct.prefab = character.characterPrefab;
            }
            if(character.shoes.itemID == data.shoes)
            {
                characterStruct.shoes = character.shoes;
            }
        }
        characterStruct.name = data.characterName;

        ColorUtility.TryParseHtmlString(data.hairColor, out characterStruct.hairColor);
        ColorUtility.TryParseHtmlString(data.skinColor, out characterStruct.skinColor);
        ColorUtility.TryParseHtmlString(data.topColor, out characterStruct.topColor);
		ColorUtility.TryParseHtmlString(data.bottomColor, out characterStruct.bottomColor);

		return characterStruct;
    }
    public static Character CreateRandomCharacter()
    {
        Character character = new Character();
        character.prefab = characterDataList.characters[Random.Range(0, characterDataList.characters.Length)].characterPrefab;
        character.name = "";
        character.skinColor = Random.ColorHSV(0.08f, 0.1f, 0.25f, 0.5f, 0.4f, 1f);
        character.hairColor = Random.ColorHSV(0, 1, 0.25f, 0.75f, 0f, 1f);
        character.topColor = Random.ColorHSV();
        character.bottomColor = Random.ColorHSV();
        character.shoes = characterDataList.characters[Random.Range(0, characterDataList.characters.Length)].shoes;
        return character;
	}
}
public struct Character
{
    public GameObject prefab;
    public int prefabId;
    public string name;
    public ObjectItem shoes;
    public Color hairColor;
    public Color skinColor;
    public Color topColor;
    public Color bottomColor;
}