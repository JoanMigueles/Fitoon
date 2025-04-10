using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class is used to change the colors and shoes of the character.
/// </summary>
public class CharacterPrefabColorChanger : MonoBehaviour
{
    [SerializeField] Material hairMaterial;
    [SerializeField] Material skinMaterial;
    [SerializeField] Material topMaterial;
    [SerializeField] Material bottomMaterial;
    [SerializeField] SkinnedMeshRenderer shoe;
    Color hColor;
    Color sColor;
    Color tColor;
    Color bColor;
	private void Awake()
	{
		hColor = hairMaterial.color;
        sColor = skinMaterial.color;
        tColor = topMaterial.color;
        bColor = bottomMaterial.color;
	}
	/// <summary>
	/// This method is used to change the colors of the character. It will change the colors of the hair, skin, top and bottom of the character.
	/// </summary>
	public void ChangeColors(Color hairColor, Color skinColor, Color topColor, Color bottomColor)
    {
        Debug.Log($"[CHARLOAD]Cambiando colores: {hairColor} {skinColor} {topColor} {bottomColor}");
        List<GameObject> children = GetAllChildrenRecursive(gameObject);
        foreach(GameObject child in children)
        {
			SkinnedMeshRenderer renderer = child.GetComponent<SkinnedMeshRenderer>();
			if (renderer == null)
                continue;

            for(int i = 0; i < renderer.materials.Length - 2; i++)
            {
                if (renderer.materials[i].color == hColor)
                {
                    renderer.materials[i].color = hairColor;
                }
                if (renderer.materials[i].color == sColor)
                {
                    renderer.materials[i].color = skinColor;
                }
                if (renderer.materials[i].color == tColor)
                {
                    renderer.materials[i].color = topColor;
                }
                if (renderer.materials[i].color == bColor)
                {
                    renderer.materials[i].color = bottomColor;
                }
            }
        }
        hColor = hairColor;
        sColor = skinColor;
        tColor = topColor;
        bColor = bottomColor;
    }
    List<GameObject> GetAllChildrenRecursive(GameObject gameObject)
    {
        List<GameObject> children = new List<GameObject>();
        for(int i =0; i < gameObject.transform.childCount;i++) 
        {
            children.Add(gameObject.transform.GetChild(i).gameObject);
            children.AddRange(GetAllChildrenRecursive(gameObject.transform.GetChild(i).gameObject));
        }
        return children;
    }
	/// <summary>
	/// This method is used to change the shoes of the character. It will change the mesh and materials of the shoes.
	/// </summary>
	public void ChangeShoe(Mesh mesh, Material[] materials)
    {
        shoe.sharedMesh = mesh;
        shoe.materials = materials;
    }
}
