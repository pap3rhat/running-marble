using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MirrorTrigger : MonoBehaviour
{
    // GameObjects 
    [SerializeField] private BoxCollider _colliderTrigger;
    [SerializeField] private Camera _mirrorCam;
    [SerializeField] private GameObject _mirrorCamObj;

    // Render camera
    private RenderTexture _renderTarget;
    private Texture _defaultTexture;
    private Renderer _renderer;
    private MaterialPropertyBlock _materialPropertyBlock;
    [SerializeField] private Vector2 _renderTextureSize = new Vector2(1141, 940);

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Awake()
    {
        SetUpCamera();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Activate camera
        _mirrorCamObj.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        // Apply defaul texture to material
        _renderer.material.SetTexture("_renderTarget", _defaultTexture);

        // Deactivate camera
        _mirrorCamObj.SetActive(false);
    }

    private void OnDestroy()
    {
        // Cleaning up
        if (_renderTarget) _renderTarget.Release();
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    /*
     * Set all properties that are necessary for camera to behave like a mirror.
     */
    private void SetUpCamera()
    {
        // Getting components
        _renderer = GetComponentInParent<Renderer>();
        _defaultTexture = _renderer.material.mainTexture;
        _materialPropertyBlock = new MaterialPropertyBlock();

        // Setting _renderTarget to be default 
        _renderer.material.SetTexture("_renderTarget", _defaultTexture);

        // Create render target
        _renderTarget = new RenderTexture((int)_renderTextureSize.x, (int)_renderTextureSize.y, 16, RenderTextureFormat.ARGB32);
        _renderTarget.Create();

        // Apply texture to shader
        _renderer.GetPropertyBlock(_materialPropertyBlock);
        _materialPropertyBlock.SetTexture("_renderTarget", _renderTarget);
        _renderer.SetPropertyBlock(_materialPropertyBlock);

        // Init Camera
        _mirrorCam.targetTexture = _renderTarget;
        // Inactive at beginning
        _mirrorCamObj.SetActive(false);
    }
}
