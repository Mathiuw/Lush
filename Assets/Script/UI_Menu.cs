using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_Menu : MonoBehaviour
{
    [Header("Volume")]
    [SerializeField] Slider volumeSlider;
    [SerializeField] AudioMixer audioMixer;
    [Header("Mouse Sensibility")]
    [SerializeField] Slider mouseSenseSlider;
    [Header("Widescreen")]
    [SerializeField] Toggle widescreenToggle;
    [SerializeField] Transform borders;
    [Header("Timer")]
    [SerializeField]TextMeshProUGUI timerText;
    Timer timer;

    CanvasGroup canvasGroup;
    Player player;

    public delegate void OnMenuToggle(bool toggle);
    public static event OnMenuToggle onMenuToggle;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;
    }

    private void Start()
    {
        // Searches for player class
        player = FindAnyObjectByType<Player>();

        // Bind event to player pause bind if he exists
        if (player)
        {
            player.GetInputActions_Player().Player.Exit.performed += ToggleMenu;
        }
        else
        {
            Debug.LogError("Menu cant find player, menu disabled");
            enabled = false;
        }

        //Searches for timer class
        timer = FindAnyObjectByType<Timer>();

        if (!timer)
        {
            Debug.LogError("Menu cant find timer");
        }

        // Check if have any save files, if have, load then
        if (player && audioMixer)
        {
            GameData gameData = Save.LoadData();

            if (gameData != null)
            {
                // Load volume data
                SetVolume(gameData.volume);
                volumeSlider.value = gameData.volume;

                // Load mouse sensibility data
                SetMouseSensibility(gameData.mouseSensibility);
                mouseSenseSlider.value = gameData.mouseSensibility;

                // Load widescreen data
                ToggleWidescreen(gameData.widescreen);
                widescreenToggle.isOn = gameData.widescreen;
            }
            else
            {
                // Set volume slider to its audio mixer value
                audioMixer.GetFloat("volume", out float volumeValue);
                volumeSlider.value = volumeValue;

                // Set mouse sense slider to its audio mixer value
                mouseSenseSlider.value = player.GetSensibility();
            }
        }
        else Debug.LogError("An error occurred when trying load an save");
    }

    private void Update()
    {
        int elapsedTime = Mathf.FloorToInt(timer.GetElapsedTime());

        int minutes = elapsedTime / 60;
        int seconds = elapsedTime % 60;
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetVolume(float volume) 
    {
        audioMixer.SetFloat("volume", volume);
        SaveGame();
    }

    public void SetMouseSensibility(float sensibility) 
    {
        player.SetSensibiity(sensibility);
        SaveGame();
    }

    public void ToggleWidescreen(bool toggle) 
    {
        if (borders)
        {
            borders.gameObject.SetActive(!toggle);
            SaveGame();
        }
        else Debug.LogError("Cant find UI borders");
    }

    private void ToggleMenu(InputAction.CallbackContext context)
    {
        if (canvasGroup.alpha == 0)
        {
            // Pause the game
            Time.timeScale = 0;
            // Disable audio
            AudioListener.pause = true;
            // Turn the menu visible
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            // Call Menu toggle event
            onMenuToggle?.Invoke(true);
        }
        else
        {
            // Unpause the game
            Time.timeScale = 1;
            // Enable audio
            AudioListener.pause = false;
            // Turn the menu invisible
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            // Call Menu toggle event
            onMenuToggle?.Invoke(false);
        }
    }

    private void SaveGame() 
    {
        Save.SaveData(player, audioMixer, widescreenToggle.isOn);
    }
}
