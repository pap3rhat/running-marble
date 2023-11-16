using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDeathDisplay : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;

    // Fall Death
    [SerializeField] private GameObject _fallDeathObj;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    void Start()
    {
        _gameManager.FallDeath.AddListener(on => _fallDeathObj.SetActive(on));
    }

}
