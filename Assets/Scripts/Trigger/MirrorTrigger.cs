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

    // Effect
    [SerializeField] private bool _effectOn;
    [SerializeField] private Shader _shader;
    private Material _material;

    // Render camera
    private RenderTexture _renderTarget;
    private Texture _defaultTexture;
    private Renderer _renderer;
    [SerializeField] private Vector2 _renderTextureSize = new Vector2(1800, 1000);

    private bool _isActive;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Start()
    {
        _defaultTexture = GetComponentInParent<Renderer>().material.mainTexture;
        _renderer = GetComponentInParent<Renderer>();

        // Init Camera
        _mirrorCamObj.SetActive(false);


        //// Init effect
        if (_material != null) CoreUtils.Destroy(_material);
        _material = CoreUtils.CreateEngineMaterial(_shader);

        _isActive = false;

     //   if (_effectOn) RenderPipelineManager.endContextRendering += OnEndContextRendering;
  
    }

    void OnEndContextRendering(ScriptableRenderContext context, List<Camera> cameras)
    {
        if (!_isActive) return;

        // Applying effect
        RenderTexture tmp = new RenderTexture((int)_renderTextureSize.x, (int)_renderTextureSize.y, 16, RenderTextureFormat.ARGB32);
        _material.SetTexture("_renderTarget", _renderTarget);
        Graphics.Blit(_renderTarget, tmp, _material);
        Graphics.Blit(tmp, _renderTarget);
        tmp.Release();
    }


    private void Update()
    {
        // Only update if mirror is active
        if (!_isActive)
        {
            return;
        }


        //    //// Apply render target texture to material
        //    //_renderer.material.SetTexture("_BaseMap", _renderTarget);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    // Create render target
    //    _renderTarget = new RenderTexture((int)_renderTextureSize.x, (int)_renderTextureSize.y, 16, RenderTextureFormat.ARGB32);
    //    _renderTarget.Create();

    //    // Apply render target texture to material
    //    _renderer.material.SetTexture("_BaseMap", _renderTarget);

    //    //_renderer.material.SetTexture("_renderTarget", _renderTarget);

    //    // Set up camera
    //    _mirrorCam.targetTexture = _renderTarget;
    //    _mirrorCamObj.SetActive(true);

    //    // Set active
    //    _isActive = true;
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    // Apply defaul texture to material
    //    _renderer.material.SetTexture("_BaseMap", _defaultTexture);

    //    // Disable camera
    //    _mirrorCamObj.SetActive(false);

    //    // Clean up
    //    if (_renderTarget) _renderTarget.Release();

    //    // Set inactive
    //    _isActive = false;
    //}

    private void OnDestroy()
    {
        // Cleaning up
        if (_renderTarget) _renderTarget.Release();
        if (_material != null) CoreUtils.Destroy(_material);

        RenderPipelineManager.endContextRendering -= OnEndContextRendering;
    }
}
