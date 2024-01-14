using TMPro;
using UnityEngine;

public class TimerDisplay : MonoBehaviour, ISubscriber<RemainingTimeSignal>, ISubscriber<BackToMainMenuSignal>, ISubscriber<ContinueFromSaveFileSignal>, ISubscriber<GameOverSignal>, ISubscriber<NewGameSignal>
{
    private GameManager _gameManager;

    // Time Count down
    [SerializeField] private GameObject _countdownObject;
    [SerializeField] private TextMeshProUGUI _countdownText;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    private void Awake()
    {
        _gameManager = GameManager.Instance;

        SignalBus.Subscribe<RemainingTimeSignal>(this);
        SignalBus.Subscribe<BackToMainMenuSignal>(this);
        SignalBus.Subscribe<ContinueFromSaveFileSignal>(this);
        SignalBus.Subscribe<GameOverSignal>(this);
        SignalBus.Subscribe<NewGameSignal>(this);

        _countdownText.text = $"{_gameManager.TimerLength:00.00}";

        TurnOff();
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<RemainingTimeSignal>(this);
        SignalBus.Unsubscribe<BackToMainMenuSignal>(this);
        SignalBus.Unsubscribe<ContinueFromSaveFileSignal>(this);
        SignalBus.Unsubscribe<GameOverSignal>(this);
        SignalBus.Unsubscribe<NewGameSignal>(this);
    }

    private void OnApplicationQuit()
    {
        SignalBus.Unsubscribe<RemainingTimeSignal>(this);
        SignalBus.Unsubscribe<BackToMainMenuSignal>(this);
        SignalBus.Unsubscribe<ContinueFromSaveFileSignal>(this);
        SignalBus.Unsubscribe<GameOverSignal>(this);
        SignalBus.Unsubscribe<NewGameSignal>(this);
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void TurnOff()
    {
        _countdownObject.SetActive(false);
    }

    private void TurnOn()
    {
        _countdownObject.SetActive(true);
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    public void OnEventHappen(RemainingTimeSignal e)
    {
        _countdownText.text = $"{e.RemainingTime:00.00}";
    }

    public void OnEventHappen(BackToMainMenuSignal e)
    {
        TurnOff();
    }

    public void OnEventHappen(ContinueFromSaveFileSignal e)
    {
        TurnOn();
    }

    public void OnEventHappen(GameOverSignal e)
    {
        TurnOff();
    }

    public void OnEventHappen(NewGameSignal e)
    {
        TurnOn();
    }
}
