using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorDataList", menuName = "ScriptableObjects/ColorDataList", order = 10)]
[System.Serializable]
public class ColorDataList : ScriptableObject
{
    public ColorItem[] colors;
}
