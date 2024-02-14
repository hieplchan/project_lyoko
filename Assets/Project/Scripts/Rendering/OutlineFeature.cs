using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace StartledSeal.Rendering
{
    public class OutlineFeature : ScriptableRendererFeature
    {
        [SerializeField] private RenderPassEvent _renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        [SerializeField] private Shader _shader;
        [SerializeField] private LayerMask _layerMask = -1;

        private OutlinePass _outlinePass;
        
        public override void Create()
        {
            _outlinePass = new OutlinePass(
                _renderPassEvent,
                _shader,
                _layerMask);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_outlinePass);
        }
    }
    
    public class OutlinePass : ScriptableRenderPass
    {
        private Material _material;
        private FilteringSettings _filteringSettings;
        private readonly List<ShaderTagId> _shaderTagIdList;

        public OutlinePass(
            RenderPassEvent renderPassEvent, 
            Shader customScreenSpaceOutlineShader, 
            LayerMask layerMask)
        {
            this.renderPassEvent = renderPassEvent;
            if (customScreenSpaceOutlineShader != null)
                _material = new Material(customScreenSpaceOutlineShader);
            _filteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);
            _shaderTagIdList = new List<ShaderTagId>{
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("LightWeightForward"),
                new ShaderTagId("SRPDefaultUnlit")
            };
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ConfigureTarget(renderingData.cameraData.renderer.cameraColorTargetHandle);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_material == null) return;
            
            CommandBuffer cmd = CommandBufferPool.Get("CustomScreenSpaceOutlines");
            using (new ProfilingScope(cmd, new ProfilingSampler("CustomScreenSpaceOutlines")))
            {
                DrawingSettings drawingSettings = CreateDrawingSettings(
                    _shaderTagIdList, ref renderingData,
                    renderingData.cameraData.defaultOpaqueSortFlags);
                drawingSettings.overrideMaterial = _material;
                
                context.DrawRenderers(renderingData.cullResults,
                    ref drawingSettings, ref _filteringSettings);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}