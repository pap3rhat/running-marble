using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeDisplay : MonoBehaviour, ISubscriber<GameUIOffSignal>, ISubscriber<NewGameSignal>, ISubscriber<PlayerDiedSignal>
{
    [SerializeField] private GameObject _lifeDisplay;
    [SerializeField] private List<GameObject> _lifesIcons;
    [SerializeField] private Sprite _deadLifeIcon;
    [SerializeField] private Sprite _aliveLifeIcon;

    private int _dark = 0;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        _lifeDisplay.SetActive(false);
    }

    void Start()
    {
        SignalBus.Subscribe<PlayerDiedSignal>(this);
        SignalBus.Subscribe<GameUIOffSignal>(this);
        SignalBus.Subscribe<NewGameSignal>(this);
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<GameUIOffSignal>(this);
        SignalBus.Unsubscribe<NewGameSignal>(this);
        SignalBus.Unsubscribe<PlayerDiedSignal>(this);
    }

    private void OnApplicationQuit()
    {
        SignalBus.Unsubscribe<GameUIOffSignal>(this);
        SignalBus.Unsubscribe<NewGameSignal>(this);
        SignalBus.Unsubscribe<PlayerDiedSignal>(this);
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    public void OnEventHappen(GameUIOffSignal e)
    {
        _lifeDisplay.SetActive(false);
    }

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

    // TODO when game from continue -> reset dark to 0, bzw use save system to call set up method -> better
}
