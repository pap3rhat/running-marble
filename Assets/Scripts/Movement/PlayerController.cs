using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour, ISubscriber<GameOverSignal>, ISubscriber<PlayerDiedSignal>
{
    [SerializeField] private float _playerSpeed = 20.0f;
    [SerializeField] private float _boostStrength = 1.67f;
    [SerializeField] private float _bouncerStrength = 1.0f;

    private Rigidbody _playerRigidBody;
    private InputManager _inputManager;
    private Transform _cameraTransform;
    private Material _playerMaterial;
    private Color _boostableColor = new(0.9254901960784314f, 0.9254901960784314f, 0.9254901960784314f);
    private Color _defaultColor = new(0.42745098039215684f, 0.5843137254901961f, 1);

    // Controls boost cooldown & vfx length
    private bool _isBoosting = false;
    private float _vfxLength = 2f;
    private float _boostCooldown = 5f;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        SignalBus.Subscribe<GameOverSignal>(this);
        SignalBus.Subscribe<PlayerDiedSignal>(this);

        var cam = GameObject.Find("Main Camera");
        var vol = cam.GetComponent<Volume>();

        UniversalRenderPipelineUtils.SetRendererFeatureActive("SpeedLines", false);
    }

    private void Start()
    {
        _playerMaterial = GetComponent<Renderer>().material;
        _playerMaterial.color = _boostableColor;
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
            _playerRigidBody.AddForce(move * _playerSpeed * _boostStrength, ForceMode.Impulse);
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
        SignalBus.Unsubscribe<GameOverSignal>(this);
        SignalBus.Unsubscribe<PlayerDiedSignal>(this);
    }

    private void OnDisable()
    {
        UniversalRenderPipelineUtils.SetRendererFeatureActive("SpeedLines", false);
    }

    private void OnApplicationQuit()
    {
        UniversalRenderPipelineUtils.SetRendererFeatureActive("SpeedLines", false);
        SignalBus.Unsubscribe<GameOverSignal>(this);
        SignalBus.Unsubscribe<PlayerDiedSignal>(this);
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private IEnumerator BoostControl()
    {
        _playerMaterial.color = _defaultColor;
        _isBoosting = true;

        UniversalRenderPipelineUtils.SetRendererFeatureActive("SpeedLines", true);
        yield return new WaitForSeconds(_vfxLength);
        UniversalRenderPipelineUtils.SetRendererFeatureActive("SpeedLines", false);
        yield return new WaitForSeconds(_boostCooldown - _vfxLength);

        _playerMaterial.color = _boostableColor;
        _isBoosting = false;


    }

    private void StopBoostVFX()
    {
        // Stopping Boost Effect if it is happening
        StopCoroutine(BoostControl());
        // Deactivating Render Feature
        UniversalRenderPipelineUtils.SetRendererFeatureActive("SpeedLines", false);
        // Letting player immediatly boost again
        _playerMaterial.color = _boostableColor;
        _isBoosting = false;
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    public void OnEventHappen(GameOverSignal e)
    {
        StopBoostVFX();
    }

    public void OnEventHappen(PlayerDiedSignal e)
    {
        StopBoostVFX();
    }
}