using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectParameterList", menuName = "ScriptableObjects/ObjectParameterList", order = 8)]
[System.Serializable]
public class ObjectParameterList : ScriptableObject
{
	public Sprite[] icons;
	public Mesh[] meshes;
	public Material[] materials;
}