using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeDisplay : MonoBehaviour
{
    private GameManager _gameManager;

    [SerializeField] private GameObject _lifeDisplay;
    [SerializeField] private List<GameObject> _lifesIcons;
    [SerializeField] private Sprite _deadLifeIcon;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        _lifeDisplay.SetActive(false);
        _gameManager = GameManager.Instance;
    }

    void Start()
    {
        _gameManager.BackToMain.AddListener(OnBackToMain);
        _gameManager.PlayerDied.AddListener(OnPlayerDied);
        _gameManager.StartCountdown.AddListener(() => _lifeDisplay.SetActive(true));
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /* BACK TO MAIN MENU */
    private void OnBackToMain()
    {
        _lifeDisplay.SetActive(false);
    }

    /*
     * Handles life display when player dies.
     */
    private void OnPlayerDied(int startingLifes, int remainingLifes)
    {
        _lifesIcons[startingLifes - remainingLifes].GetComponent<Image>().sprite = _deadLifeIcon;
    }
}
