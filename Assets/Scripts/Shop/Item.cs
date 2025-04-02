using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
    public int itemID;
    public ItemType itemType;
    public string itemName;
    public int itemPrice;
}

public enum ItemType
{
    SKIN,
    COLOR,
    SHOE
}
