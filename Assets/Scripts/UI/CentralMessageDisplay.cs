using TMPro;
using UnityEngine;

public class CentralMessageDisplay : MonoBehaviour
{
    private GameManager _gameManager;

    [SerializeField] private GameObject _centralMessageObject;
    [SerializeField] private TextMeshProUGUI _centralMessageText;

    private string _diedText = "You Died!";

    private string _gameOverText = "Game Over!";

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        _gameManager = GameManager.Instance;

        _gameManager.Death.AddListener(OnDeath);
        _gameManager.GameOver.AddListener(OnGameOver);

        _centralMessageObject.SetActive(false);
    }


    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void OnGameOver(bool display)
    {
        if (display)
        {
            _centralMessageText.text = _gameOverText;
        }

        _centralMessageObject.SetActive(display);
    }

    private void OnDeath(bool display)
    {
        if (display)
        {
            _centralMessageText.text = _diedText;
        }

        _centralMessageObject.SetActive(display);
    }
}
