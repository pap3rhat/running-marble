using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
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

    [SerializeField] private GameObject _credits;
    private CanvasGroup _creditsCanvas;

    private float _fadeInTime = 0.7f;
    private float _fadeOutTime = 0.7f;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Start()
    {
        _settingsCanvas = _settings.GetComponent<CanvasGroup>();
        _settingsCanvas.alpha = 0;
        _settings.SetActive(false);
        _creditsCanvas = _credits.GetComponent<CanvasGroup>();
        _creditsCanvas.alpha = 0;
        _credits.SetActive(false);

        _srSetting.options = Screen.resolutions.Select(r => new TMP_Dropdown.OptionData(r.ToString())).ToList();

        _dmSetting.options = FullScreenMode.GetNames(typeof(FullScreenMode)).ToList().Select(r => new TMP_Dropdown.OptionData(r.ToString())).ToList();

        _usSetting.minValue = 0.25f;
        _usSetting.maxValue = 5;
        _usSetting.onValueChanged.AddListener(value => _usSettingText.text = value.ToString());
        _usSetting.value = 1;

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
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
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

    /* CREDITS PANEL */
    public void AccessCredits()
    {
        _credits.SetActive(true);
        StartCoroutine(FadeIn(_creditsCanvas));
    }

    public void CloseCredits()
    {
        StartCoroutine(FadeOut(_creditsCanvas, _credits));
    }

    /* FADE FUNCTIONS */
    private IEnumerator FadeIn(CanvasGroup canvas)
    {
        float t = 0;
        while (t < _fadeInTime)
        {
            t += Time.deltaTime;
            canvas.alpha = t * 1/ _fadeInTime;
            yield return null;
        }
    }

    private IEnumerator FadeOut(CanvasGroup canvas, GameObject obj)
    {
        float t = 0;
        while (t < _fadeOutTime)
        {
            t += Time.deltaTime;
            canvas.alpha = 1 - (t * 1/ _fadeOutTime);
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
}
