using UnityEngine.Audio;

[System.Serializable]
public class GameData
{
    public float volume;
    public float mouseSensibility;
    public bool widescreen;
    public int language;

    public GameData(Player player, AudioMixer audioMixer, bool widescreen, int language) 
    {
        audioMixer.GetFloat("volume", out volume);
        mouseSensibility = player.GetSensibility();
        this.widescreen = widescreen;
        this.language = language;
    }
}
