using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Drawing;

public class InitialScreenCharacterLoad : MonoBehaviour
{
    [SerializeField] GameObject characterContainer;
    [SerializeField] List<CharacterItem> characters;
    CharacterItem actualCharacter;
    [SerializeField] List<ObjectItem> shoes;
    [SerializeField] GameObject treadmillPrefab;

    private void Start()
    {
        ReadCharacter();
    }

    void ReadCharacter()
    {
        //Leer la skin
        string savedSkin = SaveData.player.playerCharacterData.characterName;
        if (savedSkin == null)
        {
            print("Error: No hay personaje guardado");
            //instanciar a juan
            Instantiate(characters[0].characterPrefab, Vector3.zero, Quaternion.identity, characterContainer.transform);
            return;
        }
        //Buscar personaje
        actualCharacter = characters.Find(character => character.itemName == savedSkin);

        //Instanciar la cinta de correr
        GameObject treadmill = Instantiate(treadmillPrefab, Vector3.zero, Quaternion.identity, characterContainer.transform);

        //Instanciar el personaje como hijo de la cinta
        DestroyImmediate(characterContainer.transform.GetChild(0).gameObject);
        GameObject characterInstance = Instantiate(actualCharacter.characterPrefab, Vector3.zero, Quaternion.identity, treadmill.transform);
        characterInstance.GetComponent<Animator>().SetBool("isRunning", true);
        characterInstance.GetComponent<Outline>().enabled = false;

        UpdateShoes();
       // UpdateColors();

        //Colocar a personaje adecuadamente en la cinta
        characterInstance.transform.Rotate(transform.up, 180f);
        characterInstance.transform.position = new Vector3(0, 2.54f, 1.6f);

        //Para alejarlo un poco de la camara
        characterContainer.transform.position = new Vector3(0, 0, -2.91f);
        characterContainer.transform.Rotate(transform.up, 120f);
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

    public void SaveUsername(string value)
    {
        SaveData.player.username = value;
        SaveData.SaveToJson();
    }

    public void ResetScenesPlayed()
    {
        SaveData.player.scenesPlayed.Clear();
        SaveData.SaveToJson();
    }

    public void StartGame()
    {
        DiscoveryHandler.Passcode = null;
        SessionDataHolder.Reset();
		SceneManager.LoadScene("LobbyScene");
	}
}
