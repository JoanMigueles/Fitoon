using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    [HideInInspector] public ShopElement shopItemElement;
    [SerializeField] private TextMeshProUGUI itemNameTag;
    [SerializeField] private Transform item3DPreview;
    [SerializeField] private GameObject priceButton;
    [SerializeField] private TextMeshProUGUI priceTag;
    [SerializeField] private Image currencyIcon;
    [SerializeField] private GameObject ownedTag;

    public void SetShopItemData(ShopElement item)
    {
        shopItemElement = item;

        // Nombre
        itemNameTag.text = item.itemName;

        // Icono 3D
        Destroy(item3DPreview.GetChild(0).gameObject);
        Instantiate(item.objectIcon, item3DPreview);

        // Precio
        priceTag.text = item.itemPrice.ToString();
        currencyIcon.sprite = item.coinType;

        // Comprobar si está comprado mediante el ID y cambiar la apariencia
        if (IsAlreadyOwned(item)) {
            SetOwned(true);
        }
    }

    private bool IsAlreadyOwned(ShopElement item)
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
            GetComponent<Image>().color = new Color(235f / 255f, 90f / 255f, 210f / 255f);
        } else {
            GetComponent<Image>().color = Color.white;
        }
    }

    public void TryToPurchaseButton()
    {
        ShopManager.Instance.ActivateConfirmPurchaseMenu(this);
    }
}
