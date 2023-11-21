using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerControls _playerControls;
    private static InputManager _instance;
    public static InputManager Instance { get => _instance; }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        _playerControls = new PlayerControls();
       // Cursor.visible = false;
    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }

    private void OnDisable()
    {
        _playerControls.Disable();
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    public Vector2 GetPlayerMovement()
    {
        return _playerControls.PlayerFirstPerson.Movement.ReadValue<Vector2>();
    }

    public Vector2 GetMouseData()
    {
        return _playerControls.PlayerFirstPerson.Look.ReadValue<Vector2>();
    }

    public bool PlayerJumped()
    {
        return _playerControls.PlayerFirstPerson.Jump.triggered;
    }

}
