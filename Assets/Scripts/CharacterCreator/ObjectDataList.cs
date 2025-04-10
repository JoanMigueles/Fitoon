using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to store the shoes.
/// </summary>
[CreateAssetMenu(fileName = "ObjectDataList", menuName = "ScriptableObjects/ObjectDataList", order = 11)]
[System.Serializable]
public class ObjectDataList : ScriptableObject
{
    public ObjectItem[] shoes;
}
