using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class ChangeCharacter : MonoBehaviour
{
    public CharacterItem actualCharacter; //el que sale mostrado actualmente en el creador de personajes
    public CharacterItem playerCharacter; //el guardado

    [SerializeField] GameObject optionsPanels;
    [SerializeField] GameObject container;
    [SerializeField] GameObject characterSavedText;
    [SerializeField] List<CharacterItem> characters;
    [SerializeField] List<ObjectItem> shoes;
    int characterActive = 0;
    [SerializeField] ObjectItem actualShoes;
    [SerializeField] TextMeshProUGUI nameText;


    private void Awake()
    {
		//Leer el personaje guardado
		SaveData.ReadFromJson();
        ReadCharacter();
        
    }

    public void OnSkinClicked(string skinName)
    {
        for(int i = 0; i < optionsPanels.transform.childCount; i++)
        {
            if (optionsPanels.transform.GetChild(i).gameObject.activeInHierarchy)
            {
                optionsPanels.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        //Buscar en qué índice de la lista de personajes está, segun el NOMBRE de la skin
        characterActive = characters.FindIndex(character => character.characterName == skinName);
        actualCharacter = characters[characterActive];

        //Actualizar el personaje en pantalla
        DestroyImmediate(container.transform.GetChild(0).gameObject);
        GameObject instance = Instantiate(actualCharacter.prefab, Vector3.zero, Quaternion.identity, container.transform);
        instance.GetComponent<RotateCharacter>().enabled = true;
        instance.GetComponent<Outline>().enabled = false;
        nameText.text = actualCharacter.characterName;

        UpdateShoes();
    }

    public void OnArrowClicked(string direction)
    {
        DestroyImmediate(container.transform.GetChild(0).gameObject);
        for (int i = 0; i < optionsPanels.transform.childCount; i++)
        {
            if (optionsPanels.transform.GetChild(i).gameObject.activeInHierarchy)
            {
                optionsPanels.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

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

        UpdateShoes();
    }

    public void OnShoeClicked(ObjectItem shoeItem)
    {
        actualShoes = shoeItem;
    }

    public void ResetCharacter()
    {
        characters[characterActive].hair.color = characters[characterActive].hairColor;
        characters[characterActive].skin.color = characters[characterActive].skinColor;
        characters[characterActive].top.color = characters[characterActive].topColor;
        characters[characterActive].bottom.color = characters[characterActive].bottomColor;

        UpdateShoes();

    }

    public void ReadCharacter()
    {
        //Buscar la skin
        string savedSkin = SaveData.player.playerCharacterData.characterName;
        if(savedSkin == null)
        {
            actualCharacter = characters[0];
            characterActive = 0;
            Debug.LogError("Error: No hay personaje guardado");
            return;
        }
        //Buscar en qué índice de la lista de personajes está, segun el NOMBRE de la skin
        characterActive = characters.FindIndex(character => character.characterName == savedSkin);
        actualCharacter = characters[characterActive];

        //Actualizar el personaje en pantalla
        if(container.transform.childCount != 0)
        {
            DestroyImmediate(container.transform.GetChild(0).gameObject);
        }
        
        GameObject instance = Instantiate(actualCharacter.prefab, Vector3.zero, Quaternion.identity, container.transform);
        instance.GetComponent<RotateCharacter>().enabled = true;
        instance.GetComponent<Outline>().enabled = false;
        nameText.text = actualCharacter.characterName;

        UpdateShoes();
        UpdateColors();
        

        //Asignar colores guardados (cuando haga reset deben salir estos)
        /* Color color = Color.black; //si falla saldrá negro
         if (ColorUtility.TryParseHtmlString(saveData.player.playerCharacterData.hairColor, out color))
         {
             actualCharacter.hairColor = color;
             playerCharacter.hairColor = color;
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
         }*/


        //scriptable object con estos datos
        /*playerCharacter.characterName = actualCharacter.characterName;
          playerCharacter.prefab = actualCharacter.prefab;
          playerCharacter.hair = actualCharacter.hair;
          playerCharacter.skin = actualCharacter.skin;
          playerCharacter.top = actualCharacter.top;
          playerCharacter.bottom = actualCharacter.bottom;
          playerCharacter.hairColor = actualCharacter.hairColor;
          playerCharacter.skinColor = actualCharacter.skinColor;
          playerCharacter.topColor = actualCharacter.topColor;
          playerCharacter.bottomColor = actualCharacter.bottomColor;*/
    }
    void UpdateShoes()
    {
        //Actualizar zapatillas
        GameObject zapatos = GameObject.FindGameObjectWithTag("Shoes");
        SkinnedMeshRenderer renderer = zapatos.GetComponent<SkinnedMeshRenderer>();
        int i = SaveData.player.playerCharacterData.shoes;

      //  Debug.Log($"ANTES: Zapato GO: {zapatos.name}. Mesh rendered: {renderer.sharedMesh}. ActualShoe id: {i}");

        foreach (ObjectItem shoeItem in shoes)
        {
            if (shoeItem.id == i)
            {
                renderer.sharedMesh = ShoeLoader.GetMesh(shoeItem.mesh);
                renderer.materials = ShoeLoader.getMaterials(shoeItem.materials);
                zapatos.GetComponent<WhatShoeIHave>().myShoe = shoeItem;
                actualShoes = zapatos.GetComponent<WhatShoeIHave>().myShoe;
                break;
            }
        }

      //  Debug.Log($"DESPUES: Zapato GO: {zapatos.name}. Mesh rendered: {renderer.sharedMesh}. ActualShoe id: {i}");
    }

    void UpdateColors()
    {
        Color color = Color.black; //si falla saldrá negro
        if (ColorUtility.TryParseHtmlString(SaveData.player.playerCharacterData.hairColor, out color))
        {
            actualCharacter.hair.color = color;
        }
        if (ColorUtility.TryParseHtmlString(SaveData.player.playerCharacterData.skinColor, out color))
        {
            actualCharacter.skin.color = color;
        }
        if (ColorUtility.TryParseHtmlString(SaveData.player.playerCharacterData.bottomColor, out color))
        {
            actualCharacter.bottom.color = color;
        }
        if (ColorUtility.TryParseHtmlString(SaveData.player.playerCharacterData.topColor, out color))
        {
            actualCharacter.top.color = color;
        }
    }

    public void SaveCharacter()
    {
        for (int i = 0; i < optionsPanels.transform.childCount; i++)
        {
            if (optionsPanels.transform.GetChild(i).gameObject.activeInHierarchy)
            {
                optionsPanels.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        SaveData.player.playerCharacterData.characterName = actualCharacter.characterName;
        SaveData.player.playerCharacterData.hairColor = ColorToHex(actualCharacter.hair.color);
        SaveData.player.playerCharacterData.skinColor = ColorToHex(actualCharacter.skin.color);
        SaveData.player.playerCharacterData.topColor = ColorToHex(actualCharacter.top.color);
        SaveData.player.playerCharacterData.bottomColor = ColorToHex(actualCharacter.bottom.color);
        SaveData.player.playerCharacterData.shoes = actualShoes.id;
        SaveData.SaveToJson();
        //saveData.ReadFromJson();
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
