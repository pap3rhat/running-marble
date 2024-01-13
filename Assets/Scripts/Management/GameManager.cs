using Cinemachine;
using System;
using System.Collections;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
public class GameManager : MonoBehaviour, ISubscriber<NewGameSignal>, ISubscriber<PlayerDiedSignal>, ISubscriber<ContinueFromSaveFileSignal>, ISubscriber<PauseSignal>, ISubscriber<StartSignal>, ISubscriber<BackToMainMenuSignal>, ISubscriber<SaveHighscoreSignal>, ISubscriber<CheckpointSignal>
{
    // Saving system
    public string SAVE_PATH_GAME_INFORMATION;
    public string SAVE_PATH_HIGHSCORES;
    public SerializedHighscores HighscoreData;
    private bool _highscoreSubmitted = false;

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
    public float TimerLength = 60f;

    // Lifes
    private static int STARTING_LIFES = 3;
    private int _remainingLifes;

    // Level Progression
    private int _currentLevel = 1;
    private int _currentObjectAmount = 5;
    private static int OBJECT_AMOUNT_PROGRESSION = 5;
    private PopulateModule _popMod;

    // Respawn
    private bool _playerAlive = false;
    [HideInInspector] public float RespawnMessageTime; // used to control how long respawning message is shown; gets set be RespawingDisplay UI class, because that one control coroutine

    // Time out -> Game Over
    public bool DeathHappened = false; // gets set by killtrigger
    // Pause
    private bool _isPaused = false;

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


        // Subscribing to events
        SignalBus.Subscribe<NewGameSignal>(this);
        SignalBus.Subscribe<PlayerDiedSignal>(this);
        SignalBus.Subscribe<ContinueFromSaveFileSignal>(this);
        SignalBus.Subscribe<PauseSignal>(this);
        SignalBus.Subscribe<StartSignal>(this);
        SignalBus.Subscribe<BackToMainMenuSignal>(this);
        SignalBus.Subscribe<SaveHighscoreSignal>(this);
        SignalBus.Subscribe<CheckpointSignal>(this);

        // Activating fog, to enhance illusion of endlessness
        RenderSettings.fog = true;

        // Loading Highscores
        LoadHighscoreInformation();

        // Access to populate class
        _popMod = GameObject.Find("Base Module").GetComponentInChildren<PopulateModule>();
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

        // Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SignalBus.Fire(new PauseSignal { IsPaused = !_isPaused });
        }

        // Only update time if game is not paused
        if (!_isPaused)
        {
            // If no time is left, game is over
            if (Time.time - _startTime >= TimerLength)
            {
                GameIsOver();
            }
            // Updating Countdown
            SignalBus.Fire(new RemainingTimeSignal { RemainingTime = (float)Math.Round(TimerLength - Time.time + _startTime, 2) });
        }
    }

    private void OnApplicationQuit()
    {

        // TODO: check if this truely only saves when player is still playing
        // Saving game information if user closes application and is still in play
        if (_remainingLifes > 0 && Time.time - _startTime < TimerLength)
        {
            SaveGameInformation();
        }

        SignalBus.Unsubscribe<NewGameSignal>(this);
        SignalBus.Subscribe<PlayerDiedSignal>(this);
        SignalBus.Unsubscribe<ContinueFromSaveFileSignal>(this);
        SignalBus.Unsubscribe<PauseSignal>(this);
        SignalBus.Unsubscribe<StartSignal>(this);
        SignalBus.Unsubscribe<BackToMainMenuSignal>(this);
        SignalBus.Unsubscribe<SaveHighscoreSignal>(this);
        SignalBus.Unsubscribe<SaveHighscoreSignal>(this);
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<NewGameSignal>(this);
        SignalBus.Subscribe<PlayerDiedSignal>(this);
        SignalBus.Unsubscribe<ContinueFromSaveFileSignal>(this);
        SignalBus.Unsubscribe<PauseSignal>(this);
        SignalBus.Unsubscribe<StartSignal>(this);
        SignalBus.Unsubscribe<BackToMainMenuSignal>(this);
        SignalBus.Unsubscribe<SaveHighscoreSignal>(this);
        SignalBus.Unsubscribe<SaveHighscoreSignal>(this);
    }

    /*--- METHODS OTHER CLASSES CALL (this should not be done like this, but oh well, that is how it is now) -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    // I NEED A SIGNAL BUS ASAP, UI IS RUINING MY LIFE (AND CODE)
    // AND A SEPERATE CALSS TO SAVE STUFF; AHAHAHRHUJHHW
    // STUFF SHOULD BE PLANNED AND NOT GROW "DYNAMICALLY" FM



    /*--- GAME LOOP -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /*
     * Cleans up everything after game is over.
     */
    private void GameIsOver()
    {
        // just clear everything in UI that is not Game Over Menu
        SignalBus.Fire(new GameOverSignal());
        SignalBus.Fire(new GameUIOffSignal());
        // Setting player to dead, and disallowing themto move
        _playerAlive = false;
        _inputManager.TriggerDisable();

        // Deleting scene
        _popMod.DepopulatePrefabs();
        // Destroying player
        DestroyPlayer();
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
        _remainingLifes--;
        DestroyPlayer();

        // Player cannot move anymore
        _inputManager.TriggerDisable();

        // Use next life if possible
        if (_remainingLifes > 0)
        {
            // Respawning player
            SpawnPlayer();

            // Wait for respawn message to have been fully displayed
            yield return new WaitForSeconds(RespawnMessageTime);

            // Setting player to be alive again
            _playerAlive = true;

            // Correcting time
            _startTime -= (respawnStartTime - Time.time);

            // player can move again
            _inputManager.TriggerEnable();
        }
        else
        {
            GameIsOver();
        }
    }

    /* 
     * Handles spawning of player.
     */
    private void SpawnPlayer()
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
    }

    /*
     * Handles destroying of player
     */
    private void DestroyPlayer()
    {
        Destroy(_currentPlayerObject);
    }

    /*--- SAVING SYSTEM FOR GAME INFORMATION -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

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
        SerializedState serializedState = new SerializedState(_remainingLifes, TimerLength - (Time.time - _startTime), _currentLevel);
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
        _startTime = Time.time - (TimerLength - serializedState.remainingTime);
        _currentLevel = serializedState.currentLevel;
        _currentObjectAmount = OBJECT_AMOUNT_PROGRESSION * _currentLevel;

        // Update level and life display
        SignalBus.Fire(new LevelUpdateSignal { Level = _currentLevel });
        for (int i = _remainingLifes; i < STARTING_LIFES; i++)
        {
            SignalBus.Fire(new PlayerDiedSignal());
        }

        // Deleting file, no use for it anymore
        File.Delete(SAVE_PATH_GAME_INFORMATION);
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

        HighscoreData.Highscores.Add(new Highscore(_currentLevel, name));
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

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /*
     * A new game has been started.
     */
    public void OnEventHappen(NewGameSignal e)
    {
        // Setting everything to default -> need when starting new game while a also having already played a game in the same session
        _remainingLifes = STARTING_LIFES;
        _startTime = Time.time;
        _currentLevel = 1;
        SignalBus.Fire(new LevelUpdateSignal { Level = _currentLevel });
        _currentObjectAmount = OBJECT_AMOUNT_PROGRESSION;
        _highscoreSubmitted = false;

        // Setting up first level up
        _popMod.PopulateWithPrefab(_currentObjectAmount);

        // Player
        SpawnPlayer();
    }

    /*
     * Player died.
     */
    public void OnEventHappen(PlayerDiedSignal e)
    {
        StartCoroutine(Respawn());
    }

    /*
     * Continue run from save file.
     */
    public void OnEventHappen(ContinueFromSaveFileSignal e)
    {
        LoadGameInformation();
        _popMod.PopulateWithPrefab(_currentObjectAmount);
        SpawnPlayer();
        _isPaused = false;
        _highscoreSubmitted = false;
    }

    /*
     * Pause and continue.
     */
    public void OnEventHappen(PauseSignal e)
    {
        _isPaused = e.IsPaused;
    }

    /*
     * Start.
     */
    public void OnEventHappen(StartSignal e)
    {
        // player can move and is alive
        _playerAlive = true;
        _inputManager.TriggerEnable();

        // No pause
        _isPaused = false;

        // Countdown
        _startTime = (float)Math.Round(Time.time, 2);
    }

    /*
     * Game stops, because player goes back to main menu.
     */
    public void OnEventHappen(BackToMainMenuSignal e)
    {
        // Telling Game UI that main menu is there now and they have to go away
        SignalBus.Fire(new GameUIOffSignal());

        // Deleting scene
        _popMod.DepopulatePrefabs();
        // Destroying player
        DestroyPlayer();

        // Saving game information if user came out of unfinished game
        if (_remainingLifes > 0 && Time.time - _startTime < TimerLength)
        {
            SaveGameInformation();
        }
    }

    /*
     * Save highscore.
     */
    public void OnEventHappen(SaveHighscoreSignal e)
    {
        if (!_highscoreSubmitted)
        {
            SaveHighscoreInformation(e.PlayerName);
            _highscoreSubmitted = true;
        }
    }

    /*
     * Checkpoint reached.
     */
    public void OnEventHappen(CheckpointSignal e)
    {
        // Updating Level display
        _currentLevel++;
        SignalBus.Fire(new LevelUpdateSignal { Level = _currentLevel });

        // Ereasing old level
        _popMod.DepopulatePrefabs();
        // Creating new level
        _currentObjectAmount += OBJECT_AMOUNT_PROGRESSION;
        _popMod.PopulateWithPrefab(_currentObjectAmount);

        // Setting player back, but keeping x and y coordinate, so it is not as obvious
        Vector3 playerPosition = _currentPlayerObject.transform.position;
        _currentPlayerObject.transform.position = new Vector3(playerPosition.x, playerPosition.y, _playerSpawnPosition.z);
    }
}

