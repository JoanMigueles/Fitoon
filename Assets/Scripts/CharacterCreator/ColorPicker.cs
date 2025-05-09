using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

[Serializable]
public class ColorEvent : UnityEvent<Color> { }
public class ColorPicker : MonoBehaviour
{
    public ColorEvent OnColorPreview;
    public ColorEvent OnColorSelect;
    RectTransform Rect;
    Texture2D ColorTexture;
    public void Start()
    {
        Rect = GetComponent<RectTransform>();
        ColorTexture = GetComponent<Image>().mainTexture as Texture2D;
    }

    private void Update()
    {
        if(RectTransformUtility.RectangleContainsScreenPoint(Rect, Input.mousePosition))
        {
            Vector2 delta;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Rect, Input.mousePosition, null, out delta);

            float width = Rect.rect.width;
            float height = Rect.rect.height;
            delta += new Vector2(width * .5f, height * 0.5f);

            float x = Mathf.Clamp(delta.x / width, 0f, 1f);
            float y = Mathf.Clamp(delta.y / height, 0f, 1f);

            int texX = Mathf.RoundToInt(x * ColorTexture.width);
            int texY = Mathf.RoundToInt(y * ColorTexture.height);

            Color color = ColorTexture.GetPixel(texX, texY);

            OnColorPreview?.Invoke(color);

            if (Input.GetMouseButtonDown(0))
            {
                OnColorSelect?.Invoke(color);
            }
        }
    }
}
