using TMPro;
using UnityEngine;

public class TimerDisplay : MonoBehaviour
{
    private GameManager _gameManager;

    // Time Count down
    [SerializeField] private GameObject _countdownObject;
    [SerializeField] private TextMeshProUGUI _countdownText;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    private void Awake()
    {
        _gameManager = GameManager.Instance;
        _gameManager.TimeLeft.AddListener(OnTimeLeft);
        _gameManager.TimerDisplayed.AddListener(display => _countdownObject.SetActive(display));

        _countdownObject.SetActive(false);
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /*
     *  Handles displaying remaining time.
     */
    private void OnTimeLeft(float time)
    {
        _countdownText.text = $"Time: {time}";
    }
}
