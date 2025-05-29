using UnityEngine;
using UnityEngine.UI;

public class End_Trigger : MonoBehaviour
{
    RawImage fade;

    private void Awake()
    {
        fade = GameObject.Find("Fade").GetComponent<RawImage>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (fade != null)
        {
            fade.color = new Color(0, 0, 0, 1);
        }
        else
        {
            Debug.LogError("Cant find fade image");
        }

        Debug.Log("Game Ended");

        enabled = false;
    }
}
