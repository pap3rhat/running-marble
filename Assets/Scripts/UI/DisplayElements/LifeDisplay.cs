using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeDisplay : MonoBehaviour
{
    private GameManager _gameManager;

    [SerializeField] private GameObject _lifeDisplay;
    [SerializeField] private List<GameObject> _lifesIcons;
    [SerializeField] private Sprite _deadLifeIcon;
    [SerializeField] private Sprite _aliveLifeIcon;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        _lifeDisplay.SetActive(false);
        _gameManager = GameManager.Instance;
    }

    void Start()
    {
        _gameManager.BackToMain.AddListener(() => _lifeDisplay.SetActive(false));
        _gameManager.ResetLifeDisplay.AddListener(OnResetLifeDisplay);
        _gameManager.PlayerDied.AddListener(OnPlayerDied);
        _gameManager.GameOver.AddListener(() => _lifeDisplay.SetActive(false));
        _gameManager.StartCountdown.AddListener(() => _lifeDisplay.SetActive(true));
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /*
     * Handles life display when player dies.
     */
    private void OnPlayerDied(int startingLifes, int remainingLifes)
    {
        _lifesIcons[startingLifes - remainingLifes].GetComponent<Image>().sprite = _deadLifeIcon;
    }

    /*
     * Resets all lifes to be on again.
     */
    private void OnResetLifeDisplay()
    {
        foreach (var icon in _lifesIcons)
        {
            icon.GetComponent<Image>().sprite = _aliveLifeIcon;
        }
    }
}
