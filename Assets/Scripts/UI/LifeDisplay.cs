using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeDisplay : MonoBehaviour
{
    private GameManager _gameManager;

    [SerializeField] private List<GameObject> _lifesIcons;
    [SerializeField] private Sprite _deadLifeIcon;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        _gameManager = GameManager.Instance;
    }

    void Start()
    {
        _gameManager.PlayerDied.AddListener(OnPlayerDied);
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /*
     * Handles life display when player dies.
     */
    private void OnPlayerDied(int startingLifes, int remainingLifes)
    {
        _lifesIcons[startingLifes - remainingLifes].GetComponent<Image>().sprite = _deadLifeIcon;
    }
}
