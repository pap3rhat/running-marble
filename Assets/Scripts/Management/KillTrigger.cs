using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTrigger : MonoBehaviour
{
    // Connection to Manager
    private GameManager _gameManager;

    private string _playerObjName = "Player";

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Start()
    {
        _gameManager = GameManager.Instance;
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains(_playerObjName))
        {
            _gameManager.MiscDeathHappened = true;
        }
    }
}
