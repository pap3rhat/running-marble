using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class NewOutlinePass : ScriptableRenderPass
{
    private Material m_edgesMaterial;
    private NewOutlineFeature.Settings m_settings;

    private ProfilingSampler m_profilingSampler;
    private RTHandle m_cameraColorTarget, m_cameraDepthTarget, m_tmpEdges;

    /* -------------------------------------------------------------------------------------------------------------------------------------------------------------- */

    public NewOutlinePass(Material vertexColorEdgesMaterial, NewOutlineFeature.Settings settings, string name)
    {
        m_edgesMaterial = vertexColorEdgesMaterial;
        m_settings = settings;
        m_profilingSampler = new ProfilingSampler(name);
    }

    /* -------------------------------------------------------------------------------------------------------------------------------------------------------------- */

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        RenderTextureDescriptor colorDesc = renderingData.cameraData.cameraTargetDescriptor;
        colorDesc.depthBufferBits = 0;

        RenderingUtils.ReAllocateIfNeeded(ref m_tmpEdges, colorDesc, name: "m_tmpEdges");
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        // only show in game
        var cameraData = renderingData.cameraData;
        if (cameraData.camera.cameraType != CameraType.Game)
            return;

        CommandBuffer cmd = CommandBufferPool.Get("Outline Effect");
        using (new ProfilingScope(cmd, m_profilingSampler))
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            EdgeDetectionDepth(cmd, m_cameraColorTarget);
        }

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        m_cameraColorTarget = null;
        m_cameraDepthTarget = null;
    }

    // Sets up source texture of camera
    public void SetUpCameraTargets(RTHandle cameraColorTargetHandle, RTHandle cameraDepthTargetHandle)
    {
        m_cameraColorTarget = cameraColorTargetHandle;
        m_cameraDepthTarget = cameraDepthTargetHandle;
    }

    /* -------------------------------------------------------------------------------------------------------------------------------------------------------------- */

    // Detects edges based on vertex color
    private void EdgeDetectionDepth(CommandBuffer cmd, RTHandle source)
    {
        m_edgesMaterial.SetFloat("_OutlineThickness", m_settings.OutlineThickness);
        m_edgesMaterial.SetColor("_OutlineColor", m_settings.OutlineColor);
        m_edgesMaterial.SetFloat("_SamplingNoiseStrength", m_settings.samplingNoiseStrength);
        m_edgesMaterial.SetFloat("_SamplingNoiseSmoothness", m_settings.samplingNoiseSmoothness);
        m_edgesMaterial.SetFloat("_ColorThreshold", m_settings.colorThreshold);
        m_edgesMaterial.SetFloat("_DepthThreshold", m_settings.depthThreshold);

        Blitter.BlitCameraTexture(cmd, source, m_tmpEdges, m_edgesMaterial, 0);
        cmd.SetGlobalTexture("_tmpEdges", m_tmpEdges);
        BlitToScreen(cmd, m_tmpEdges);
    }

    // Blits given texture (source) to screen
    private void BlitToScreen(CommandBuffer cmd, RTHandle source)
    {
        Blitter.BlitCameraTexture(cmd, source, m_cameraColorTarget);
    }

    /* -------------------------------------------------------------------------------------------------------------------------------------------------------------- */

    // Disposes of all temporary RTHandles
    public void Dispose()
    {
        m_tmpEdges?.Release();
    }
}
