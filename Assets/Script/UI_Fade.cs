using UnityEngine;
using UnityEngine.UI;

public class UI_Fade : MonoBehaviour
{
    enum FadeType
    {
        FadeIn,
        FadeOut
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
            default:
                break;
        }

        imageColor.a = Mathf.Clamp(imageColor.a, 0, 1);

        image.color = imageColor;
    }
}
