using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class SpeedLinesFeature : ScriptableRendererFeature
{ // class to nicely display possible settings for shader within inspector
    [System.Serializable]
    public class Settings
    {
        [Header("Speed Lines Settings")]
        [SerializeField, Range(0,1)] public float MaskSize = 0.6f;
        [SerializeField, Range(0, 1)] public float MaskContrast = 0.8f;
    }

    // customizable settings in inspector
    [SerializeField] private Settings settings;
    [SerializeField] private Shader m_speedLines;

    // private variables -- outline
    private Material m_speedLinesMaterial;
    private SpeedLinesPass m_speedLinesPass;


    /* -------------------------------------------------------------------------------------------------------------------------------------------------------------- */

    public override void Create()
    {
        // --- creating materials for shader ---
        if (m_speedLinesMaterial != null) CoreUtils.Destroy(m_speedLinesMaterial);
        m_speedLinesMaterial = CoreUtils.CreateEngineMaterial(m_speedLines);

        name = "Speed Lines";

        // -- creating and configuring passes ---
        m_speedLinesPass = new SpeedLinesPass(m_speedLinesMaterial, settings, name);
        m_speedLinesPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_speedLinesPass);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            m_speedLinesPass.ConfigureInput(ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal);
            m_speedLinesPass.SetUpCameraTargets(renderer.cameraColorTargetHandle, renderer.cameraDepthTargetHandle);
        }

    }

    protected override void Dispose(bool disposing)
    {
        // destroy materials
        CoreUtils.Destroy(m_speedLinesMaterial);

        // dispose of everything passes used
        m_speedLinesPass.Dispose();
    }

}
