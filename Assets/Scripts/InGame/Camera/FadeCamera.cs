using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FadeCamera : MonoBehaviour
{
    public float fadeDuration = 1.0f;
    private Coroutine currentFadeCoroutine;
    RawImage rawImage;

    public void StartFade(bool fadeIn)
    {
        Debug.Log("StartFade");
		if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);

        currentFadeCoroutine = StartCoroutine(FadeRawImageCoroutine(fadeIn));
    }

    private IEnumerator FadeRawImageCoroutine(bool fadeIn)
    {
        if (rawImage == null) rawImage = GetComponent<RawImage>();

        float startAlpha = fadeIn ? 0f : 0.5f;
        float endAlpha = fadeIn ? 0.5f : 0f;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            rawImage.color = new Color(rawImage.color.r, rawImage.color.g, rawImage.color.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rawImage.color = new Color(rawImage.color.r, rawImage.color.g, rawImage.color.b, endAlpha);
    }


}
