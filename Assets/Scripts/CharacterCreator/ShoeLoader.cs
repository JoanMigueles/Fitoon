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
			shoeParams = Resources.Load<ObjectParameterList>("ObjectParameterList");
		}
		return shoeParams.meshes[shoeIndex];
	}
	public static Material[] getMaterials(int[] matIndices)
	{
		if (shoeParams == null)
		{
			shoeParams = Resources.Load<ObjectParameterList>("ObjectParameterList");
		}
		Material[] result = new Material[matIndices.Length];
		for (int i = 0; i < matIndices.Length; i++)
		{
			result[i] = shoeParams.materials[matIndices[i]];
		}
		return result;
	}

	public static Sprite GetIcon(int iconIndex)
	{
        if (shoeParams == null) {
            shoeParams = Resources.Load<ObjectParameterList>("ObjectParameterList");
        }
        return shoeParams.icons[iconIndex];
    }
}