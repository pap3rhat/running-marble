using TMPro;
using UnityEngine;

public class TimerDisplay : MonoBehaviour
{
    private GameManager _gameManager;

    // Count down
    [SerializeField] private TextMeshProUGUI _countdownText;
    // Time out
    [SerializeField] private GameObject _timeOutObj;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    private void Awake()
    {
        _gameManager = GameManager.Instance;
    }

    void Start()
    {
        _gameManager.TimeLeft.AddListener(OnTimeLeft);
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
