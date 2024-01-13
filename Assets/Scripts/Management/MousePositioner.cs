using UnityEngine;

public class MousePositioner : MonoBehaviour, ISubscriber<PauseSignal>, ISubscriber<GameOverSignal>, ISubscriber<NewGameSignal>, ISubscriber<ContinueFromSaveFileSignal>
{
    private void Awake()
    {
        LetItMove();
        SignalBus.Subscribe<PauseSignal>(this);
        SignalBus.Subscribe<GameOverSignal>(this);
        SignalBus.Subscribe<NewGameSignal>(this);
        SignalBus.Subscribe<ContinueFromSaveFileSignal>(this);
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<PauseSignal>(this);
        SignalBus.Unsubscribe<GameOverSignal>(this);
        SignalBus.Unsubscribe<NewGameSignal>(this);
        SignalBus.Unsubscribe<ContinueFromSaveFileSignal>(this);
    }

    private void OnApplicationQuit()
    {
        SignalBus.Unsubscribe<PauseSignal>(this);
        SignalBus.Unsubscribe<GameOverSignal>(this);
        SignalBus.Unsubscribe<NewGameSignal>(this);
        SignalBus.Unsubscribe<ContinueFromSaveFileSignal>(this);
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void PositionInCenter()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LetItMove()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    public void OnEventHappen(PauseSignal e)
    {
        if (e.IsPaused)
        {
            LetItMove();
        }
        else
        {
            PositionInCenter();
        }
    }

    public void OnEventHappen(GameOverSignal e)
    {
        LetItMove();
    }

    public void OnEventHappen(NewGameSignal e)
    {
        PositionInCenter();
    }

    public void OnEventHappen(ContinueFromSaveFileSignal e)
    {
        PositionInCenter();
    }
}
