using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    // Camera Information
    [SerializeField] private CinemachineStateDrivenCamera _stateCamera;
    [SerializeField] private Animator _animator;

    // --- Getter and Setter  ---
    public CinemachineStateDrivenCamera StateCamera { get => _stateCamera; set => _stateCamera = value; }
    public Animator Animator { get => _animator; set => _animator = value; }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void OnTriggerEnter(Collider other)
    {
        string[] parts = other.name.Split(" ");
        _animator.Play(parts[0]);
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/


}
