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
    [SerializeField] Color fadeColor;
    [SerializeField] float fadeTime = 1;
    [SerializeField] bool active;
    RawImage image;

    private void Start()
    {
        image = GetComponentInChildren<RawImage>();

        if (!image)
        {
            Debug.LogError("UI_Fade cant find RawImage, disabling component");
            enabled = false;
        }
    }

    public void SetFadeValue(float value) 
    {
        fadeType = FadeType.Paused;

        value = Mathf.Clamp01(value);

        Color imageColor = image.color;
        imageColor.a = value;

        image.color = imageColor;
    }

    private void Update()
    {
        Color imageColor = image.color;

        switch (fadeType)
        {
            case FadeType.FadeIn:
                imageColor.a += Time.deltaTime / fadeTime;
                break;
            case FadeType.FadeOut:
                imageColor.a -= Time.deltaTime / fadeTime;
                break;
            case FadeType.Paused:
                break;
            default:
                break;
        }

        imageColor.a = Mathf.Clamp(imageColor.a, 0, 1);

        image.color = imageColor;
    }
}
