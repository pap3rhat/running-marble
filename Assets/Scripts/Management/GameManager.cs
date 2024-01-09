using Cinemachine;
using System;
using System.Collections;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
public class GameManager : MonoBehaviour
{
    // Saving system
    public string SAVE_PATH_GAME_INFORMATION;
    public string SAVE_PATH_HIGHSCORES;
    public SerializedHighscores HighscoreData;

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
    private static float TIMER_LENGTH = 600f;
    [HideInInspector] public UnityEvent<float> TimeLeft = new();
    [HideInInspector] public UnityEvent<bool> TimerDisplayed = new();

    // Lifes
    private static int STARTING_LIFES = 3;
    private int _remainingLifes;
    [HideInInspector] public UnityEvent ResetLifeDisplay = new();

    // Level Progression
    private int _currentLevel = 1;
    [HideInInspector] public UnityEvent<int> LevelUpdate = new();
    private int _currentObjectAmount = 5;
    private static int OBJECT_AMOUNT_PROGRESSION = 5;
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
    [HideInInspector] public UnityEvent<bool> Paused = new();

    // Went back to main menu
    [HideInInspector] public UnityEvent BackToMain = new();

    // Instance
    private static GameManager _instance;
    public static GameManager Instance { get => _instance; }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        // Save paths
        SAVE_PATH_GAME_INFORMATION = Path.Combine(Application.persistentDataPath, "superDuperSaveFileForGameInformation.json");
        SAVE_PATH_HIGHSCORES = Path.Combine(Application.persistentDataPath, "superDuperSaveFileForHighscores.json");

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

        // Activating fog, to enhance illusion of endlessness
        RenderSettings.fog = true;

        // Loading Highscores
        LoadHighscoreInformation();
    }

    void Start()
    {
        // Lifes
        _remainingLifes = STARTING_LIFES;
    }

    void Update()
    {
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

    private void OnApplicationQuit()
    {

        // TODO: check if this truely only saves when player is still playing
        // Saving game information if user closes application and is still in play
        if (_remainingLifes > 0 && Time.time - _startTime < TIMER_LENGTH)
        {
            SaveGameInformation();
        }
    }

    /*--- METHODS OTHER CLASSES CALL (this should not be done like this, but oh well, that is how it is now) -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    // I NEED A SIGNAL BUS ASAP, UI IS RUINING MY LIFE (AND CODE)
    // AND A SEPERATE CALSS TO SAVE STUFF; AHAHAHRHUJHHW

    /* Used to tell game manager that new game button got pressed. */
    public void StartNewGame()
    {
        // Setting everything to default -> need when starting new game while a also having already played a game in the same session
        _remainingLifes = STARTING_LIFES;
        ResetLifeDisplay.Invoke();
        _startTime = Time.time;
        _currentLevel = 1;
        LevelUpdate.Invoke(_currentLevel);
        _currentObjectAmount = OBJECT_AMOUNT_PROGRESSION;

        // Setting up first level up
        _popMod = GameObject.Find("Base Module").GetComponentInChildren<PopulateModule>();
        _popMod.PopulateWithPrefab(_currentObjectAmount);

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

    /* Used to tell GameManager that the Continue button in the pause menu got pressed */
    public void ContinueGameFromPauseMenu()
    {
        Pause();
    }

    /* Used to tell GameManager that player went back to main menu after already being in game.*/
    public void GoBackToMainMenu()
    {
        // Telling Game UI that main menu is there now and they have to go away
        BackToMain.Invoke();

        // Deleting scene
        _popMod.DepopulatePrefabs();
        // Destroying player
        DestroyPlayer();

        // Saving game information if user came out of unfinished game
        if (_remainingLifes > 0 && Time.time - _startTime < TIMER_LENGTH)
        {
            SaveGameInformation();
        }
    }

    /* Used to tell GameManager that player pressed continue in main menu. */
    public void ContinueGameFromSaveFile()
    {
        LoadGameInformation();
        _popMod.PopulateWithPrefab(_currentObjectAmount);
        SpawnPlayer(false);
        _isPaused = false;
    }


    /* Used to tell GameManager that highscore got submitted */
    public void SaveHighscore(string name)
    {
        SaveHighscoreInformation(name);
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
        _currentObjectAmount += OBJECT_AMOUNT_PROGRESSION;
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
    }

    /*
     * Updates time counter.
     */
    private void CountdownUpdate()
    {
        TimeLeft.Invoke((float)Math.Round(TIMER_LENGTH - Time.time + _startTime, 2));
    }

    /*
     * Checks if game is over (0 lifes or time ran out)
     */
    private void CheckGameOver()
    {
        // Time ran out or no more lifes left
        if (Time.time - _startTime >= TIMER_LENGTH || _remainingLifes == 0)
        {
            // just clear everything in UI that is not Game Over Menu
            GameOver.Invoke();
            // Setting player to dead, and disallowing themto move
            _playerAlive = false;
            _inputManager.TriggerDisable();

            // Deleting scene
            _popMod.DepopulatePrefabs();
            // Destroying player
            DestroyPlayer();
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
        PlayerDied.Invoke(STARTING_LIFES, _remainingLifes);
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
        // Player set to dead
        _playerAlive = false;


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

    /*--- SAVING SYSTEM -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /* 
     * Saves gameplay information into a file. 
     * The following things get saved:
     * Remaining lifes
     * Remaining time
     * Level Progression
     * The following things DO NOT get saved:
     * Exact postion and velocity of player within level.
     * Level outline.
     * When a player continues their game they will have as much time and as many lifes left as they stopped with. They will also be at the save level number, 
     * but have to start the level from the beginning and the level will very much likely look different. (It would be way to tedious to save and load that kind of 
     * information).
     */
    private void SaveGameInformation()
    {
        SerializedState serializedState = new SerializedState(_remainingLifes, TIMER_LENGTH - (Time.time - _startTime), _currentLevel);
        serializedState.Serialize();
        // Overwritting old file
        File.WriteAllText(SAVE_PATH_GAME_INFORMATION, JsonUtility.ToJson(serializedState));
    }

    /*
     * Loads information from game information save file.
     */
    private void LoadGameInformation()
    {
        var json = File.ReadAllText(SAVE_PATH_GAME_INFORMATION);
        var serializedState = new SerializedState();
        JsonUtility.FromJsonOverwrite(json, serializedState);

        _remainingLifes = serializedState.remainingLifes;
        _startTime = Time.time - (TIMER_LENGTH - serializedState.remainingTime);
        _currentLevel = serializedState.currentLevel;
        _currentObjectAmount = OBJECT_AMOUNT_PROGRESSION * _currentLevel;

        // Update level and life display
        LevelUpdate.Invoke(_currentLevel);
        for (int i = _remainingLifes; i < STARTING_LIFES; i++)
        {
            PlayerDied.Invoke(STARTING_LIFES, i + 1);
        }
    }


    /*
     * Saves highscore information.
     */
    private void SaveHighscoreInformation(string name)
    {
        if (HighscoreData == null)
        {
            HighscoreData = new SerializedHighscores();

        }

        HighscoreData.Highscores.Add(new Highscore(5, name));
        HighscoreData.Serialize();
        // Overwritting old file
        File.WriteAllText(SAVE_PATH_HIGHSCORES, JsonUtility.ToJson(HighscoreData));
    }

    private void LoadHighscoreInformation()
    {
        if (File.Exists(SAVE_PATH_HIGHSCORES))
        {
            var json = File.ReadAllText(SAVE_PATH_HIGHSCORES);
            HighscoreData = new SerializedHighscores();
            JsonUtility.FromJsonOverwrite(json, HighscoreData);
        }

    }
}

