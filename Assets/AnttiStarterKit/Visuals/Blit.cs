using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace AnttiStarterKit.Visuals
{
    public class Blit : ScriptableRendererFeature {
 
        public class BlitPass : ScriptableRenderPass {
            public enum RenderTarget {
                Color,
                RenderTexture,
            }
 
            public Material blitMaterial = null;
            public int blitShaderPassIndex = 0;
            public FilterMode filterMode { get; set; }
 
            private RenderTargetIdentifier source { get; set; }
            private RenderTargetHandle destination { get; set; }
 
            RenderTargetHandle m_TemporaryColorTexture;
            string m_ProfilerTag;
         
            public BlitPass(RenderPassEvent renderPassEvent, Material blitMaterial, int blitShaderPassIndex, string tag) {
                this.renderPassEvent = renderPassEvent;
                this.blitMaterial = blitMaterial;
                this.blitShaderPassIndex = blitShaderPassIndex;
                m_ProfilerTag = tag;
                m_TemporaryColorTexture.Init("_TemporaryColorTexture");
            }
         
            public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination) {
                this.source = source;
                this.destination = destination;
            }
         
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
                CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
 
                RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
                opaqueDesc.depthBufferBits = 0;
 
                // Can't read and write to same color target, use a TemporaryRT
                if (destination == RenderTargetHandle.CameraTarget) {
                    cmd.GetTemporaryRT(m_TemporaryColorTexture.id, opaqueDesc, filterMode);
                    Blit(cmd, source, m_TemporaryColorTexture.Identifier(), blitMaterial, blitShaderPassIndex);
                    Blit(cmd, m_TemporaryColorTexture.Identifier(), source);
                } else {
                    Blit(cmd, source, destination.Identifier(), blitMaterial, blitShaderPassIndex);
                }
 
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
         
            public override void FrameCleanup(CommandBuffer cmd) {
                if (destination == RenderTargetHandle.CameraTarget)
                    cmd.ReleaseTemporaryRT(m_TemporaryColorTexture.id);
            }
        }
 
        [System.Serializable]
        public class BlitSettings {
            public RenderPassEvent Event = RenderPassEvent.AfterRenderingOpaques;
 
            public Material blitMaterial = null;
            public int blitMaterialPassIndex = -1;
            public Target destination = Target.Color;
            public string textureId = "_BlitPassTexture";
        }
 
        public enum Target {
            Color,
            Texture
        }
 
        public BlitSettings settings = new BlitSettings();
        RenderTargetHandle m_RenderTextureHandle;
 
        BlitPass blitPass;
 
        public override void Create() {
            var passIndex = settings.blitMaterial != null ? settings.blitMaterial.passCount - 1 : 1;
            settings.blitMaterialPassIndex = Mathf.Clamp(settings.blitMaterialPassIndex, -1, passIndex);
            blitPass = new BlitPass(settings.Event, settings.blitMaterial, settings.blitMaterialPassIndex, name);
            m_RenderTextureHandle.Init(settings.textureId);
        }
 
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            var src = renderer.cameraColorTarget;
            var dest = (settings.destination == Target.Color) ? RenderTargetHandle.CameraTarget : m_RenderTextureHandle;
 
            if (settings.blitMaterial == null) {
                Debug.LogWarningFormat("Missing Blit Material. {0} blit pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
                return;
            }
 
            blitPass.Setup(src, dest);
            renderer.EnqueuePass(blitPass);
        }
    }
}