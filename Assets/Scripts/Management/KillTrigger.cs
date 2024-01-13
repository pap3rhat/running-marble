using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTrigger : MonoBehaviour
{

    private string _playerObjName = "Player";

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains(_playerObjName))
        {
            SignalBus.Fire(new PlayerDiedSignal());
        }
    }
}
