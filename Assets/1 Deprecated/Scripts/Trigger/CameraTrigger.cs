using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    // Input Manager
    private InputManager _inputManager;

    // Camera Information
    [SerializeField] private CinemachineStateDrivenCamera _stateCamera;
    [SerializeField] private Animator _animator;

    // --- Getter and Setter  ---
    public CinemachineStateDrivenCamera StateCamera { get => _stateCamera; set => _stateCamera = value; }
    public Animator Animator { get => _animator; set => _animator = value; }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        _inputManager = InputManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.ToLower().Contains("camera"))
        {
            string[] parts = other.name.Split(" ");
            _animator.Play(parts[0]);

            // Disable player movement if camera gets switched to top-down
            if (parts[0].Equals("TopDownCamera"))
            {
                _inputManager.TriggerDisable();
            } // otherwise enable player movemnt
            else
            {
                _inputManager.TriggerEnable();
            }
        }
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/


}
