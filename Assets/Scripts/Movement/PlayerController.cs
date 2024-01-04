using System.Collections;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _playerSpeed = 20.0f;
    [SerializeField] private float _boostStrength = 250f;
    [SerializeField] private float _bouncerStrength = 1.0f;

    private Rigidbody _playerRigidBody;
    private InputManager _inputManager;
    private Transform _cameraTransform;

    // Controls boost cooldown & vfx length
    private bool _isBoosting = false;
    private float _vfxLength = 2f;
    private float _boostCooldown = 5f;

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

        // Check if player boosted
        if (_inputManager.PlayerBoosted() && !_isBoosting)
        {
            _playerRigidBody.AddForce(move * _playerSpeed * _boostStrength * Time.deltaTime, ForceMode.Impulse);
            StartCoroutine(BoostControl());
        }
    }

    void OnCollisionEnter(Collision collison)
    {
        if (collison.gameObject.name.Contains("Bouncer"))
        {
            _playerRigidBody.AddForce(collison.relativeVelocity * _bouncerStrength, ForceMode.Impulse);
        }
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private IEnumerator BoostControl()
    {
        _isBoosting = true;
        UniversalRenderPipelineUtils.SetRendererFeatureActive("SpeedLines", true);
        yield return new WaitForSeconds(_vfxLength);
        UniversalRenderPipelineUtils.SetRendererFeatureActive("SpeedLines", false);
        yield return new WaitForSeconds(_boostCooldown - _vfxLength);
        _isBoosting = false;
    
    }
}