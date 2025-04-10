using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to store information about the meshes, materials and icons for the shoes.
/// </summary>
[CreateAssetMenu(fileName = "ObjectParameterList", menuName = "ScriptableObjects/ObjectParameterList", order = 8)]
[System.Serializable]
public class ObjectParameterList : ScriptableObject
{
	public Sprite[] icons;
	public Mesh[] meshes;
	public Material[] materials;
}