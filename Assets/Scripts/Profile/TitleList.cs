using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "TitleList", menuName = "ScriptableObjects/TitleList", order = 12)]
[System.Serializable]
public class TitleList : ScriptableObject
{
    public string[] Titles;
}