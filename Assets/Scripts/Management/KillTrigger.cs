using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTrigger : MonoBehaviour
{
    private GameManager _gameManager;
    private string _playerObjName = "Player";

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        _gameManager = GameManager.Instance; // needs to be done sadly, so respwn sequence is not playing if player died
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains(_playerObjName))
        {
            SignalBus.Fire(new PlayerDiedSignal { RespawnSequence = _gameManager.RemainingLifes != 1});
        }
    }
}
