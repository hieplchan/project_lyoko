using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace StartledSeal.Rendering
{
    public class CustomScreenSpaceOutlines : ScriptableRendererFeature
    {
        [SerializeField] private RenderPassEvent _renderPassEvent;
        [SerializeField] private LayerMask _layerMask;
        
        [SerializeField] private Shader _customScreenSpaceOutlineShader;

        private CustomScreenSpaceOutlinePass _customScreenSpaceOutlinePass;
        
        public override void Create()
        {
            _customScreenSpaceOutlinePass = new CustomScreenSpaceOutlinePass(
                _renderPassEvent,
                _customScreenSpaceOutlineShader,
                _layerMask);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_customScreenSpaceOutlinePass);
        }
    }
    
    public class CustomScreenSpaceOutlinePass : ScriptableRenderPass
    {
        private Material _material;
        private FilteringSettings _filteringSettings;
        private readonly List<ShaderTagId> _shaderTagIdList;

        public CustomScreenSpaceOutlinePass(
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
            
            CommandBuffer cmd = CommandBufferPool.Get();
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