using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Drawing;

public class InitialScreenCharacterLoad : MonoBehaviour
{
    [SerializeField] List<CharacterItem> characters;
    Character actualCharacter;
    [SerializeField] List<ObjectItem> shoes;
    [SerializeField] GameObject treadmillPrefab;
    GameObject treadmill;

    private void Start()
    {
        ReadCharacter();
    }

    void ReadCharacter()
    {
        SaveData.ReadFromJson();
        //Leer la skin
        CharacterData characterData = SaveData.player.playerCharacterData;
        if (characterData == null)
        {
            print("Error: No hay personaje guardado");
            //instanciar a juan
            return;
        }

        //Instanciar la cinta de correr
        treadmill = Instantiate(treadmillPrefab, new Vector3(0, 2, 0), Quaternion.Euler(0, -220, 0));

        Character character = CharacterLoader.GetCharacter(characterData);

		if (character.prefab == null)
		{
			Debug.LogError("Character Data is Null");
			return;
		}

		GameObject characterObject = Instantiate(character.prefab, new Vector3(0, .9f, 0), Quaternion.identity, treadmill.transform);

        characterObject.transform.localRotation = Quaternion.Euler(0, 180, 0);

		characterObject.GetComponent<CharacterPrefabColorChanger>().ChangeColors(character.hairColor, character.skinColor, character.topColor, character.bottomColor);
		characterObject.GetComponent<CharacterPrefabColorChanger>().ChangeShoe(ShoeLoader.GetMesh(character.shoes.itemID), ShoeLoader.getMaterials(character.shoes.materials));

        characterObject.GetComponent<Animator>().SetBool("isRunning", true);
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

    public void SaveUsername(string value)
    {
        SaveData.player.username = value;
        SaveData.SaveToJson();
    }

    public void StartGame()
    {
        DiscoveryHandler.Passcode = null;
        SessionDataHolder.Reset();
		SceneManager.LoadScene("LobbyScene");
	}
}
