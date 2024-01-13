using TMPro;
using UnityEngine;
public class LevelDisplay : MonoBehaviour, ISubscriber<LevelUpdateSignal>
{
    // Level Display Text -> Never hidden, because menu just appears above it and blocks it
    [SerializeField] private TextMeshProUGUI _levelText;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        SignalBus.Subscribe<LevelUpdateSignal>(this);
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<LevelUpdateSignal>(this);
    }

    private void OnApplicationQuit()
    {
        SignalBus.Unsubscribe<LevelUpdateSignal>(this);
    }


    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    public void OnEventHappen(LevelUpdateSignal e)
    {
        _levelText.text = $"Level: {e.Level}";
    }
}
