using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
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
    //[Header("Timer")]
    //[SerializeField] LocalizedString localStringElapsedTime;
    //[SerializeField] TextMeshProUGUI timerText;
    //Timer timer;

    CanvasGroup canvasGroup;
    Player player;

    public delegate void OnMenuToggle(bool toggle);
    public static event OnMenuToggle onMenuToggle;

    private bool changeLanguageActive = false;

    private void OnDisable()
    {
        //localStringElapsedTime.StringChanged -= UpdateText;

        if (player)
        {
            player.GetInput().Player.Exit.performed -= ToggleMenu;
        }

        StopAllCoroutines();
    }

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
            player.GetInput().Player.Exit.performed += ToggleMenu;
        }
        else
        {
            Debug.LogError("Cant find player, menu disabled");
            enabled = false;
        }

        // Searches for timer class
        //timer = FindAnyObjectByType<Timer>();

        //if (!timer)
        //{
        //    Debug.LogError("Cant find timer");
        //}
        //else
        //{
        //    // Set the argument for the timer text
        //    localStringElapsedTime.Arguments = new object[] { timer.GetElapsedTime() };
        //    localStringElapsedTime.StringChanged += UpdateText;
        //    StartCoroutine(UpdateTimer());
        //}

        // Tries to load saved settings
        LoadGame();
    }

    //IEnumerator UpdateTimer()
    //{
    //    while (true)
    //    {
    //        Updates timer text
    //        int elapsedTime = Mathf.FloorToInt(timer.GetElapsedTime());

    //        int minutes = elapsedTime / 60;
    //        int seconds = elapsedTime % 60;
    //        localStringElapsedTime.Arguments[0] = string.Format("{0:00}:{1:00}", minutes, seconds);
    //        localStringElapsedTime.RefreshString();
    //        yield return new WaitForSeconds(0.1f);
    //    }
    //}

    // Open and close menu
    private void ToggleMenu(InputAction.CallbackContext context)
    {
        if (canvasGroup.alpha == 0)
        {
            //Unlocks mouse
            Cursor.lockState = CursorLockMode.None;
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
            // Locks mouse
            Cursor.lockState = CursorLockMode.Locked;
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

    // Menu functions

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

    // Localization functions

    // Get current language
    public int GetCurrentLocale()
    {
        UnityEngine.Localization.Locale currentLocale = LocalizationSettings.SelectedLocale;

        return LocalizationSettings.AvailableLocales.Locales.IndexOf(currentLocale);
    }

    // Change language
    public void ChangeLocale(int localeID)
    {
        // if SetLocale coroutine is active, then return
        if (changeLanguageActive) return;

        StartCoroutine(SetLocale(localeID));
    }

    // Change language coroutine
    IEnumerator SetLocale(int localeID)
    {
        changeLanguageActive = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
        SaveGame();
        changeLanguageActive = false;
    }
    
    //private void UpdateTimerText(string value)
    //{
    //    timerText.text = value;
    //}

    // Game data functions

    private void LoadGame() 
    {
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

                // Load language data
                ChangeLocale(gameData.language);
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

    private void SaveGame()
    {
        Save.SaveData(player, audioMixer, widescreenToggle.isOn, GetCurrentLocale());
    }
}
