using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class NewOutlineFeature : ScriptableRendererFeature
{
    // class to nicely display possible settings for shader within inspector
    [System.Serializable]
    public class Settings
    {
        [Header("Outline Settings")]
        [SerializeField] public Color OutlineColor = Color.black;
        [SerializeField, Range(0.5f, 10)] public float OutlineThickness = 0.5f;
        [SerializeField, Range(-0.01f, 0.01f)] public float samplingNoiseStrength = 0.001f;
        [SerializeField, Range(0, 1000)] public float samplingNoiseSmoothness = 100;
        [SerializeField, Range(0.05f, 10)] public float colorThreshold = 0.05f;
        [SerializeField, Range(0.05f, 10)] public float depthThreshold = 0.05f;
    }

    // customizable settings in inspector
    [SerializeField] private Settings settings;
    [SerializeField] private Shader m_vertexColorEdges;

    // private variables -- outline
    private Material m_vertexColorEdgesMaterial;
    private NewOutlinePass m_newOutlinePass;


    /* -------------------------------------------------------------------------------------------------------------------------------------------------------------- */

    public override void Create()
    {
        // --- creating materials for shader ---
        if (m_vertexColorEdgesMaterial != null) CoreUtils.Destroy(m_vertexColorEdgesMaterial);
        m_vertexColorEdgesMaterial = CoreUtils.CreateEngineMaterial(m_vertexColorEdges);

        name = "Outline";

        // -- creating and configuring passes ---
        m_newOutlinePass = new NewOutlinePass(m_vertexColorEdgesMaterial, settings, name);
        m_newOutlinePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_newOutlinePass);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            m_newOutlinePass.ConfigureInput(ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal);
            m_newOutlinePass.SetUpCameraTargets(renderer.cameraColorTargetHandle, renderer.cameraDepthTargetHandle);
        }

    }

    protected override void Dispose(bool disposing)
    {
        // destroy materials
        CoreUtils.Destroy(m_vertexColorEdgesMaterial);

        // dispose of everything passes used
        m_newOutlinePass.Dispose();
    }
}


