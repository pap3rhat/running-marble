using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class PixelFeature : ScriptableRendererFeature
{ // class to nicely display possible settings for shader within inspector
    [System.Serializable]
    public class Settings
    {
        [Header("Pixel Settings")]
        [SerializeField, Range(1,16)] public float Resolution = 16;
    }

    // customizable settings in inspector
    [SerializeField] private Settings settings;
    [SerializeField] private Shader m_pixel;

    // private variables -- outline
    private Material m_pixelMaterial;
    private PixelPass m_pixelPass;


    /* -------------------------------------------------------------------------------------------------------------------------------------------------------------- */

    public override void Create()
    {
        // --- creating materials for shader ---
        if (m_pixelMaterial != null) CoreUtils.Destroy(m_pixelMaterial);
        m_pixelMaterial = CoreUtils.CreateEngineMaterial(m_pixel);

        name = "Pixel";

        // -- creating and configuring passes ---
        m_pixelPass = new PixelPass(m_pixelMaterial, settings, name);
        m_pixelPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_pixelPass);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            m_pixelPass.ConfigureInput(ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal);
            m_pixelPass.SetUpCameraTargets(renderer.cameraColorTargetHandle, renderer.cameraDepthTargetHandle);
        }

    }

    protected override void Dispose(bool disposing)
    {
        // destroy materials
        CoreUtils.Destroy(m_pixelMaterial);

        // dispose of everything passes used
        m_pixelPass.Dispose();
    }

}
