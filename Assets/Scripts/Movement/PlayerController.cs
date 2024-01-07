using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _playerSpeed = 20.0f;
    [SerializeField] private float _boostStrength = 250f;
    [SerializeField] private float _bouncerStrength = 1.0f;

    private Rigidbody _playerRigidBody;
    private InputManager _inputManager;
    private Transform _cameraTransform;

    private GameManager _gameManager;
    private UnityEngine.Rendering.Universal.Bloom _bloom;

    // Controls boost cooldown & vfx length
    private bool _isBoosting = false;
    private float _vfxLength = 2f;
    private float _boostCooldown = 5f;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        _gameManager = GameManager.Instance;
        _gameManager.RespawnCountdown.AddListener(StopBoostVFX);
        _gameManager.GameOver.AddListener(StopBoostVFX);

        var cam = GameObject.Find("Main Camera");
        var vol = cam.GetComponent<Volume>();
        if (!vol.profile.TryGet(out _bloom)) throw new System.NullReferenceException(nameof(_bloom));

        UniversalRenderPipelineUtils.SetRendererFeatureActive("SpeedLines", false);
    }

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

    private void OnDestroy()
    {
        UniversalRenderPipelineUtils.SetRendererFeatureActive("SpeedLines", false);
    }

    private void OnDisable()
    {
        UniversalRenderPipelineUtils.SetRendererFeatureActive("SpeedLines", false);
    }

    private void OnApplicationQuit()
    {
        UniversalRenderPipelineUtils.SetRendererFeatureActive("SpeedLines", false);
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private IEnumerator BoostControl()
    {
       
        _bloom.intensity.Override(0f);

        _isBoosting = true;
        UniversalRenderPipelineUtils.SetRendererFeatureActive("SpeedLines", true);
        yield return new WaitForSeconds(_vfxLength);
        UniversalRenderPipelineUtils.SetRendererFeatureActive("SpeedLines", false);
        yield return new WaitForSeconds(_boostCooldown - _vfxLength);

        _bloom.intensity.Override(1f);
        _isBoosting = false;
       

    }

    private void StopBoostVFX()
    {
        // Stopping Boost Effect if it is happening
        StopCoroutine(BoostControl());
        // Deactivating Render Feature
        UniversalRenderPipelineUtils.SetRendererFeatureActive("SpeedLines", false);
        // Letting player immediatly boost again
        _bloom.intensity.Override(1f);
        _isBoosting = false;
    }
}