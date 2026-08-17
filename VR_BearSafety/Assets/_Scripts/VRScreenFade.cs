using System;
using System.Collections;
using UnityEngine;

public class VRScreenFade : Singleton<VRScreenFade>
{
    [SerializeField] private Renderer fadeRenderer;

    private Material fadeMaterial;

    private Color fadeColor;

    public Action FadeToCompleted;
    public Action FadeFromCompleted;

    protected override void Awake()
    {
        base.Awake();

        fadeMaterial = fadeRenderer.material;
        fadeColor = fadeMaterial.color;

        FadeToCompleted = null;
        FadeFromCompleted = null;

        SetAlpha(0.0f);
    }


    public void FadeToBlack(float duration) { StartCoroutine(FadeTo(duration)); }
    public void FadeFromBlack(float duration) { StartCoroutine(FadeFrom(duration)); }


    private IEnumerator FadeTo(float duration)
    {
        float startAlpha = fadeColor.a;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;
            float alpha = Mathf.Lerp(startAlpha, 1.0f, t);

            SetAlpha(alpha);

            yield return null;
        }

        SetAlpha(1f);
        FadeToCompleted?.Invoke();
    }

    private IEnumerator FadeFrom(float duration)
    {
        float startAlpha = fadeColor.a;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;
            float alpha = Mathf.Lerp(startAlpha, 0.0f, t);

            SetAlpha(alpha);

            yield return null;
        }

        SetAlpha(0f);
        FadeFromCompleted?.Invoke();
    }

    private void SetAlpha(float alpha)
    {
        fadeColor.a = alpha;
        fadeMaterial.color = fadeColor;
    }
}