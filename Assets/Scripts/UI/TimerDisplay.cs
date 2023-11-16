using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerDisplay : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;

    //Count down
    [SerializeField] private TextMeshProUGUI _countdownText;
    // Time out
    [SerializeField] private GameObject _timeOutObj;

    void Start()
    {
        _timeOutObj.SetActive(false);
        _gameManager.TimeOut.AddListener(OnTimeOut);
        _gameManager.TimeLeft.AddListener(OnTimeLeft);
    }

    /*
     * Handles display when time runs out.
     */
    private void OnTimeOut()
    {
        _timeOutObj.SetActive(true);
    }



    /*
     *  Handles displaying remaining time.
     */
    private void OnTimeLeft(float time)
    {
        _countdownText.text = $"Time: {time}";
    }
}
