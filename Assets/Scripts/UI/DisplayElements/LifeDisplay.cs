using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeDisplay : MonoBehaviour,ISubscriber<NewGameSignal>, ISubscriber<PlayerDiedSignal>, ISubscriber<BackToMainMenuSignal>, ISubscriber<GameOverSignal>, ISubscriber<SpecificLifeSignal>
{
    [SerializeField] private GameObject _lifeDisplay;
    [SerializeField] private List<GameObject> _lifesIcons;
    [SerializeField] private Sprite _deadLifeIcon;
    [SerializeField] private Sprite _aliveLifeIcon;

    private int _dark = 0;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        TurnOff();
    }

    void Start()
    {
        SignalBus.Subscribe<PlayerDiedSignal>(this);
        SignalBus.Subscribe<NewGameSignal>(this);
        SignalBus.Subscribe<BackToMainMenuSignal>(this);
        SignalBus.Subscribe<GameOverSignal>(this);
        SignalBus.Subscribe<SpecificLifeSignal>(this);
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<NewGameSignal>(this);
        SignalBus.Unsubscribe<PlayerDiedSignal>(this);
        SignalBus.Unsubscribe<BackToMainMenuSignal>(this);
        SignalBus.Unsubscribe<GameOverSignal>(this);
        SignalBus.Unsubscribe<SpecificLifeSignal>(this);
    }

    private void OnApplicationQuit()
    {
        SignalBus.Unsubscribe<NewGameSignal>(this);
        SignalBus.Unsubscribe<PlayerDiedSignal>(this);
        SignalBus.Unsubscribe<BackToMainMenuSignal>(this);
        SignalBus.Unsubscribe<GameOverSignal>(this);
        SignalBus.Unsubscribe<SpecificLifeSignal>(this);
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void TurnOff()
    {
        _lifeDisplay.SetActive(false);
    }

    private void TurnOn()
    {
        _lifeDisplay.SetActive(true);
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    public void OnEventHappen(NewGameSignal e)
    {
        // Reset all life icons
        _dark = 0;
        foreach (var icon in _lifesIcons)
            icon.GetComponent<Image>().sprite = _aliveLifeIcon;

        _lifeDisplay.SetActive(true);
    }

    public void OnEventHappen(PlayerDiedSignal e)
    {
        _lifesIcons[_dark].GetComponent<Image>().sprite = _deadLifeIcon;
        _dark++;
    }

    public void OnEventHappen(BackToMainMenuSignal e)
    {
        _dark = 0;
        TurnOff();
    }

    public void OnEventHappen(GameOverSignal e)
    {
        _dark = 0;
        TurnOff();
    }

    public void OnEventHappen(SpecificLifeSignal e)
    {
        _lifesIcons[e.index].GetComponent<Image>().sprite = _deadLifeIcon;
        _dark++;
        TurnOn();
    }
}
