using System.Collections;
using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    Transform focusTransform;
    AudioSource glitchSound;

    private void Awake()
    {
        focusTransform = transform.Find("Focus");
        glitchSound = GetComponentInChildren<AudioSource>();

        if (!focusTransform)
        {
            Debug.LogError("Cant find transform");
            enabled = false;
        }
        if (!glitchSound)
        {
            Debug.LogError("Cant find audio glitch audio");
            enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(EndGameCoroutine(other));
    }

    IEnumerator EndGameCoroutine(Collider other) 
    {
        Player player = other.GetComponent<Player>();

        if (player)
        {
            // Focus player camera on hanging rope
            player.FocusPlayerCamera(focusTransform);
            // Play glitching sound
            glitchSound.Play();
        }

        yield return new WaitForSeconds(1.0f);

#if UNITY_EDITOR
        glitchSound.Stop();
#endif

        Debug.Log("Game Ended");
        Application.Quit();

        yield return null;
    }
}
