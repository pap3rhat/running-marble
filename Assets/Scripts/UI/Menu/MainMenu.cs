using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Main Menu
    [SerializeField] private GameObject _mainMenu;

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

    // List of all images used for main menu buttons -> cannot be accessed via EventTrigger, because Unity does not allow for that..
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
    }

    private void Start()
    {
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
        yield return new WaitForSeconds(0.3f);
        _mainMenu.SetActive(false);
        _gameManager.StartGame();
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
            t += Time.deltaTime;
            canvas.alpha = t * 1 / _fadeInTime;
            yield return null;
        }
    }

    private IEnumerator FadeOut(CanvasGroup canvas, GameObject obj)
    {
        float t = 0;
        while (t < _fadeOutTime)
        {
            t += Time.deltaTime;
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


    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    // Specific settings

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

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

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
}
