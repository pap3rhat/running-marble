using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointLogic : MonoBehaviour
{
    private GameManager _gameManager;

    private string _playerObjName = "Player";

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Start()
    {
        _gameManager = GameManager.Instance;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Collision");
        Debug.Log(other.gameObject.name);

        if (other.gameObject.name.Contains(_playerObjName))
        {
            _gameManager.CheckpointReached();
        }
    }
}
