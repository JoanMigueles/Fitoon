using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopElement", menuName = "ScriptableObjects/ShopElement", order = 3)]
public class ShopElement : ScriptableObject
{
    public int itemID;
    public ItemType itemType;
    public GameObject objectIcon;
    public string itemName;
    public int itemPrice;
    public Sprite coinType;
    public bool isAlreadyBought;
}

public enum ItemType
{
    SKIN,
    COLOR,
    SHOE
}
