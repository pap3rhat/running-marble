using TMPro;
using UnityEngine;

public class CentralMessageDisplay : MonoBehaviour
{
    private GameManager _gameManager;

    [SerializeField] private GameObject _centralMessageObject;
    [SerializeField] private TextMeshProUGUI _centralMessageText;

    private string _winText = "Winner!";
    private string _loseText = "Loser!";

    private string _diedText = "You Died!";

    private string _timeoutText = "time out!";

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        _gameManager = GameManager.Instance;

        _gameManager.MiscDeath.AddListener(OnMiscDeath);
        _gameManager.TimeOut.AddListener(OnTimeOut);
        _gameManager.EndState.AddListener(OnEndState);

        _centralMessageObject.SetActive(false);
    }


    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void OnTimeOut(bool display)
    {
        if (display)
        {
            _centralMessageText.text = _timeoutText;
        }

        _centralMessageObject.SetActive(display);
    }

    private void OnMiscDeath(bool display)
    {
        if (display)
        {
            _centralMessageText.text = _diedText;
        }

        _centralMessageObject.SetActive(display);
    }

    private void OnEndState(bool won)
    {
        if (won)
        {
            _centralMessageText.text = _winText;
        }
        else
        {
            _centralMessageText.text = _loseText;
        }

        _centralMessageObject.SetActive(true);
    }
}
