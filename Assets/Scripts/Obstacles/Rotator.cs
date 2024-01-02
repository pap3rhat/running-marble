using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{

    [SerializeField] private GameObject _rotator;
    [SerializeField] private Vector3 _roationVector;
    private float _rotationSpeed;


    /*----------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        if (Random.Range(0, 1) < 0.5) _roationVector *= -1;
        _rotationSpeed = Random.Range(1, 5);
    }

    void Update()
    {
        var parent = _rotator.transform.parent;
        _rotator.transform.parent = null;
        _rotator.transform.Rotate(_roationVector * _rotationSpeed);
        // reattaching did not work, why?
        //_rotator.transform.SetParent(parent,true);
    }
}
