using System.Collections;
using TMPro;
using UnityEngine;

public class CentralMessageDisplay : MonoBehaviour, ISubscriber<NewGameSignal>, ISubscriber<ContinueFromSaveFileSignal>, ISubscriber<PlayerDiedSignal>, ISubscriber<GameOverSignal>
{
    private GameManager _gameManager;

    [SerializeField] private GameObject _centralMessageObject;
    [SerializeField] private TextMeshProUGUI _centralMessageText;

    private string _diedText = "You Died!";

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        _gameManager = GameManager.Instance;

        SignalBus.Subscribe<NewGameSignal>(this);
        SignalBus.Subscribe<ContinueFromSaveFileSignal>(this);
        SignalBus.Subscribe<PlayerDiedSignal>(this);
        SignalBus.Subscribe<GameOverSignal>(this);
        _gameManager.RespawnMessageTime = 6f; // setting how long respawning text takes to be displayed
        _gameManager.SpawnMessageTime = 5f; // setting how long text takes to be displayed

        TurnOff();
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<NewGameSignal>(this);
        SignalBus.Unsubscribe<ContinueFromSaveFileSignal>(this);
        SignalBus.Unsubscribe<PlayerDiedSignal>(this);
        SignalBus.Unsubscribe<GameOverSignal>(this);
    }

    private void OnApplicationQuit()
    {
        SignalBus.Unsubscribe<NewGameSignal>(this);
        SignalBus.Unsubscribe<ContinueFromSaveFileSignal>(this);
        SignalBus.Unsubscribe<PlayerDiedSignal>(this);
        SignalBus.Unsubscribe<GameOverSignal>(this);
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void TurnOff()
    {
        _centralMessageObject.SetActive(false);
        StopAllCoroutines();
        Time.timeScale = 1f;
    }

    private void TurnOn()
    {
        Time.timeScale = 0f;
        _centralMessageObject.SetActive(true);
    }


    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /* SPAWNING */
    private void StartDisplay()
    {
        StartCoroutine(DisplayStart());
    }

    private IEnumerator DisplayStart()
    {
        TurnOn();
        _centralMessageText.alpha = 1;
        _centralMessageText.SetText("Starting in...");
        yield return StartCoroutine(FadeOut(_centralMessageText));
        _centralMessageText.SetText("3...");
        yield return StartCoroutine(FadeOut(_centralMessageText));
        _centralMessageText.SetText("2...");
        yield return StartCoroutine(FadeOut(_centralMessageText));
        _centralMessageText.SetText("1...");
        yield return StartCoroutine(FadeOut(_centralMessageText));
        _centralMessageText.SetText("GO!");
        yield return StartCoroutine(FadeOut(_centralMessageText));
        TurnOff();

        SignalBus.Fire(new StartSignal());
    }

    /* RESPAWNING */
    private void RespawnDisplay()
    {
        StartCoroutine(DisplayRespawn());
    }

    private IEnumerator DisplayRespawn()
    {
        // You died message
        TurnOn();
        _centralMessageText.alpha = 1;
        _centralMessageText.text = _diedText;
        yield return new WaitForSecondsRealtime(1);

        // starting respawn messages 
        _centralMessageText.SetText("Respawning...");
        yield return StartCoroutine(FadeOut(_centralMessageText));
        _centralMessageText.SetText("3...");
        yield return StartCoroutine(FadeOut(_centralMessageText));
        _centralMessageText.SetText("2...");
        yield return StartCoroutine(FadeOut(_centralMessageText));
        _centralMessageText.SetText("1...");
        yield return StartCoroutine(FadeOut(_centralMessageText));
        _centralMessageText.SetText("GO!");
        yield return StartCoroutine(FadeOut(_centralMessageText));
        TurnOff();
    }


    /* GENERAL */

    /*
     * Fades out text.
     */
    private IEnumerator FadeOut(TMP_Text text)
    {
        float t = 0;
        while (t <= 1)
        {
            t += Time.unscaledDeltaTime;
            text.alpha = 1 - t;
            yield return null;
        }
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    public void OnEventHappen(NewGameSignal e)
    {
        StartDisplay();
    }

    public void OnEventHappen(ContinueFromSaveFileSignal e)
    {
        StartDisplay();
    }

    public void OnEventHappen(PlayerDiedSignal e)
    {
        if (e.RespawnSequence)
        {
            RespawnDisplay();
        } 
    }

    public void OnEventHappen(GameOverSignal e)
    {
        _centralMessageObject.SetActive(false);
        StopAllCoroutines();
    }
}
