using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Tienda : MonoBehaviour
{
    public List<IconTienda> itemsByCategory;
    public GameObject iconPrefab;
    public GameObject gameManager;
    public GameObject container;
    ScrollRect scrollRect;
    RectTransform item;
    void Start()
    {
        CleanShop();
        CreateShop();
        scrollRect = FindObjectOfType<ScrollRect>();
        item = container.GetComponent<RectTransform>();
    }

    private void CleanShop()
    {
        for(int i = 0; i < container.transform.childCount; i++)
        {
            Destroy(container.transform.GetChild(i).gameObject);
        }
    }
    private void CreateShop()
    {
        for(int i = 0; i < itemsByCategory.Count; i++)
        {
            GameObject iconoCreado = Instantiate(iconPrefab, container.transform);
            IconTienda iconTiendaActual = itemsByCategory[i];
            //Primer hijo: Boton. Hijos (en orden): Texto monedas y Icono monedas
            iconoCreado.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = iconTiendaActual.itemPrice.ToString();
            iconoCreado.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = iconTiendaActual.coinType;
            //segundo hijo: icono 3D
            Transform iconTransform = iconoCreado.transform.GetChild(1).GetChild(0).transform;
            //destruir hijo que ya existe
            Destroy(iconoCreado.transform.GetChild(1).GetChild(0).gameObject);
            Instantiate(iconTiendaActual.objectIcon, iconTransform.position, iconTransform.rotation, iconoCreado.transform.GetChild(1));
            //tercer hijo: nombre
            iconoCreado.transform.GetChild(2).gameObject.GetComponentInChildren<TextMeshProUGUI>().text = iconTiendaActual.itemName;
            //a�adir id
            itemsByCategory[i].itemID = i;
            iconoCreado.GetComponent<ShopItem>().iconTiendaPropio = iconTiendaActual;
        }
    }

    public void ScrollUntilItem()
    {
        StartCoroutine(ScrollViewFocusFunctions.FocusOnItemToLeftCoroutine(scrollRect, item, 0.5f));
    }

}
