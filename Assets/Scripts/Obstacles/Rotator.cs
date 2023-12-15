using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{

    [SerializeField] private GameObject _rotator;
    [SerializeField] private Vector3 _roationVector;

    /*----------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    void Update()
    {
        _rotator.transform.Rotate(_roationVector);        
    }
}
