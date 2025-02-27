using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShoeLoader
{
    static ObjectParameterList shoeParams;
    public static Mesh GetMesh(int shoeIndex)
    {
        if (shoeParams == null)
        {
            shoeParams = Resources.Load<ObjectParameterList>("ShoeParameters");
        }
        return shoeParams.meshes[shoeIndex];
    }
    public static Material[] getMaterials(int[] matIndices)
    {
		if (shoeParams == null)
		{
			shoeParams = Resources.Load<ObjectParameterList>("ShoeParameters");
		}
        Material[] result = new Material[matIndices.Length];
        for(int i = 0; i < matIndices.Length; i++)
        {
            result[i] = shoeParams.materials[matIndices[i]];
        }
        return result;
	}
}
[CreateAssetMenu(fileName = "ObjectItem", menuName = "ScriptableObjects/ObjectItem", order = 2)]
[System.Serializable]
public class ObjectItem : ScriptableObject
{
    public int id;
    public int icon;
    public int mesh;
    public int[] materials;
}

[CreateAssetMenu(fileName = "ObjectParameterList", menuName = "ScriptableObjects/ObjectParameterList", order = 3)]
[System.Serializable]
public class ObjectParameterList : ScriptableObject
{
	public Sprite[] icons;
	public Mesh[] meshes;
	public Material[] materials;
}