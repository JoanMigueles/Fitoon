using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to load the shoe meshes and materials.
/// </summary>
public static class ShoeLoader
{
	static ObjectParameterList shoeParams;
	/// <summary>
	/// Given the index of a shoe, this method retrieves the corresponding mesh.
	/// </summary>
	public static Mesh GetMesh(int shoeIndex)
	{
		if (shoeParams == null)
		{
			shoeParams = Resources.Load<ObjectParameterList>("ObjectParameterList");
		}
		return shoeParams.meshes[shoeIndex];
	}
	/// <summary>
	/// Given an array of shoe material indices, this method retrieves the corresponding materials.
	/// </summary>
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

	/// <summary>
	/// Given the index of a shoe, this method retrieves the corresponding icon.
	/// </summary>
	public static Sprite GetIcon(int iconIndex)
	{
        if (shoeParams == null) {
            shoeParams = Resources.Load<ObjectParameterList>("ObjectParameterList");
        }
        return shoeParams.icons[iconIndex];
    }
}