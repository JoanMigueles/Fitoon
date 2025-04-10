using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "BannerList", menuName = "ScriptableObjects/BannerList", order = 8)]
[System.Serializable]
public class BannerList : ScriptableObject
{
	public Sprite[] Banners;
    public Color[] Colors;
}
