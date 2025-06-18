using UnityEngine;
using UnityEngine.UI;

public class UI_Fade : MonoBehaviour
{
    enum FadeType
    {
        FadeIn,
        FadeOut,
        Paused
    }

    [SerializeField] FadeType fadeType;
    float fadeValue = 0.0f;
    RawImage image;

    public float GetFadeValue() 
    {
        return fadeValue;
    }

    public void SetFadeValue(float value)
    {
        fadeType = FadeType.Paused;

        value = Mathf.Clamp01(value);

        Color imageColor = image.color;
        imageColor.a = value;

        image.color = imageColor;
    }

    private void Start()
    {
        image = GetComponentInChildren<RawImage>();

        if (!image)
        {
            Debug.LogError("UI_Fade cant find RawImage, disabling component");
            enabled = false;
        }
    }

    private void Update()
    {
        Color imageColor = image.color;

        switch (fadeType)
        {
            case FadeType.FadeIn:
                imageColor.a += Time.deltaTime;
                break;
            case FadeType.FadeOut:
                imageColor.a -= Time.deltaTime;
                break;
            case FadeType.Paused:
                break;
            default:
                break;
        }

        imageColor.a = Mathf.Clamp01(imageColor.a);

        fadeValue = imageColor.a;

        image.color = imageColor;
    }
}
