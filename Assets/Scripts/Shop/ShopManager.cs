using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }
    [SerializeField] private GameObject confirmationMenu;
    
    private ShopSlot slotSelected;
    public List<ShopElement> characterItems;
    public List<ShopElement> shoeItems;
    public List<ShopElement> colorItems;

    public GameObject charactersContainer;
    public GameObject shoesContainer;
    public GameObject colorsContainer;

    public GameObject shopSlotPrefab;

    public void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CleanShop();
        CreateShop();
    }

    private void CleanShop()
    {
        EmptyShopContainer(charactersContainer);
        EmptyShopContainer(shoesContainer);
        EmptyShopContainer(colorsContainer);
    }

    private void CreateShop()
    {
        SaveData.ReadFromJson();
        FillShopContainer(charactersContainer, characterItems);
        FillShopContainer(shoesContainer, shoeItems);
        FillShopContainer(colorsContainer, colorItems);
    }

    private void EmptyShopContainer(GameObject container)
    {
        foreach (Transform slot in container.transform) {
            Destroy(slot.gameObject);
        }
    }

    private void FillShopContainer(GameObject categoryContainer, List<ShopElement> categoryItems)
    {
        for (int i = 0; i < categoryItems.Count; i++) {
            ShopElement currentItem = categoryItems[i];
            GameObject slot = Instantiate(shopSlotPrefab, categoryContainer.transform);
            ShopSlot shopSlot = slot.GetComponent<ShopSlot>();
            shopSlot.SetShopItemData(currentItem);
        }
    }

    public void PurchaseItem()
    {
        GameObject tienda = charactersContainer;
        //Add item to purchased
        switch (slotSelected.shopItemElement.itemType)
        {
            case ItemType.SKIN:
                SaveData.player.purchasedSkins.Add(slotSelected.shopItemElement.itemID);
                tienda = charactersContainer;
                break;
            case ItemType.SHOE:
                SaveData.player.purchasedShoes.Add(slotSelected.shopItemElement.itemID);
                tienda = shoesContainer;
                break;
            case ItemType.COLOR:
                SaveData.player.purchasedColors.Add(slotSelected.shopItemElement.itemID);
                tienda = colorsContainer;
                break;
        }

        //Update Money
        SaveData.player.normalCoins -= slotSelected.shopItemElement.itemPrice;
        GetComponent<UIManager>().UpdateCoins();

        //Save
        SaveData.SaveToJson();

        //Change the item to purchased aspect
        slotSelected.SetOwned(true);
    }

    public void ActivateConfirmPurchaseMenu(ShopSlot slot)
    {
        GetComponent<UIManager>().OpenMenu(confirmationMenu);
        slotSelected = slot;

        //Read
        SaveData.ReadFromJson();

        if (SaveData.player.normalCoins >= slot.shopItemElement.itemPrice)
        {
            confirmationMenu.GetComponentInChildren<TextMeshProUGUI>().text = "Do you want to buy this item?";
            confirmationMenu.transform.GetChild(1).gameObject.SetActive(true);
            confirmationMenu.transform.GetChild(2).gameObject.SetActive(true);
            confirmationMenu.transform.GetChild(3).gameObject.SetActive(false);
        }
        else
        {
            confirmationMenu.GetComponentInChildren<TextMeshProUGUI>().text = "You don't have enough money";
            confirmationMenu.transform.GetChild(1).gameObject.SetActive(false);
            confirmationMenu.transform.GetChild(2).gameObject.SetActive(false);
            confirmationMenu.transform.GetChild(3).gameObject.SetActive(true);
        }
    }

    /*
    public void ScrollUntilItem()
    {
        StartCoroutine(ScrollViewFocusFunctions.FocusOnItemToLeftCoroutine(scrollRect, item, 0.5f));
    }*/
}
