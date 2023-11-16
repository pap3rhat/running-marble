using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeDisplay : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;

    [SerializeField] private List<GameObject> _lifesIcons;
    [SerializeField] private Sprite _deadLifeIcon;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    // Start is called before the first frame update
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
