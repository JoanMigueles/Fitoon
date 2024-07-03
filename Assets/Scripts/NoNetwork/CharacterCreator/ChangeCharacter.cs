using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class ChangeCharacter : MonoBehaviour
{
    public CharacterItem actualCharacter; //el que sale mostrado actualmente en el creador de personajes
    public CharacterItem playerCharacter; //el guardado

    [SerializeField] GameObject container;
    [SerializeField] GameObject characterSavedText;
    [SerializeField] List<CharacterItem> characters;
    int characterActive = 0;
    [SerializeField] TextMeshProUGUI nameText;

    SaveData saveData;

    private void Awake()
    {
        //Leer el personaje guardado
        saveData = GetComponent<SaveData>();
        saveData.ReadFromJson();
        ReadCharacter();
    }

    public void OnSkinClicked(string skinName)
    {
        //Buscar en qu� �ndice de la lista de personajes est�, segun el NOMBRE de la skin
        characterActive = characters.FindIndex(character => character.characterName == skinName);
        actualCharacter = characters[characterActive];

        //Actualizar el personaje en pantalla
        Destroy(container.transform.GetChild(0).gameObject);
        GameObject instance = Instantiate(actualCharacter.prefab, Vector3.zero, Quaternion.identity, container.transform);
        instance.GetComponent<RotateCharacter>().enabled = true;
        instance.GetComponent<Outline>().enabled = false;
        nameText.text = actualCharacter.characterName;
    }

    public void OnArrowClicked(string direction)
    {
        Destroy(container.transform.GetChild(0).gameObject);

        if (direction == "RIGHT")
        {
            characterActive++;

            if (characterActive == characters.Count)
            {
                characterActive = 0;
            }
        }

        else if (direction == "LEFT")
        {
            characterActive--;

            if (characterActive < 0)
            {
                characterActive = characters.Count - 1;
            }
        }

        GameObject instance = Instantiate(characters[characterActive].prefab, Vector3.zero, Quaternion.identity, container.transform);
        instance.GetComponent<RotateCharacter>().enabled = true;
        instance.GetComponent<Outline>().enabled = false;
        nameText.text = characters[characterActive].characterName;

        actualCharacter = characters[characterActive];
    }

    public void ResetCharacter()
    {
        characters[characterActive].hair.color = characters[characterActive].hairColor;
        characters[characterActive].skin.color = characters[characterActive].skinColor;
        characters[characterActive].top.color = characters[characterActive].topColor;
        characters[characterActive].bottom.color = characters[characterActive].bottomColor;

        GameObject[] shoes = GameObject.FindGameObjectsWithTag("Shoes");

        foreach (GameObject shoe in shoes)
        {
            SkinnedMeshRenderer renderer = shoe.GetComponent<SkinnedMeshRenderer>();
            renderer.sharedMesh = characters[characterActive].shoes.mesh;
            renderer.materials = characters[characterActive].shoes.materials;
        }

    }

    public void ReadCharacter()
    {
        //Buscar la skin
        string savedSkin = saveData.player.playerCharacterData.characterName;
        if(savedSkin == null)
        {
            actualCharacter = characters[0];
            characterActive = 0;
            Debug.LogError("Error: No hay personaje guardado");
            return;
        }
        //Buscar en qu� �ndice de la lista de personajes est�, segun el NOMBRE de la skin
        characterActive = characters.FindIndex(character => character.characterName == savedSkin);
        actualCharacter = characters[characterActive];

        //Actualizar el personaje en pantalla
        Destroy(container.transform.GetChild(0).gameObject);
        GameObject instance = Instantiate(actualCharacter.prefab, Vector3.zero, Quaternion.identity, container.transform);
        instance.GetComponent<RotateCharacter>().enabled = true;
        instance.GetComponent<Outline>().enabled = false;
        nameText.text = actualCharacter.characterName;

        //Asignar colores guardados (cuando haga reset deben salir estos)
        Color color = Color.black; //si falla saldr� negro
        if (ColorUtility.TryParseHtmlString(saveData.player.playerCharacterData.hairColor, out color))
        {
            actualCharacter.hairColor = color;
        }
        if (ColorUtility.TryParseHtmlString(saveData.player.playerCharacterData.skinColor, out color))
        {
            actualCharacter.skinColor = color;
        }
        if (ColorUtility.TryParseHtmlString(saveData.player.playerCharacterData.bottomColor, out color))
        {
            actualCharacter.bottomColor = color;
        }
        if (ColorUtility.TryParseHtmlString(saveData.player.playerCharacterData.topColor, out color))
        {
            actualCharacter.topColor = color;
        }

        //scriptable object con estos datos
        playerCharacter.characterName = actualCharacter.characterName;
        playerCharacter.prefab = actualCharacter.prefab;
        playerCharacter.hair = actualCharacter.hair;
        playerCharacter.skin = actualCharacter.skin;
        playerCharacter.top = actualCharacter.top;
        playerCharacter.bottom = actualCharacter.bottom;
        playerCharacter.hairColor = actualCharacter.hairColor;
        playerCharacter.skinColor = actualCharacter.skinColor;
        playerCharacter.topColor = actualCharacter.topColor;
        playerCharacter.bottomColor = actualCharacter.bottomColor;
    }

    public void SaveCharacter()
    {
        saveData.player.playerCharacterData.characterName = actualCharacter.characterName;
        saveData.player.playerCharacterData.hairColor = ColorToHex(actualCharacter.hair.color);
        saveData.player.playerCharacterData.skinColor = ColorToHex(actualCharacter.skin.color);
        saveData.player.playerCharacterData.topColor = ColorToHex(actualCharacter.top.color);
        saveData.player.playerCharacterData.bottomColor = ColorToHex(actualCharacter.bottom.color);
        saveData.SaveToJson();
        saveData.ReadFromJson();
        ReadCharacter();
        StartCoroutine(CharacterSavedText());
    }

    IEnumerator CharacterSavedText()
    {
        characterSavedText.SetActive(true);
        yield return new WaitForSeconds(2);
        characterSavedText.SetActive(false);
    }

    private void AsignColors()
    {
        //Para hacer reset de todos los personajes al darle al play
        foreach (CharacterItem character in characters)
        {
            character.hairColor = character.hair.color;
            character.skinColor = character.skin.color;
            character.topColor = character.top.color;
            character.bottomColor = character.bottom.color;
        }
    }

    public static string ColorToHex(Color color)
    {
        // Convert RGB values to hexadecimal format
        int r = (int)(color.r * 255f);
        int g = (int)(color.g * 255f);
        int b = (int)(color.b * 255f);

        // Format the hexadecimal string
        string hex = string.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);

        return hex;
    }

}
