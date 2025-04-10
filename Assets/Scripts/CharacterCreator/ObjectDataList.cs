using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectDataList", menuName = "ScriptableObjects/ObjectDataList", order = 11)]
[System.Serializable]
public class ObjectDataList : ScriptableObject
{
    public ObjectItem[] shoes;
}
