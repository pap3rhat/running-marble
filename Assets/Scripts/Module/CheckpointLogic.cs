using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointLogic : MonoBehaviour
{
    // Connection to Manager
    private GameManager _gameManager;

    // Mirror Effect
    [SerializeField] private Camera _mirrorCam;
    private RenderTexture _renderTarget;
    private Renderer _renderer;
    private MaterialPropertyBlock _materialPropertyBlock;
    [SerializeField] private Vector2 _renderTextureSize = new Vector2(1141, 940);

    private string _playerObjName = "Player";

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Start()
    {
        _gameManager = GameManager.Instance;
        SetUpCamera();
    }

    private void OnDestroy()
    {
        // Cleaning up
        if (_renderTarget) _renderTarget.Release();
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Contains(_playerObjName))
        {
            _gameManager.CheckpointReached();
        }
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /*
     * Set all properties that are necessary for camera to behave like a mirror.
     */
    private void SetUpCamera()
    {
        // Getting components
        _renderer = GetComponentInParent<Renderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();

        // Setting _renderTarget to be default 
        _renderer.material.SetTexture("_renderTarget", _renderer.material.mainTexture);

        // Create render target
        _renderTarget = new RenderTexture((int)_renderTextureSize.x, (int)_renderTextureSize.y, 16, RenderTextureFormat.ARGB32);
        _renderTarget.Create();

        // Apply texture to shader
        _renderer.GetPropertyBlock(_materialPropertyBlock);
        _materialPropertyBlock.SetTexture("_renderTarget", _renderTarget);
        _renderer.SetPropertyBlock(_materialPropertyBlock);

        // Init Camera
        _mirrorCam.targetTexture = _renderTarget;
    }
}
