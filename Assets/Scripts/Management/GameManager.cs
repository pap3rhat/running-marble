using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public class GameManager : MonoBehaviour
{
    // Camera
    [SerializeField] private CinemachineStateDrivenCamera _stateCamera;
    [SerializeField] private Animator _animator;

    // Player information
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Vector3 _playerSpawnPosition;
    [SerializeField] private float _playerYValueCondition;

    private GameObject _currentPlayerObject;
    private PlayerMovement _currentPlayerMovementScript;

    // Countdown
    private float _startTime;
    private float _timerLength = 500f;

    public UnityEvent<float> TimeLeft = new();
    public UnityEvent<bool> TimeOut = new();

    // Lifes
    private int _startingLifes = 3;
    private int _remainingLifes;

    // Respawn
    private bool _playerAlive = true;
    public UnityEvent<int, int> PlayerDied = new();
    public UnityEvent RespawnCountdown = new();

    // Fall death
    public UnityEvent<bool> FallDeath = new();

    // End state
    public UnityEvent<bool> EndState = new();

    // Goal destination
    [SerializeField] private GameObject _goal;
    private Vector3 _goalPosition;
    private bool _playerWon = false;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    void Start()
    {
        // Player
        SpawnPlayer();
        // Countdown
        _startTime = (float)Math.Round(Time.time, 2);
        // Lifes
        _remainingLifes = _startingLifes;
        // Goal
        _goalPosition = _goal.transform.position;
    }

    void Update()
    {
        if (!_playerAlive || _playerWon)
        {
            return;
        }

        CheckWin();
        CheckDeath();
       // CheckCameraSwitch();
        CountdownUpdate();
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /*
     * Updates time counter.
     */
    private void CountdownUpdate()
    {
        TimeLeft.Invoke((float)Math.Round(_timerLength - Time.time + _startTime, 2));
    }

    /*
     * Checks if player died this frame.
     */
    private void CheckDeath()
    {
        // Time ran out
        if (Time.time - _startTime >= _timerLength)
        {
            TimeOut.Invoke(true);
            StartCoroutine(Restart());
        }
        // Player fell down
        else if (_currentPlayerObject.transform.position.y < _playerYValueCondition)
        {
            FallDeath.Invoke(true);
            StartCoroutine(Restart());
        }
    }

    /*
     * Restarts the game if enough lifes are left.
     * TODO: REMOVE THIS UGLY SPHAGET
     */
    private IEnumerator Restart()
    {
        // Setting player to dead
        _playerAlive = false;
        PlayerDied.Invoke(_startingLifes, _remainingLifes);
        _remainingLifes--;
        DestroyPlayer();

        // Use next life if possible
        if (_remainingLifes > 0)
        {
            // Respawning player
            SpawnPlayer();
           // _currentPlayerMovementScript.enabled = false;
            RespawnCountdown.Invoke();

            yield return new WaitForSeconds(1);
            FallDeath.Invoke(false);
            TimeOut.Invoke(false);
            yield return new WaitForSeconds(4);

           // _currentPlayerMovementScript.enabled = true;
            _playerAlive = true;

            // Setting time back
            _startTime = (float)Math.Round(Time.time, 2);
        }

        // End game if dead
        if (_remainingLifes == 0)
        {
            FallDeath.Invoke(false);
            TimeOut.Invoke(false);
            EndState.Invoke(false);
        }
    }

    /*
     * Checks if player won by meassuring player distance to goal.
     */
    private void CheckWin()
    {
        if (Vector3.Distance(_currentPlayerObject.transform.position, _goalPosition) < 0.5)
        {
            //_currentPlayerMovementScript.enabled = false;
            _playerWon = true;
            EndState.Invoke(true);
        }
    }


    /* 
     * Handles spawning of player.
     */
    private void SpawnPlayer()
    {
        _currentPlayerObject = Instantiate(_playerPrefab, _playerSpawnPosition, _playerPrefab.transform.rotation);
        
        // Camera set up
        _stateCamera.Follow = _currentPlayerObject.transform;
        _stateCamera.LookAt = _currentPlayerObject.transform;
        _currentPlayerObject.GetComponent<CameraTrigger>().StateCamera = _stateCamera;
        _currentPlayerObject.GetComponent<CameraTrigger>().Animator = _animator;

       // _currentPlayerMovementScript = _currentPlayerObject.GetComponent<PlayerMovement>();
    }

    /*
     * Handles destroying of player
     */
    private void DestroyPlayer()
    {
        Destroy(_currentPlayerObject);
        //Destroy(_currentPlayerMovementScript);
    }
}