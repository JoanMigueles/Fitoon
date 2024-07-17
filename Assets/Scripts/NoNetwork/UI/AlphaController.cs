using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaController : MonoBehaviour
{
    [SerializeField] private int alphaOnPress = 100; // Valor alfa al pulsar el bot�n (0-255)
    [SerializeField] private int alphaDefault = 0; // Valor alfa por defecto (0-255)

    private Button[] buttons; // Array de botones
    private Image[] images; // Array de im�genes correspondientes a los botones
    private int activeButtonIndex = 0; // �ndice del bot�n actualmente activo

    private void Start()
    {
        buttons = GetComponentsInChildren<Button>();

        images = new Image[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            images[i] = buttons[i].GetComponent<Image>();
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // Captura el �ndice actual
            buttons[i].onClick.AddListener(() => OnButtonPressed(index));
        }

        // El primer bot�n empieza activo
        SetImageAlpha(images[1], alphaOnPress);
        activeButtonIndex = 0;

        for (int i = 1; i < images.Length; i++)
        {
            SetImageAlpha(images[i], alphaDefault);
        }
    }

    private void OnButtonPressed(int index)
    {
        // Restablece el alfa del bot�n activo
        SetImageAlpha(images[activeButtonIndex], alphaDefault);

        // Establece el alfa del nuevo bot�n activo
        SetImageAlpha(images[index], alphaOnPress);
        activeButtonIndex = index; // Actualiza el �ndice del bot�n activo
    }

    private void SetImageAlpha(Image image, int alpha)
    {
        // Normaliza el valor de alfa de 0-255 a 0-1
        float normalizedAlpha = Mathf.Clamp(alpha, 0, 255) / 255f;

        Color color = image.color;
        color.a = normalizedAlpha;
        image.color = color;
    }
}
