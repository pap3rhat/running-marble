using TMPro;
using UnityEngine;

public class TimerDisplay : MonoBehaviour, ISubscriber<GameUIOffSignal>, ISubscriber<RemainingTimeSignal>
{
    private GameManager _gameManager;

    // Time Count down
    [SerializeField] private GameObject _countdownObject;
    [SerializeField] private TextMeshProUGUI _countdownText;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    private void Awake()
    {
        _gameManager = GameManager.Instance;

        SignalBus.Subscribe<GameUIOffSignal>(this);
        SignalBus.Subscribe<RemainingTimeSignal>(this);
        _countdownText.text = $"{_gameManager.TimerLength:00.00}";
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<GameUIOffSignal>(this);
        SignalBus.Unsubscribe<RemainingTimeSignal>(this);
    }

    private void OnApplicationQuit()
    {
        SignalBus.Unsubscribe<GameUIOffSignal>(this);
        SignalBus.Unsubscribe<RemainingTimeSignal>(this);
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    public void OnEventHappen(GameUIOffSignal e)
    {
        _countdownObject.SetActive(false);
    }

    public void OnEventHappen(RemainingTimeSignal e)
    {
        _countdownText.text = $"{e.RemainingTime:00.00}";
    }
}
