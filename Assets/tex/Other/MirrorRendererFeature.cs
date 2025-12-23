using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MirrorRendererFeature : ScriptableRendererFeature
{
    class MirrorRenderPass : ScriptableRenderPass
    {
        private RenderTargetHandle _reflectionTexture;
        private Material _mirrorMaterial;

        public MirrorRenderPass(Material mirrorMaterial)
        {
            _mirrorMaterial = mirrorMaterial;
            _reflectionTexture.Init("_ReflectionTex");
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(_reflectionTexture.id, cameraTextureDescriptor);
            ConfigureTarget(_reflectionTexture.Identifier());
            ConfigureClear(ClearFlag.All, Color.black);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("Mirror Reflection Pass");

            Blit(cmd, renderingData.cameraData.renderer.cameraColorTarget, _reflectionTexture.Identifier(), _mirrorMaterial);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(_reflectionTexture.id);
        }
    }

    private MirrorRenderPass _mirrorRenderPass;
    public Material mirrorMaterial;

    public override void Create()
    {
        _mirrorRenderPass = new MirrorRenderPass(mirrorMaterial)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_mirrorRenderPass);
    }
}