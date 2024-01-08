using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Camera -> used to position so it looks nicer
    //[SerializeField] private GameObject _mainCamera;
    private Vector3 _mainCameraPos = new Vector3(-1.7f, 7.85f, -150);
    private Vector3 _mainCameraRot = new Vector3(45f, 0, 0);

    // Main Menu
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _continueBtn;

    // Pause Menu
    [SerializeField] private GameObject _pauseMenu;

    // Game Over Menu
    [SerializeField] private GameObject _gameOverMenu;

    // Settings UI Elements
    [SerializeField] private GameObject _settings;
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

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        _gameManager = GameManager.Instance;
        // Showing or not showing continue button depending on whether a game to continue exists
        _continueBtn.SetActive(File.Exists(_gameManager.SAVE_PATH_GAME_INFORMATION));
    }

    private void Start()
    {
        // --- GAME OVER MENU ---
        _gameOverMenu.SetActive(false);

        // --- PAUSE MENU --
        _pauseMenu.SetActive(false);
        _gameManager.Paused.AddListener(OnPaused);

        // --- SETTINGS ---
        _settingsCanvas = _settings.GetComponent<CanvasGroup>();
        _settingsCanvas.alpha = 0;
        _settings.SetActive(false);

        _resolutions = Screen.resolutions.Select(r => new TMP_Dropdown.OptionData(r.ToString().Split("@")[0])).ToList();
        // TODO: figure out why that is not working
        _resolutions = _resolutions.Distinct().ToList();
        _srSetting.options = _resolutions;
        _srSetting.onValueChanged.AddListener(ChangeScreenResolution);

        _displayModes = FullScreenMode.GetNames(typeof(FullScreenMode)).ToList().Select(r => new TMP_Dropdown.OptionData(r.ToString())).ToList();
        _dmSetting.options = _displayModes;
        _dmSetting.onValueChanged.AddListener(ChangeDisplayMode);


        // TODO: figure out how to make UI differently sized
        _usSetting.minValue = 0.25f;
        _usSetting.maxValue = 5;
        _usSetting.onValueChanged.AddListener(value => _usSettingText.text = value.ToString());
        _usSetting.value = 1;


        // TODO: do this once sound and music is implemented
        _mvSetting.minValue = 0;
        _mvSetting.maxValue = 100;
        _mvSetting.wholeNumbers = true;
        _mvSetting.onValueChanged.AddListener(value => _mvSettingText.text = value.ToString());
        _mvSetting.value = 50;


        _svSetting.minValue = 0;
        _svSetting.maxValue = 100;
        _svSetting.wholeNumbers = true;
        _svSetting.onValueChanged.AddListener(value => _svSettingText.text = value.ToString());
        _svSetting.value = 50;
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

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
        _gameManager.StartNewGame();
    }

    /* SETTINGS PANEL */
    public void AccessSettings()
    {
        _settings.SetActive(true);
        StartCoroutine(FadeIn(_settingsCanvas));
    }

    public void CloseSettings()
    {
        StartCoroutine(FadeOut(_settingsCanvas, _settings));
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
    private void OnPaused(bool pause)
    {
        Time.timeScale = pause ? 0f : 1f;
        _pauseMenu.SetActive(pause);
    }

    public void ContinueAfterPause()
    {
        StartCoroutine(WaitForCklickAfterContinueAfterPause());
    }

    private IEnumerator WaitForCklickAfterContinueAfterPause()
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(0.3f);
        _gameManager.ContinueGameFromPauseMenu();
    }

    /* BACK TO MAIN MENU */
    public void GoBackToMain()
    {
        _pauseMenu.SetActive(false);
        _gameOverMenu.SetActive(false);
        _mainMenu.SetActive(true);
        StartCoroutine(WaitForCklickAfterGoBackToMain());
    }

    private IEnumerator WaitForCklickAfterGoBackToMain()
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(0.3f);
        Time.timeScale = 0f;
        //PositionCamera();
        _gameManager.GoBackToMainMenu();
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
        _gameManager.ContinueGameFromSaveFile();
    }


    /*--- Specific settings -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

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

    /*--- Make stuff better looking -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

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

    // TODO: fgure out why cinemachine does not allow this and camera is not getting repositioned.
    /* Positioning camera. */
    //private void PositionCamera()
    //{
    //    _mainCamera.transform.position = _mainCameraPos;
    //    _mainCamera.transform.rotation = Quaternion.Euler(_mainCameraRot);
    //}

}
