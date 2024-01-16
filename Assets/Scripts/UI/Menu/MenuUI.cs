using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour, ISubscriber<GameOverSignal>, ISubscriber<PauseSignal>, ISubscriber<LevelUpdateSignal>
{
    #region
    // Main Canvas
    [SerializeField] private Canvas _mainCanvas;
    private CanvasScaler _mainCanvasScaler;
    private Vector2 _defaultUIResolution;

    // Saving settings
    private string SAVE_PATH_SETTINGS;

    // AudioMixer
    [SerializeField] private AudioMixer _audioMixer;
    private const string MIXER_MENU_BACKGROUND_MUSIC = "MenuBackground";
    private const string MIXER_GAME_BACKGROUND_MUSIC = "GameBackground";
    private const string MIXER_SFX = "SFX";
    private bool _menuPlaying = true;

    // Main Menu
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _continueBtn;

    // Pause Menu
    [SerializeField] private GameObject _pauseMenu;

    // Game Over Menu
    [SerializeField] private GameObject _gameOverMenu;
    [SerializeField] private TMP_InputField _nameInput;
    private string _highscoreName;
    private int _score;
    [SerializeField] private TMP_Text _finalScoreText;

    // Highscore Menu
    [SerializeField] private GameObject _highscorePanel;
    private CanvasGroup _highscoreCanvas;
    [SerializeField] private GameObject _highscoreEntryContainer;
    // Using two different prefabs, so Image components do not have be accessed everytime as well
    [SerializeField] private GameObject _highscoreEntraDarkPrefab;
    [SerializeField] private GameObject _highscoreEntraLightPrefab;

    // Settings UI Elements
    [SerializeField] private GameObject _settingsPanel;
    private CanvasGroup _settingsCanvas;
    [SerializeField] private TMP_Dropdown _srSetting;
    [SerializeField] private TMP_Dropdown _dmSetting;
    [SerializeField] private Slider _usSetting;
    [SerializeField] private TextMeshProUGUI _usSettingText;
    [SerializeField] private Slider _mvSetting;
    [SerializeField] private TextMeshProUGUI _mvSettingText;
    [SerializeField] private Slider _svSetting;
    [SerializeField] private TextMeshProUGUI _svSettingText;

    // List of all images used for main menu buttons and pause menu buttons -> cannot be accessed via EventTrigger, because Unity does not allow for that..
    [SerializeField] private List<Image> _images = new();

    // Settings Content
    private List<TMPro.TMP_Dropdown.OptionData> _resolutions = new();
    private List<TMPro.TMP_Dropdown.OptionData> _displayModes = new();

    private float _fadeInTime = 0.7f;
    private float _fadeOutTime = 0.7f;

    private GameManager _gameManager;
    #endregion

    /*--- UNITY FUNCTIONS -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    #region
    private void Awake()
    {
        _gameManager = GameManager.Instance;
        // Showing or not showing continue button depending on whether a game to continue exists
        _continueBtn.SetActive(File.Exists(_gameManager.SAVE_PATH_GAME_INFORMATION));
        SAVE_PATH_SETTINGS = Path.Combine(Application.persistentDataPath, "superDuperSaveFileForSettings.json");
    }

    private void Start()
    {
        // --- MAIN CANVAS ---
        _mainCanvasScaler = _mainCanvas.GetComponent<CanvasScaler>();
        _defaultUIResolution = _mainCanvasScaler.referenceResolution;

        // --- GAME OVER MENU ---
        _gameOverMenu.SetActive(false);
        SignalBus.Subscribe<GameOverSignal>(this);

        _nameInput.onValueChanged.AddListener(value => _highscoreName = value);

        // --- PAUSE MENU --
        _pauseMenu.SetActive(false);
        SignalBus.Subscribe<PauseSignal>(this);


        // --- HIGHSCORES ---
        _highscoreCanvas = _highscorePanel.GetComponent<CanvasGroup>();
        _highscoreCanvas.alpha = 0;
        _highscorePanel.SetActive(false);
        SignalBus.Subscribe<LevelUpdateSignal>(this);

        // --- SETTINGS ---
        _settingsCanvas = _settingsPanel.GetComponent<CanvasGroup>();
        _settingsCanvas.alpha = 0;
        _settingsPanel.SetActive(false);

        //_resolutions = Screen.resolutions.Distinct().Select(r => new TMP_Dropdown.OptionData(r.ToString().Split("@")[0])).ToList();
        _resolutions = Screen.resolutions.Distinct().Select(r => new TMP_Dropdown.OptionData(r.ToString())).ToList();
        // The update frequency does not change at the moment when selecting another option, because it did not work. But by leaving it in all options are unique.
        _resolutions = _resolutions.Distinct().ToList();
        _srSetting.options = _resolutions;
        _srSetting.onValueChanged.AddListener(ChangeScreenResolution);
        _srSetting.value = _srSetting.options.FindIndex(option => option.text == Screen.currentResolution.ToString().Split("@")[0]);

        _displayModes = FullScreenMode.GetNames(typeof(FullScreenMode)).ToList().Select(r => new TMP_Dropdown.OptionData(r.ToString())).ToList();
        _dmSetting.options = _displayModes;
        _dmSetting.onValueChanged.AddListener(ChangeDisplayMode);

        _usSetting.minValue = 0.1f;
        _usSetting.maxValue = 1;
        _usSetting.onValueChanged.AddListener(value => _usSettingText.text = value.ToString());
        _usSetting.onValueChanged.AddListener(SetUIScale);
        _usSetting.value = 1;


        _mvSetting.minValue = 0;
        _mvSetting.maxValue = 100;
        _mvSetting.wholeNumbers = true;
        _mvSetting.onValueChanged.AddListener(value => _mvSettingText.text = value.ToString());
        _mvSetting.onValueChanged.AddListener(SetBackgroundmusicVolume);
        _mvSetting.value = 50;

        _svSetting.minValue = 0;
        _svSetting.maxValue = 100;
        _svSetting.wholeNumbers = true;
        _svSetting.onValueChanged.AddListener(value => _svSettingText.text = value.ToString());
        _svSetting.onValueChanged.AddListener(SetSFXVolume);
        _svSetting.value = 50;

        LoadSettingsInformation();
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<GameOverSignal>(this);
        SignalBus.Unsubscribe<PauseSignal>(this);
        SignalBus.Unsubscribe<LevelUpdateSignal>(this);
    }

    private void OnApplicationQuit()
    {
        SignalBus.Unsubscribe<GameOverSignal>(this);
        SignalBus.Unsubscribe<PauseSignal>(this);
        SignalBus.Unsubscribe<LevelUpdateSignal>(this);

        SaveSettingsInformation();
    }

    #endregion

    /*--- BUTTONS -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    #region

    /* START */
    public void StartGame()
    {
        StartCoroutine(WaitForClickAtStart());
    }

    private IEnumerator WaitForClickAtStart()
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(0.3f);
        _mainMenu.SetActive(false);
        SignalBus.Fire(new NewGameSignal());
        SwitchBackgroundMusicTracks();
    }

    /* SETTINGS PANEL */
    public void AccessSettings()
    {
        _settingsPanel.SetActive(true);
        StartCoroutine(FadeIn(_settingsCanvas));
    }

    public void CloseSettings()
    {
        SaveSettingsInformation();
        StartCoroutine(FadeOut(_settingsCanvas, _settingsPanel));
    }

    /* FADE FUNCTIONS */
    private IEnumerator FadeIn(CanvasGroup canvas)
    {
        float t = 0;
        while (t < _fadeInTime)
        {
            t += Time.unscaledDeltaTime;
            canvas.alpha = t * 1 / _fadeInTime;
            yield return null;
        }
    }

    private IEnumerator FadeOut(CanvasGroup canvas, GameObject obj)
    {
        float t = 0;
        while (t < _fadeOutTime)
        {
            t += Time.unscaledDeltaTime;
            canvas.alpha = 1 - (t * 1 / _fadeOutTime);
            yield return null;
        }

        obj.SetActive(false);
    }


    /* QUIT */
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }


    /* PAUSE */
    public void ContinueAfterPause()
    {
        StartCoroutine(WaitForCklickAfterContinueAfterPause());
    }

    private IEnumerator WaitForCklickAfterContinueAfterPause()
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(0.3f);
        SignalBus.Fire(new PauseSignal { IsPaused = false });
    }

    /* BACK TO MAIN MENU */
    public void GoBackToMain()
    {
        _pauseMenu.SetActive(false);
        _gameOverMenu.SetActive(false);
        // Showing or not showing continue button depending on whether a game to continue exists
        _continueBtn.SetActive(File.Exists(_gameManager.SAVE_PATH_GAME_INFORMATION));
        _mainMenu.SetActive(true);
        StartCoroutine(WaitForCklickAfterGoBackToMain());
    }

    private IEnumerator WaitForCklickAfterGoBackToMain()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.3f);
        SignalBus.Fire(new BackToMainMenuSignal());
        // Showing or not showing continue button depending on whether a game to continue exists
        _continueBtn.SetActive(File.Exists(_gameManager.SAVE_PATH_GAME_INFORMATION));
    }

    /* CONTINUE FROM MAIN MENU */
    public void ContinueGameFromMain()
    {
        StartCoroutine(WaitForCklickAfterContinueGameFromMain());
    }

    private IEnumerator WaitForCklickAfterContinueGameFromMain()
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(0.3f);
        _mainMenu.SetActive(false);
        SignalBus.Fire(new ContinueFromSaveFileSignal());
        SwitchBackgroundMusicTracks();
    }

    /* HIGHSCORE */
    public void SubmitHighscore()
    {
        // Save highscore
        SignalBus.Fire(new SaveHighscoreSignal { PlayerName = _highscoreName });
        // Open highscore page
        AccessHighscore();
    }

    public void AccessHighscore()
    {
        // Depopulating highcore view from before -> otherwise just inserting a new element is hard
        foreach (Transform child in _highscoreEntryContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }


        // Populating highscore view
        var highscores = _gameManager.HighscoreData.Highscores.OrderByDescending(i => i.Score).ToList();
        if (highscores.Count == 0)
        {
            var entry = Instantiate(_highscoreEntraDarkPrefab);
            entry.transform.SetParent(_highscoreEntryContainer.transform);

            foreach (var text in entry.GetComponentsInChildren<TMP_Text>())
            {
                if (text.gameObject.name.Equals("Highscore Entry Name"))
                {
                    text.text = "No highscores yet!";
                }
                else
                {
                    text.text = "";
                }
            };
        }
        else
        {
            for (int i = 0; i < highscores.Count; i++)
            {
                GameObject entry;
                if (i % 2 == 0)
                {
                    entry = Instantiate(_highscoreEntraDarkPrefab);
                }
                else
                {
                    entry = Instantiate(_highscoreEntraLightPrefab);
                }
                entry.transform.SetParent(_highscoreEntryContainer.transform);
                foreach (var text in entry.GetComponentsInChildren<TMP_Text>())
                {
                    if (text.gameObject.name.Equals("Highscore Entry Name"))
                    {
                        text.text = highscores[i].Name;
                    }
                    else
                    {
                        text.text = highscores[i].Score.ToString();
                    }
                };

            }
        }

        // Making menu visible
        _highscorePanel.SetActive(true);
        StartCoroutine(FadeIn(_highscoreCanvas));
    }

    public void CloseHighscore()
    {
        // Making menu invisible
        StartCoroutine(FadeOut(_highscoreCanvas, _highscorePanel));
    }

    #endregion Buttons

    /*--- Specific settings -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    #region
    private void ChangeScreenResolution(int idx)
    {
        var x = _resolutions[idx].text.Split(" ")[0];
        var y = _resolutions[idx].text.Split(" ")[2];
        Screen.SetResolution(Int32.Parse(x), Int32.Parse(y), FullScreenMode.ExclusiveFullScreen);
    }
    private void ChangeDisplayMode(int idx)
    {
        var d = _displayModes[idx].text;
        if (d.Equals("FullScreenWindow"))
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else if (d.Equals("ExclusiveFullScreen"))
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else if (d.Equals("MaximizedWindow"))
        {
            Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
        }
        else if (d.Equals("Windowed"))
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }

    private void SetBackgroundmusicVolume(float value)
    {
        var val = value == 0 ? 0.001f : value / 100;
        if (_menuPlaying)
        {
            _audioMixer.SetFloat(MIXER_MENU_BACKGROUND_MUSIC, Mathf.Log10(val) * 20);
        }
        else
        {
            _audioMixer.SetFloat(MIXER_GAME_BACKGROUND_MUSIC, Mathf.Log10(val) * 20);
        }
    }

    private void SwitchBackgroundMusicTracks()
    {
        if (_menuPlaying)
        {
            float currentBackgroundVolume;
            _audioMixer.GetFloat(MIXER_MENU_BACKGROUND_MUSIC, out currentBackgroundVolume);
            StartCoroutine(FadeMixerGroup.StartFade(_audioMixer, MIXER_MENU_BACKGROUND_MUSIC, 1f, -80));
            StartCoroutine(FadeMixerGroup.StartFade(_audioMixer, MIXER_GAME_BACKGROUND_MUSIC, 1f, currentBackgroundVolume));
        }
        else
        {
            float currentBackgroundVolume;
            _audioMixer.GetFloat(MIXER_GAME_BACKGROUND_MUSIC, out currentBackgroundVolume);
            StartCoroutine(FadeMixerGroup.StartFade(_audioMixer, MIXER_GAME_BACKGROUND_MUSIC, 1f, -80));
            StartCoroutine(FadeMixerGroup.StartFade(_audioMixer, MIXER_MENU_BACKGROUND_MUSIC, 1f, currentBackgroundVolume));
        }
        _menuPlaying = !_menuPlaying;
    }

    private void SetSFXVolume(float value)
    {
        var val = value == 0 ? 0.001f : value / 100;
        _audioMixer.SetFloat(MIXER_SFX, Mathf.Log10(val) * 20);
    }

    private void SetUIScale(float scale)
    {
        _mainCanvasScaler.referenceResolution = new Vector2(_defaultUIResolution.x / scale, _defaultUIResolution.y / scale);
    }

    #endregion

    /*--- Make stuff better looking -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    #region 
    /* This needs to be two functions, cause Unity says so */
    public void ChangeButtonBackgroundOpacityToOne(int idx)
    {
        var image = _images[idx];
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
    }

    public void ChangeButtonBackgroundOpacityToZero(int idx)
    {
        var image = _images[idx];
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
    }
    #endregion

    /*--- SIGNAL RESPONSES -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    #region
    public void OnEventHappen(GameOverSignal e)
    {
        Time.timeScale = 0;
        _mainMenu.SetActive(false);
        _pauseMenu.SetActive(false);
        _finalScoreText.text = "Your score: " + _score.ToString();
        _gameOverMenu.SetActive(true);
        SwitchBackgroundMusicTracks();
    }

    public void OnEventHappen(PauseSignal e)
    {
        Time.timeScale = e.IsPaused ? 0f : 1f;
        _pauseMenu.SetActive(e.IsPaused);
        SwitchBackgroundMusicTracks();
    }

    public void OnEventHappen(LevelUpdateSignal e)
    {
        _score = e.Level;
    }
    #endregion

    /*--- SAVING FOR SETTINGS -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    #region

    /*
     * Saves Settings in file.
     */
    private void SaveSettingsInformation()
    {
        SerializedSettings serializedSettings = new SerializedSettings(_srSetting.value, _dmSetting.value,_usSetting.value, _mvSetting.value, _svSetting.value);
        serializedSettings.Serialize();
        // Overwritting old file
        File.WriteAllText(SAVE_PATH_SETTINGS, JsonUtility.ToJson(serializedSettings));
    }

    /*
    * Loads settings from settings information save file.
    */
    private void LoadSettingsInformation()
    {
        if (File.Exists(SAVE_PATH_SETTINGS))
        {
            var json = File.ReadAllText(SAVE_PATH_SETTINGS);
            var serializedSettings = new SerializedSettings();
            JsonUtility.FromJsonOverwrite(json, serializedSettings);

            // Sreen resoultion
            #region
            var x = _srSetting.options[serializedSettings.ScreenResolution].text.Split(" ")[0];
            var y = _srSetting.options[serializedSettings.ScreenResolution].text.Split(" ")[2];
            Screen.SetResolution(Int32.Parse(x), Int32.Parse(y), FullScreenMode.ExclusiveFullScreen);
            _srSetting.value = serializedSettings.ScreenResolution;
            #endregion

            // Fullscreen mode
            #region
            var d = _dmSetting.options[serializedSettings.FullscreenMode].text;
            if (d.Equals("FullScreenWindow"))
            {
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            }
            else if (d.Equals("ExclusiveFullScreen"))
            {
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            }
            else if (d.Equals("MaximizedWindow"))
            {
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
            }
            else if (d.Equals("Windowed"))
            {
                Screen.fullScreenMode = FullScreenMode.Windowed;
            }
            _dmSetting.value = serializedSettings.FullscreenMode;
            #endregion

            // UI Scale
            SetUIScale(serializedSettings.UIScale);
            _usSetting.value = serializedSettings.UIScale;

            // Backgroundmusic volume
            SetBackgroundmusicVolume(serializedSettings.BackgroundmusicVolume);
            _mvSetting.value = serializedSettings.BackgroundmusicVolume;

            // SFX Volume
            SetSFXVolume(serializedSettings.SFXVolume);
            _svSetting.value = serializedSettings.SFXVolume;
        }
    }

    #endregion
}
