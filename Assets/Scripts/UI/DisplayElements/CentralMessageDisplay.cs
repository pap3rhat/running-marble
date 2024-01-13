using System.Collections;
using TMPro;
using UnityEngine;

public class CentralMessageDisplay : MonoBehaviour, ISubscriber<GameUIOffSignal>, ISubscriber<NewGameSignal>, ISubscriber<ContinueFromSaveFileSignal>, ISubscriber<PlayerDiedSignal>
{
    private GameManager _gameManager;

    [SerializeField] private GameObject _centralMessageObject;
    [SerializeField] private TextMeshProUGUI _centralMessageText;

    private string _diedText = "You Died!";

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        _gameManager = GameManager.Instance;

        SignalBus.Subscribe<GameUIOffSignal>(this);
        SignalBus.Subscribe<NewGameSignal>(this);
        SignalBus.Subscribe<ContinueFromSaveFileSignal>(this);
        SignalBus.Subscribe<PlayerDiedSignal>(this);
        _gameManager.RespawnMessageTime = 6f; // setting how long respawning text takes to be displayed
        _centralMessageObject.SetActive(false);
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<GameUIOffSignal>(this);
        SignalBus.Unsubscribe<NewGameSignal>(this);
        SignalBus.Unsubscribe<ContinueFromSaveFileSignal>(this);
        SignalBus.Unsubscribe<PlayerDiedSignal>(this);
    }

    private void OnApplicationQuit()
    {
        SignalBus.Unsubscribe<GameUIOffSignal>(this);
        SignalBus.Unsubscribe<NewGameSignal>(this);
        SignalBus.Unsubscribe<ContinueFromSaveFileSignal>(this);
        SignalBus.Unsubscribe<PlayerDiedSignal>(this);
    }



    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /* SPAWNING */
    private void StartDisplay()
    {
        StartCoroutine(DisplayStart());
    }

    private IEnumerator DisplayStart()
    {
        _centralMessageObject.SetActive(true);
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
        _centralMessageObject.SetActive(false);

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
        _centralMessageObject.SetActive(true);
        _centralMessageText.alpha = 1;
        _centralMessageText.text = _diedText;
        yield return new WaitForSeconds(1);

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
        _centralMessageObject.SetActive(false);
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
            t += Time.deltaTime;
            text.alpha = 1 - t;
            yield return null;
        }
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    public void OnEventHappen(GameUIOffSignal e)
    {
        _centralMessageObject.SetActive(false);
        StopAllCoroutines();
    }

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
        RespawnDisplay();
    }
}
