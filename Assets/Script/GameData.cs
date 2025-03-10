using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class GameData
{
    public float volume;
    public float mouseSensibility;
    public bool widescreen;

    public GameData(Player player, AudioMixer audioMixer, bool widescreen) 
    {
        audioMixer.GetFloat("volume", out volume);
        mouseSensibility = player.GetSensibility();
        this.widescreen = widescreen;
    }
}
