using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public class GameManager : MonoBehaviour
{
    // Input Manager
    private InputManager _inputManager;

    // Camera
    [SerializeField] private CinemachineStateDrivenCamera _stateCamera;
    [SerializeField] private Animator _animator;

    // Player information
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Vector3 _playerSpawnPosition;

    private GameObject _currentPlayerObject;

    // Time Countdown
    private float _startTime;
    private float _timerLength = 6000f;
    [HideInInspector] public UnityEvent<float> TimeLeft = new();
    [HideInInspector] public UnityEvent<bool> TimerDisplayed = new();

    // Lifes
    private int _startingLifes = 3;
    private int _remainingLifes;

    // Respawn
    private bool _playerAlive = false;
    [HideInInspector] public UnityEvent<int, int> PlayerDied = new();
    [HideInInspector] public UnityEvent RespawnCountdown = new();
    [HideInInspector] public float RespawnMessageTime; // used to control how long respawning message is shown; gets set be RespawingDisplay UI class, because that one control coroutine
    [HideInInspector] public float DiedMessageTime; // used to control how long died message is shown

    // Start 
    [HideInInspector] public UnityEvent StartCountdown = new();
    // Time out death
    [HideInInspector] public UnityEvent<bool> TimeOut = new();
    // Other kind of death
    [HideInInspector] public UnityEvent<bool> MiscDeath = new();
    public bool MiscDeathHappened = false;
    // End state
    [HideInInspector] public UnityEvent<bool> EndState = new();

    // Goal destination -- TODO, switch to goal box trigger
    //[SerializeField] private GameObject _goal;
    //private Vector3 _goalPosition;
    //private bool _playerWon = false;

    // Test obstacle switch
    // TODO: List of all differnet prefabs -> spawn random one
    // Number of how many after another, then goal one -> own prefab
    // Get active one from hierarchy at beginning (search for module active or start or something)
    [SerializeField] private GameObject _modulePrefab;
    [SerializeField] private GameObject _moduleActive;



    // Instance
    private static GameManager _instance;
    public static GameManager Instance { get => _instance; }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        DiedMessageTime = 1f; // setting here, so it does not have to be set in inspector

        RenderSettings.fog = true;

        _inputManager = InputManager.Instance;
    }

    void Start()
    {
        // Player
        SpawnPlayer(false);
        // Lifes
        _remainingLifes = _startingLifes;
        // Goal
        //_goalPosition = _goal.transform.position;
    }

    void Update()
    {
        if (!_playerAlive /*|| _playerWon*/)
        {
            return;
        }

        //CheckWin();
        CheckDeath();
        // CheckCameraSwitch();
        CountdownUpdate();
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /* Used to tell GameManger that game truely started (spawning animation is done) and they can start everything */
    public void GameStarted()
    {
        _playerAlive = true;

        // player can move
        _inputManager.TriggerEnable();

        // Countdown
        TimerDisplayed.Invoke(true);
        _startTime = (float)Math.Round(Time.time, 2);
    }


    /* 
     * Used to tell GameManger that Checkpoint got reached.
     * Teleports player back to beginning.
     */
    public void CheckpointReached()
    {
        // Setting player back, but keeping x and y coordinate, so it is not as obvious
        Vector3 playerPosition = _currentPlayerObject.transform.position;
        _currentPlayerObject.transform.position = new Vector3(playerPosition.x, playerPosition.y, _playerSpawnPosition.z);

        // Setting new element of track
        Vector3 spawnPos = _moduleActive.transform.position;
        Quaternion spawnRot = _moduleActive.transform.rotation;
        Destroy(_moduleActive);
        _moduleActive = Instantiate(_modulePrefab, spawnPos, spawnRot);
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
            StartCoroutine(Respawn(true));
        }
        // Player died in another way
        else if (MiscDeathHappened)
        {
            MiscDeath.Invoke(true);
            StartCoroutine(Respawn(false));
            MiscDeathHappened = false;
        }
    }

    /*
     * Restarts the game if enough lifes are left.
     * TODO: REMOVE THIS UGLY SPHAGET (or don't, because there is no non-ugly way, mabye, probably, who even knows)
     */
    private IEnumerator Respawn(bool timeDeath)
    {
        // Setting player to dead
        _playerAlive = false;
        PlayerDied.Invoke(_startingLifes, _remainingLifes);
        _remainingLifes--;
        DestroyPlayer();

        // Player cannot move anymore
        _inputManager.TriggerDisable();

        // Use next life if possible
        if (_remainingLifes > 0)
        {
            // Removing timer while respawning animation is playing
            TimerDisplayed.Invoke(false);

            // Respawning player
            SpawnPlayer(true);

            yield return new WaitForSeconds(DiedMessageTime);

            if (timeDeath) // player died because time ran out
            {
                TimeOut.Invoke(false);
            }
            else // player died because of other things
            {
                MiscDeath.Invoke(false);
            }

            yield return new WaitForSeconds(RespawnMessageTime);

            // Setting player to be alive again
            _playerAlive = true;

            // Setting time back and displaying it again
            TimerDisplayed.Invoke(true);
            _startTime = (float)Math.Round(Time.time, 2);

            // player can move again
            _inputManager.TriggerEnable();
        }

        // End game if dead
        if (_remainingLifes == 0)
        {
            // just clear everything
            MiscDeath.Invoke(false);
            TimeOut.Invoke(false);
            EndState.Invoke(false);
        }
    }

    /*
     * Checks if player won by meassuring player distance to goal. TODO: exhcange with win trigger box
     */
    //private void CheckWin()
    //{
    //    if (Vector3.Distance(_currentPlayerObject.transform.position, _goalPosition) < 0.5)
    //    {
    //        _playerWon = true;
    //        EndState.Invoke(true);
    //    }
    //}


    /* 
     * Handles spawning of player.
     */
    private void SpawnPlayer(bool respawn)
    {
        // Player object
        _currentPlayerObject = Instantiate(_playerPrefab, _playerSpawnPosition, _playerPrefab.transform.rotation);
        
        // Player cannot move at beginning
        _inputManager.TriggerDisable();

        // Camera set up
        _stateCamera.Follow = _currentPlayerObject.transform;
        _stateCamera.LookAt = _currentPlayerObject.transform;
        _currentPlayerObject.GetComponent<CameraTrigger>().StateCamera = _stateCamera;
        _currentPlayerObject.GetComponent<CameraTrigger>().Animator = _animator;

        if (respawn) // Countdown to respawn
        {
            RespawnCountdown.Invoke();
        }
        else // Countdown to start
        {
            StartCountdown.Invoke();
        }
    }

    /*
     * Handles destroying of player
     */
    private void DestroyPlayer()
    {
        Destroy(_currentPlayerObject);
    }
}