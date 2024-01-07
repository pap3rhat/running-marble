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

    // Time
    private float _startTime;
    private float _timerLength = 600f;
    [HideInInspector] public UnityEvent<float> TimeLeft = new();
    [HideInInspector] public UnityEvent<bool> TimerDisplayed = new();

    // Lifes
    private int _startingLifes = 3;
    private int _remainingLifes;

    // Level Progression
    private int _currentLevel = 1;
    [HideInInspector] public UnityEvent<int> LevelUpdate = new();
    private int _currentObjectAmount = 5;
    private PopulateModule _popMod;

    // Respawn
    private bool _playerAlive = false;
    [HideInInspector] public UnityEvent<int, int> PlayerDied = new();
    [HideInInspector] public UnityEvent RespawnCountdown = new();
    [HideInInspector] public float RespawnMessageTime; // used to control how long respawning message is shown; gets set be RespawingDisplay UI class, because that one control coroutine

    // Start 
    [HideInInspector] public UnityEvent StartCountdown = new();
    // Time out -> Game Over
    [HideInInspector] public UnityEvent GameOver = new();
    public bool DeathHappened = false; // gets set by killtrigger
    // Pause
    private bool _isPaused = false;
    private float _pauseStartTime = 0;
    [HideInInspector] public UnityEvent<bool> Paused = new();

    // Instance
    private static GameManager _instance;
    public static GameManager Instance { get => _instance; }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        // Singelton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        // Getting inputmanager, so controls can be disabled or enabled by gamemanager
        _inputManager = InputManager.Instance;

        // Setting up first level up
        _popMod = GameObject.Find("Base Module").GetComponentInChildren<PopulateModule>();
        _popMod.PopulateWithPrefab(_currentObjectAmount);

        // Activating fog, to enhance illusion of endlessness
        RenderSettings.fog = true;
    }

    void Start()
    {
        // Lifes
        _remainingLifes = _startingLifes;
    }

    void Update()
    {

        // If player is not alive, nothing happens in here -> either animation is playing or player is in some menu
        if (!_playerAlive)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }

        // Only check for stuff if the player is not in a menu
        if (!_isPaused)
        {
            CheckDeath();
            CheckGameOver();
            CountdownUpdate();
        }
    }

    /*--- METHODS OTHER CLASSES CALL -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /* Used to tell game manager that start button got pressed. */
    public void StartGame()
    {
        // Player
        SpawnPlayer(false);
    }

    /* Used to tell GameManger that game truely started (spawning animation is done) and they can start everything */
    public void SpawnAnimationPlayed()
    {
        // player can move and is alive
        _playerAlive = true;
        _inputManager.TriggerEnable();

        // Countdown
        TimerDisplayed.Invoke(true);
        _startTime = (float)Math.Round(Time.time, 2);
    }

    /* Used to tell GameManager that the COntinue button in the pause menu got pressed */
    public void ContinueGame()
    {
        Pause();
    }

    /* 
     * Used to tell GameManger that Checkpoint got reached.
     * Teleports player back to beginning.
     * Repopulates level.
     */
    public void CheckpointReached()
    {
        // Updating Level display
        _currentLevel++;
        LevelUpdate.Invoke(_currentLevel);

        // Ereasing old level
        _popMod.DepopulatePrefabs();
        // Creating new level
        _currentObjectAmount += 5;
        _popMod.PopulateWithPrefab(_currentObjectAmount);

        // Setting player back, but keeping x and y coordinate, so it is not as obvious
        Vector3 playerPosition = _currentPlayerObject.transform.position;
        _currentPlayerObject.transform.position = new Vector3(playerPosition.x, playerPosition.y, _playerSpawnPosition.z);
    }

    /*--- GAME LOOP -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /*
     * Pauses game. Continues game.
     */
    private void Pause()
    {
        _isPaused = !_isPaused;
        Paused.Invoke(_isPaused);
        //if (_isPaused)
        //{
        //    // Player cannot move anymore
        //    _inputManager.TriggerDisable();

        //    // Do not count pause time as time player has played
        //    _pauseStartTime = Time.time;
        //}
        //else
        //{
        //    // Player can move again
        //    _inputManager.TriggerEnable();
        //    // Correcting time
        //    _startTime -= (_pauseStartTime - Time.time);
        //}
    }

    /*
     * Updates time counter.
     */
    private void CountdownUpdate()
    {
        TimeLeft.Invoke((float)Math.Round(_timerLength - Time.time + _startTime, 2));
    }

    /*
     * Checks if game is over (0 lifes or time ran out)
     */
    private void CheckGameOver()
    {
        // Time ran out or no more lifes left
        if (Time.time - _startTime >= _timerLength || _remainingLifes == 0)
        {
            // just clear everything
            GameOver.Invoke();
            // Setting player to dead, and disallowing themto move
            _playerAlive = false;
            _inputManager.TriggerDisable();
        }
    }

    /*
     * Checks if player died this frame.
     */
    private void CheckDeath()
    {
        // Player died
        if (DeathHappened)
        {
            StartCoroutine(Respawn());
            DeathHappened = false;
        }
    }

    /*
     * Restarts the game if enough lifes are left.
     * TODO: REMOVE THIS UGLY SPHAGET (or don't, because there is no non-ugly way, mabye, probably, who even knows)
     */
    private IEnumerator Respawn()
    {
        // Do not count respawning time as time player has played
        float respawnStartTime = Time.time;

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

            // Wait for respawn message to have been fully displayed
            yield return new WaitForSeconds(RespawnMessageTime);

            // Setting player to be alive again
            _playerAlive = true;

            // Setting time back and displaying it again
            TimerDisplayed.Invoke(true);
            // Correcting time
            _startTime -= (respawnStartTime - Time.time);

            // player can move again
            _inputManager.TriggerEnable();
        }
    }

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