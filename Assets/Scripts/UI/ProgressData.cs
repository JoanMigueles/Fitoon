using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProgressData", menuName = "ScriptableObjects/ProgressData", order = 6)]
[System.Serializable]
public class ProgressData : ScriptableObject
{
    public float baseXP = 100f;
    public int rankMedalInterval = 500;
    public List<Sprite> rankSprites = new List<Sprite>();
    public List<string> rankTitles = new List<string>();
}
