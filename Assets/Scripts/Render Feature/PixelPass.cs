using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelPass : ScriptableRenderPass
{

    private Material m_pixelMaterial;
    private PixelFeature.Settings m_settings;

    private ProfilingSampler m_profilingSampler;
    private RTHandle m_cameraColorTarget, m_cameraDepthTarget, m_tmpPixel;

    /* -------------------------------------------------------------------------------------------------------------------------------------------------------------- */

    public PixelPass(Material pixelMaterial, PixelFeature.Settings settings, string name)
    {
        m_pixelMaterial = pixelMaterial;
        m_settings = settings;
        m_profilingSampler = new ProfilingSampler(name);
    }

    /* -------------------------------------------------------------------------------------------------------------------------------------------------------------- */

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        RenderTextureDescriptor colorDesc = renderingData.cameraData.cameraTargetDescriptor;
        colorDesc.depthBufferBits = 0;

        RenderingUtils.ReAllocateIfNeeded(ref m_tmpPixel, colorDesc, name: "m_tmpSpeedLines");
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        // only show in game
        var cameraData = renderingData.cameraData;
        if (cameraData.camera.cameraType != CameraType.Game)
            return;

        CommandBuffer cmd = CommandBufferPool.Get("Pixel Effect");
        using (new ProfilingScope(cmd, m_profilingSampler))
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            Pixel(cmd, m_cameraColorTarget);
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

    // Pixelate Screen
    private void Pixel(CommandBuffer cmd, RTHandle source)
    {
        m_pixelMaterial.SetFloat("_Resolution", m_settings.Resolution);

        Blitter.BlitCameraTexture(cmd, source, m_tmpPixel, m_pixelMaterial, 0);
        cmd.SetGlobalTexture("_tmpSpeedLines", m_tmpPixel);
        BlitToScreen(cmd, m_tmpPixel);
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
        m_tmpPixel?.Release();
    }
}
