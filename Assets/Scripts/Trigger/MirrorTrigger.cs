using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorTrigger : MonoBehaviour
{
    [SerializeField] private BoxCollider _colliderTrigger;
    [SerializeField] private Camera _mirrorCam;
    [SerializeField] private GameObject _mirrorCamObj;

    private RenderTexture _renderTarget;
    private Texture _defaultTexture;
    private Renderer _renderer;

    /*--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Start()
    {
        _defaultTexture = GetComponentInParent<Renderer>().material.mainTexture;
        _renderer = GetComponentInParent<Renderer>();

        // Init Camera
        _mirrorCamObj.SetActive(false);


    }

    private void OnTriggerEnter(Collider other)
    {
        // Create render target
        _renderTarget = new RenderTexture(1800, 1000, 16, RenderTextureFormat.ARGB32);
        _renderTarget.Create();

        // Apply render target texture to material
        _renderer.material.SetTexture("_BaseMap", _renderTarget);
        //_renderer.material.SetTextureScale("_MainTex", new Vector2(1, -1));

        // Set up camera
        _mirrorCam.targetTexture = _renderTarget;
        _mirrorCamObj.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        // Apply defaul texture to material
        _renderer.material.SetTexture("_BaseMap", _defaultTexture);
        //_renderer.material.SetTextureScale("_MainTex", new Vector2(1, 1));

        // Disable camera
        _mirrorCamObj.SetActive(false);

        // Clean up
        if (_renderTarget)
        {
            _renderTarget.Release();
        }
    }

    private void OnDestroy()
    {
        if (_renderTarget)
        {
            _renderTarget.Release();
        }
    }
}
