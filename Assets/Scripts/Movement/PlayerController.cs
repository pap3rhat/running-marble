using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool _isGrounded;
    [SerializeField] private float _playerSpeed = 10.0f;
    [SerializeField] private float _jumpHeight = 1.0f;

    private Rigidbody _playerRigidBody;
    private InputManager _inputManager;
    private Transform _cameraTransform;
    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Start()
    {
        _playerRigidBody = GetComponent<Rigidbody>();
        _inputManager = InputManager.Instance;
        _cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        Vector2 movement = _inputManager.GetPlayerMovement();
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        move = _cameraTransform.forward * move.z + _cameraTransform.right * move.x;
        move.y = 0f;
        Debug.DrawLine(_playerRigidBody.transform.position, _playerRigidBody.transform.position + move);
        _playerRigidBody.AddForce(move * _playerSpeed * Time.deltaTime, ForceMode.Impulse);

        // Changes the height position of the player..
        if (_inputManager.PlayerJumped() && _isGrounded)
        {
            _playerRigidBody.AddForce(Vector3.up * _jumpHeight, ForceMode.Impulse);
        }
    }


    private void OnCollisionStay(Collision collision)
    {
        _isGrounded = true;
    }

    void OnCollisionEnter()
    {
        _isGrounded = true;
    }

    void OnCollisionExit()
    {
        _isGrounded = false;
    }
}