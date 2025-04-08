using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Color", menuName = "ScriptableObjects/ColorItem", order = 3)]
[System.Serializable]
public class ColorItem : Item
{
    public Color color;
}
