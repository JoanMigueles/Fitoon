using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    [HideInInspector] public Item shopItemElement;
    [SerializeField] private TextMeshProUGUI itemNameTag;
    [SerializeField] private Transform item3DPreview;
    [SerializeField] private GameObject priceButton;
    [SerializeField] private TextMeshProUGUI priceTag;
    [SerializeField] private Image currencyIcon;
    [SerializeField] private GameObject ownedTag;

    [SerializeField] private Sprite defaultPanel;
    [SerializeField] private Sprite ownedPanel;

    public void SetShopItemData(Item item)
    {
        shopItemElement = item;

        // Nombre
        itemNameTag.text = item.itemName;

        // Icono 3D
        Destroy(item3DPreview.GetChild(0).gameObject);
        if (item is CharacterItem) {
            Instantiate((item as CharacterItem).characterPrefab, item3DPreview); 
        }
        

        // Precio
        priceTag.text = item.itemPrice.ToString();

        // Comprobar si está comprado mediante el ID y cambiar la apariencia
        if (IsAlreadyOwned(item)) {
            SetOwned(true);
        }
    }

    private bool IsAlreadyOwned(Item item)
    {
        return (SaveData.player.purchasedSkins.Contains(item.itemID) && item.itemType.ToString() == "SKIN") ||
                (SaveData.player.purchasedShoes.Contains(item.itemID) && item.itemType.ToString() == "SHOE") ||
                (SaveData.player.purchasedColors.Contains(item.itemID) && item.itemType.ToString() == "COLOR");
    }

    public void SetOwned(bool owned)
    {
        priceButton.SetActive(!owned);
        ownedTag.SetActive(owned);

        if (owned) {
            GetComponent<Image>().sprite = ownedPanel;
        } else {
            GetComponent<Image>().sprite = defaultPanel;
        }
    }

    public void TryToPurchaseButton()
    {
        ShopUIManager.Instance.ActivateConfirmPurchaseMenu(this);
    }
}
