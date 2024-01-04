using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SpeedLinesPass : ScriptableRenderPass
{

    private Material m_speedLinesMaterial;
    private SpeedLinesFeature.Settings m_settings;

    private ProfilingSampler m_profilingSampler;
    private RTHandle m_cameraColorTarget, m_cameraDepthTarget, m_tmpSpeedLines;

    /* -------------------------------------------------------------------------------------------------------------------------------------------------------------- */

    public SpeedLinesPass(Material speedLinesMaterial, SpeedLinesFeature.Settings settings, string name)
    {
        m_speedLinesMaterial = speedLinesMaterial;
        m_settings = settings;
        m_profilingSampler = new ProfilingSampler(name);
    }

    /* -------------------------------------------------------------------------------------------------------------------------------------------------------------- */

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        RenderTextureDescriptor colorDesc = renderingData.cameraData.cameraTargetDescriptor;
        colorDesc.depthBufferBits = 0;

        RenderingUtils.ReAllocateIfNeeded(ref m_tmpSpeedLines, colorDesc, name: "m_tmpSpeedLines");
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        // only show in game
        var cameraData = renderingData.cameraData;
        if (cameraData.camera.cameraType != CameraType.Game)
            return;

        CommandBuffer cmd = CommandBufferPool.Get("Speed Lines Effect");
        using (new ProfilingScope(cmd, m_profilingSampler))
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            SpeedLines(cmd, m_cameraColorTarget);
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
    private void SpeedLines(CommandBuffer cmd, RTHandle source)
    {
        m_speedLinesMaterial.SetFloat("_Mask_Size", m_settings.MaskSize);
        m_speedLinesMaterial.SetFloat("_Mask_Contrast", m_settings.MaskContrast);

        Blitter.BlitCameraTexture(cmd, source, m_tmpSpeedLines, m_speedLinesMaterial, 0);
        cmd.SetGlobalTexture("_tmpSpeedLines", m_tmpSpeedLines);
        BlitToScreen(cmd, m_tmpSpeedLines);
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
        m_tmpSpeedLines?.Release();
    }
}
