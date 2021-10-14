using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlitMaterialFeature : ScriptableRendererFeature
{

    private class RenderPass : ScriptableRenderPass
    {
        private string profilingName;
        private Material material;
        private int materialPassIndex;
        private RenderTargetIdentifier sourceID;
        private RenderTargetHandle tempTextureHandle;

        public RenderPass(string name, Material material, int materialPassIndex)
        {
            this.profilingName = name;
            this.material = material;
            this.materialPassIndex = materialPassIndex;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(profilingName);

            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;

            cmd.GetTemporaryRT(tempTextureHandle.id, desc, FilterMode.Bilinear);
            Blit(cmd, sourceID, tempTextureHandle.Identifier(), material, materialPassIndex);
            Blit(cmd, tempTextureHandle.Identifier(), sourceID);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempTextureHandle.id);
        }

        public void SetSource(RenderTargetIdentifier source)
        {
            this.sourceID = source;
        }
    }
    [System.Serializable]
    public class Settings
    {
        public Material material;
        // -1 means render all passes
        // 0 shader graph code
        // 1 shadow caster pass
        // 2 depth render pass
        public int materialPassIndex = -1; 
        public RenderPassEvent renderEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    [SerializeField]
    private Settings settings = new Settings();
    private RenderPass renderPass;

    public Material Material
    {
        get => settings.material;
    }
    /// <inheritdoc/>
    public override void Create()
    {
        renderPass = new RenderPass(name, settings.material, settings.materialPassIndex);
        renderPass.renderPassEvent = settings.renderEvent;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderPass.SetSource(renderer.cameraColorTarget);
        renderer.EnqueuePass(renderPass);
    }
}


