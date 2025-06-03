using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceIgnorePause : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<AudioSource>().ignoreListenerPause = true;
    }
}
