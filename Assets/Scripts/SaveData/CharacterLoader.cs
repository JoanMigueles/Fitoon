using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to load character data and create character instances.
/// </summary>
public class CharacterLoader : Object
{
    static CharacterDataList characterDataList;

	/// <summary>
	/// Given a CharacterData object, this method retrieves the corresponding character prefab, shoes and colors.
    /// </summary>
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
            if(character.itemName == data.characterName)
            {
                characterStruct.prefab = character.characterPrefab;
            }
            if(character.shoes.itemID == data.shoes)
            {
                characterStruct.shoes = character.shoes;
            }
        }
        characterStruct.name = data.characterName;
        if(characterStruct.prefab == null)
		{
			Debug.LogError("Character Prefab Not Found");
			characterStruct.prefab = characterDataList.characters[0].characterPrefab;
		}

		//Debug.Log("COLOR TEST COLOR TEST COLOR TEST");
		//Debug.Log(data.topColor + " " + data.bottomColor);
		//Debug.Log(characterStruct.topColor + " " + characterStruct.bottomColor);


		characterStruct.hairColor = data.hairColor;
        characterStruct.skinColor = data.skinColor;
        characterStruct.topColor = data.topColor;
        characterStruct.bottomColor = data.bottomColor;

		return characterStruct;
    }

	/// <summary>
	/// Returns a random character data object.
    /// </summary>
	public static CharacterData CreateRandomCharacterData()
    {
        if(characterDataList == null)
        {
            characterDataList = Resources.Load<CharacterDataList>("CharacterDataList");
        }
        CharacterData characterData = new CharacterData();
        characterData.characterName = characterDataList.characters[Random.Range(0, characterDataList.characters.Length)].itemName;
        characterData.skinColor = Random.ColorHSV(0.08f, 0.1f, 0.25f, 0.5f, 0.4f, 1f);
        characterData.hairColor = Random.ColorHSV(0, 1, 0.25f, 0.75f, 0f, 1f);
        characterData.topColor = Random.ColorHSV();
        characterData.bottomColor = Random.ColorHSV();
        characterData.shoes = characterDataList.characters[Random.Range(0, characterDataList.characters.Length)].shoes.itemID;
        return characterData;
	}
}

/// <summary>
/// Struct that holds the data necessary to instantiate a character.
/// </summary>
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