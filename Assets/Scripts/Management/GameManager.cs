using System;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // Countdown
    private float _startTime;
    private float _timerLength = 0.5f;
    public UnityEvent<float> TimeLeft = new();
    public UnityEvent TimeOut = new();

    // Lifes
    private int _startingLifes = 3;
    private int _remainingLifes;
    public UnityEvent<int,int> PlayerDied = new();


    bool goind = true;


    // Start is called before the first frame update
    void Start()
    {
        // Countdown
        _startTime = (float)Math.Round(Time.time, 2);

        // Lifes
        _remainingLifes = _startingLifes;
    }

    // Update is called once per frame
    void Update()
    {
        CountdownUpdate();
    }

    private void CountdownUpdate()
    {
        if (Time.time - _startTime >= _timerLength && goind)
        {
            PlayerDied.Invoke(_startingLifes, _remainingLifes);
            _remainingLifes--;
            TimeOut.Invoke();
            goind = false; 
        }
        else
        {
            TimeLeft.Invoke((float)Math.Round(_timerLength - Time.time, 2));
        }
    }
}
