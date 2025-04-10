using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PFPList", menuName = "ScriptableObjects/PFPList", order = 7)]
[System.Serializable]
public class PFPList : ScriptableObject
{
    public Sprite[] sprites;
}
