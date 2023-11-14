using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Countdown
    [SerializeField] private TextMeshProUGUI _countdownText;
    private float _startTime;
    private float _timerLength = 60f;


    // Start is called before the first frame update
    void Start()
    {
        // Countdown
        _startTime = (float)Math.Round(Time.time, 2);
    }

    // Update is called once per frame
    void Update()
    {
        CountdownUpdate();
    }

    private void CountdownUpdate()
    {
        if(Time.time - _startTime >= _timerLength)
        {
            _countdownText.text = $"Time Out!";
        }
        else
        {
            _countdownText.text = $"Time: {Time.time:F2}";
        }
    }
}
