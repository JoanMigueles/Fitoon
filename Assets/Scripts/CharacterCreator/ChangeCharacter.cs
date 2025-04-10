using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using static Google.Protobuf.WellKnownTypes.Field.Types;

public class ChangeCharacter : MonoBehaviour
{
    public CharacterItem actualCharacter; //el que sale mostrado actualmente en el creador de personajes
    public CharacterItem playerCharacter; //el guardado

    [SerializeField] GameObject container;
    [SerializeField] GameObject characterSavedText;
    [SerializeField] List<CharacterItem> characters;
    [SerializeField] List<ColorItem> colors;
    [SerializeField] List<ObjectItem> shoes;
    int characterActive = 0;

    [SerializeField] ObjectItem actualShoes;
    [SerializeField] TextMeshProUGUI nameText;

    [SerializeField] private GameObject imageButtonPrefab;
    [SerializeField] private GameObject colorButtonPrefab;
    [SerializeField] private Toggle clothesToggle;
    [SerializeField] private GameObject skinsContent;
    [SerializeField] private GameObject colorContent;
    [SerializeField] private GameObject shoesContent;

    [SerializeField] private GameObject skinColorContent;
    [SerializeField] private GameObject hairColorContent;

    private void Awake()
    {
		//Leer el personaje guardado
		SaveData.ReadFromJson();
        ReadUnlockedItems();
        AddColorListeners();
        ReadCharacter();
    }

    private void ReadUnlockedItems()
    {
        // Skins
        CharacterDataList characterDataList = Resources.Load<CharacterDataList>("CharacterDataList");
        List<CharacterItem> characterItemList = characterDataList.characters.ToList();
        foreach (int skinID in SaveData.player.purchasedSkins) {
            GameObject panel = Instantiate(imageButtonPrefab, skinsContent.transform);
            Button button = panel.transform.GetChild(0).GetComponent<Button>();

            CharacterItem item = characterItemList.Find(c => c.itemID == skinID);
            button.GetComponent<Image>().sprite = item.editorIcon;
            button.onClick.AddListener(() => OnSkinClicked(skinID));
            characters.Add(item);
        }

        // Clothes colors
        ColorDataList colorDataList = Resources.Load<ColorDataList>("ColorDataList");
        List<ColorItem> colorItemList = colorDataList.colors.ToList();
        foreach (int colorID in SaveData.player.purchasedColors) {
            GameObject panel = Instantiate(colorButtonPrefab, colorContent.transform);
            Button button = panel.GetComponent<Button>();

            ColorItem item = colorItemList.Find(c => c.itemID == colorID);
            button.GetComponent<Image>().color = item.color;
            button.onClick.AddListener(() => SetClothesColor(item.color));
            colors.Add(item);
        }

        // Shoes colors
        ObjectDataList objDataList = Resources.Load<ObjectDataList>("ObjectDataList");
        List<ObjectItem> objItemList = objDataList.shoes.ToList();
        foreach (int shoeID in SaveData.player.purchasedShoes) {
            GameObject panel = Instantiate(imageButtonPrefab, shoesContent.transform);
            Button button = panel.transform.GetChild(0).GetComponent<Button>();

            ObjectItem item = objItemList.Find(c => c.itemID == shoeID);
            button.GetComponent<Image>().sprite = ShoeLoader.GetIcon(item.icon);
            button.onClick.AddListener(() => SetShoes(item));
            actualShoes = item;
            shoes.Add(item);
        }
    }

    private void AddColorListeners()
    {
        foreach (Transform colorPanel in skinColorContent.transform) {
            Button button = colorPanel.GetComponent<Button>();
            button.onClick.AddListener(() => SetSkinColor(colorPanel.GetComponent<Image>().color));
        }

        foreach (Transform colorPanel in hairColorContent.transform) {
            Button button = colorPanel.GetComponent<Button>();
            button.onClick.AddListener(() => SetHairColor(colorPanel.GetComponent<Image>().color));
        }
    }

    public void SetClothesColor(Color color)
    {
        if (clothesToggle.isOn)
        {
            actualCharacter.bottom.color = color;
        } else {
            actualCharacter.top.color = color;
        }
        
    }

    public void SetSkinColor(Color color)
    {
        actualCharacter.skin.color = color;
    }

    public void SetHairColor(Color color)
    {
        actualCharacter.hair.color = color;
    }

    //cuando se pulsa el zapato en cuestion, cambiar la malla actual por esa
    public void SetShoes(ObjectItem objectItem)
    {
        GameObject[] selectedObjects;
        selectedObjects = GameObject.FindGameObjectsWithTag("Shoes");
        foreach (GameObject obj in selectedObjects) {
            SkinnedMeshRenderer renderer = obj.GetComponent<SkinnedMeshRenderer>();
            renderer.sharedMesh = ShoeLoader.GetMesh(objectItem.mesh);
            renderer.materials = ShoeLoader.getMaterials(objectItem.materials);
            obj.GetComponent<WhatShoeIHave>().myShoe = objectItem;
            actualShoes = objectItem;
        }
    }

    public void OnSkinClicked(int skinID)
    {
        //Buscar en qu� �ndice de la lista de personajes est�, segun el NOMBRE de la skin
        characterActive = skinID;
        actualCharacter = characters.Find((c) => c.itemID == skinID);
        

        //Actualizar el personaje en pantalla
        DestroyImmediate(container.transform.GetChild(0).gameObject);
        GameObject instance = Instantiate(actualCharacter.characterPrefab, Vector3.zero, Quaternion.identity, container.transform);
        instance.GetComponent<RotateCharacter>().enabled = true;
        instance.GetComponent<Outline>().enabled = false;
        nameText.text = actualCharacter.itemName;

        UpdateShoes();
    }

    public void OnArrowClicked(string direction)
    {
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

        //Actualizar el personaje en pantalla
        DestroyImmediate(container.transform.GetChild(0).gameObject);
        GameObject instance = Instantiate(characters[characterActive].characterPrefab, Vector3.zero, Quaternion.identity, container.transform);
        instance.GetComponent<RotateCharacter>().enabled = true;
        instance.GetComponent<Outline>().enabled = false;
        nameText.text = characters[characterActive].itemName;
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
        //Buscar en qu� �ndice de la lista de personajes est�, segun el NOMBRE de la skin
        characterActive = characters.FindIndex(character => character.itemName == savedSkin);
        actualCharacter = characters[characterActive];

        //Actualizar el personaje en pantalla
        if(container.transform.childCount != 0)
        {
            DestroyImmediate(container.transform.GetChild(0).gameObject);
        }
        
        GameObject instance = Instantiate(actualCharacter.characterPrefab, Vector3.zero, Quaternion.identity, container.transform);
        instance.GetComponent<RotateCharacter>().enabled = true;
        instance.GetComponent<Outline>().enabled = false;
        nameText.text = actualCharacter.itemName;

        UpdateShoes();
        UpdateColors();
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
            if (shoeItem.itemID == i)
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
		actualCharacter.hair.color = SaveData.player.playerCharacterData.hairColor;
		actualCharacter.skin.color = SaveData.player.playerCharacterData.skinColor;
		actualCharacter.bottom.color = SaveData.player.playerCharacterData.bottomColor;
		actualCharacter.top.color = SaveData.player.playerCharacterData.topColor;
    }

    public void SaveCharacter()
    {

        SaveData.player.playerCharacterData.characterName = actualCharacter.itemName;
        SaveData.player.playerCharacterData.hairColor = actualCharacter.hair.color;
        SaveData.player.playerCharacterData.skinColor = actualCharacter.skin.color;
        SaveData.player.playerCharacterData.topColor = actualCharacter.top.color;
        SaveData.player.playerCharacterData.bottomColor = actualCharacter.bottom.color;
        SaveData.player.playerCharacterData.shoes = actualShoes.itemID;
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
}
