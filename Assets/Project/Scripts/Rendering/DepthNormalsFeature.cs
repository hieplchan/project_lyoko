using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace StartledSeal.Rendering
{
    public class DepthNormalsFeature : ScriptableRendererFeature
    {
        private class DepthNormalsPass : ScriptableRenderPass
        {
            private const int DepthBufferBits = 32;
            private const string ProfilerTag = "DepthNormalsPrepass";

            private Material _material;
            private ShaderTagId _shaderTagId;
            private RenderTextureDescriptor _descriptor;
            private RTHandle _target;
            private FilteringSettings _filteringSettings;
            
            public DepthNormalsPass(RenderQueueRange renderQueueRange, RenderPassEvent renderPassEvent, LayerMask layerMask, Material material)
            {
                this.renderPassEvent = renderPassEvent;
                
                _material = material;
                _filteringSettings = new FilteringSettings(renderQueueRange, layerMask);
                _shaderTagId = new ShaderTagId("DepthOnly");
            }
            
            public void Setup(RenderTextureDescriptor cameraDataCameraTargetDescriptor, RTHandle depthNormalsTexture)
            {
                cameraDataCameraTargetDescriptor.colorFormat = RenderTextureFormat.ARGB32;
                cameraDataCameraTargetDescriptor.depthBufferBits = DepthBufferBits;
                _descriptor = cameraDataCameraTargetDescriptor;
                
                _target = depthNormalsTexture;
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                cmd.GetTemporaryRT(Shader.PropertyToID(_target.name), _descriptor, FilterMode.Point);
                ConfigureTarget(_target);
                ConfigureClear(ClearFlag.All, Color.black);
                
                // TODO: Split to anothe pass later
                ConfigureInput(ScriptableRenderPassInput.Normal);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer cmd = CommandBufferPool.Get(ProfilerTag);

                using (new ProfilingScope(cmd, new ProfilingSampler(ProfilerTag)))
                {
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();

                    var drawingSettings = CreateDrawingSettings(
                        _shaderTagId, ref renderingData,
                        renderingData.cameraData.defaultOpaqueSortFlags);
                    drawingSettings.overrideMaterial = _material;

                    context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings);
                    cmd.SetGlobalTexture("_CameraDepthNormalsTexture", Shader.PropertyToID(_target.name));
                }
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public override void OnCameraCleanup(CommandBuffer cmd)
            {
                cmd.ReleaseTemporaryRT(Shader.PropertyToID(_target.name));
            }
        }

        private DepthNormalsPass _depthNormalsPass;
        private RTHandle _depthNormalsTexture;
        private Material _depthNormalsMaterial;
        
        public override void Create()
        {
            _depthNormalsMaterial = CoreUtils.CreateEngineMaterial("Hidden/Internal-DepthNormalsTexture");
            _depthNormalsPass = new DepthNormalsPass(
                RenderQueueRange.opaque, 
                RenderPassEvent.AfterRenderingPrePasses,
                -1, 
                _depthNormalsMaterial);
            _depthNormalsTexture = RTHandles.Alloc("_CameraDepthNormalsTexture", name: "_CameraDepthNormalsTexture");
        }

        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            _depthNormalsPass.Setup(renderingData.cameraData.cameraTargetDescriptor, _depthNormalsTexture);
            renderer.EnqueuePass(_depthNormalsPass);
        }
    }
}