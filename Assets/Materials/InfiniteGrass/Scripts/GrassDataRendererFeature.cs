using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class GrassDataRendererFeature : ScriptableRendererFeature
{
    [SerializeField] private LayerMask heightMapLayer;
    [SerializeField] private Material heightMapMat;
    [SerializeField] private ComputeShader computeShader;

    private GrassDataPass grassDataPass;

    public override void Create()
    {
        grassDataPass?.Cleanup();
        grassDataPass = new GrassDataPass(heightMapLayer, heightMapMat, computeShader);
        grassDataPass.renderPassEvent = RenderPassEvent.AfterRenderingPrePasses;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (grassDataPass == null)
            return;

        // Only run for the game camera (skip scene view / preview / reflection cameras)
        if (renderingData.cameraData.cameraType != CameraType.Game)
            return;

        renderer.EnqueuePass(grassDataPass);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && grassDataPass != null)
        {
            grassDataPass.Cleanup();
            grassDataPass = null;
        }
    }

    private class GrassDataPass : ScriptableRenderPass
    {
        private class PassData
        {
            public RendererListHandle heightList;
            public RendererListHandle maskList;
            public RendererListHandle colorList;
            public RendererListHandle slopeList;

            public TextureHandle heightTex;
            public TextureHandle heightDepthTex;
            public TextureHandle maskTex;
            public TextureHandle colorTex;
            public TextureHandle slopeTex;

            public Matrix4x4 viewMatrix;
            public Matrix4x4 projectionMatrix;
            public Matrix4x4 camViewMatrix;
            public Matrix4x4 camProjectionMatrix;
            public Matrix4x4 vpMatrix;

            public Bounds cameraBounds;
            public Vector2 centerPos;
            public Vector3 cameraPosition;

            public float spacing;
            public float fullDensityDistance;
            public float drawDistance;
            public float textureUpdateThreshold;

            public Vector2Int gridSize;
            public Vector2Int gridStartIndex;
        }

        private const int TextureSize = 2048;

        private readonly List<ShaderTagId> shaderTagsList = new List<ShaderTagId>();

        private RTHandle heightRT;
        private RTHandle heightDepthRT;
        private RTHandle maskRT;
        private RTHandle colorRT;
        private RTHandle slopeRT;

        private readonly LayerMask heightMapLayer;
        private readonly Material heightMapMat;
        private readonly ComputeShader computeShader;

        private ComputeBuffer grassPositionsBuffer;
        private int grassBufferCapacity;

        public GrassDataPass(LayerMask heightMapLayer, Material heightMapMat, ComputeShader computeShader)
        {
            this.heightMapLayer = heightMapLayer;
            this.heightMapMat = heightMapMat;
            this.computeShader = computeShader;

            shaderTagsList.Add(new ShaderTagId("SRPDefaultUnlit"));
            shaderTagsList.Add(new ShaderTagId("UniversalForward"));
            shaderTagsList.Add(new ShaderTagId("UniversalForwardOnly"));
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var grass = InfiniteGrassRenderer.instance;
            Camera mainCam = Camera.main;

            if (grass == null || heightMapMat == null || computeShader == null || mainCam == null)
                return;

            UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            UniversalLightData lightData = frameData.Get<UniversalLightData>();

            // ---------- Persistent resources ----------
            RenderingUtils.ReAllocateHandleIfNeeded(ref heightRT, new RenderTextureDescriptor(TextureSize, TextureSize, RenderTextureFormat.RGFloat, 0), FilterMode.Bilinear, name: "_GrassHeightRT");
            RenderingUtils.ReAllocateHandleIfNeeded(ref heightDepthRT, new RenderTextureDescriptor(TextureSize, TextureSize, RenderTextureFormat.RFloat, 32), FilterMode.Bilinear, name: "_GrassHeightDepthRT");
            RenderingUtils.ReAllocateHandleIfNeeded(ref maskRT, new RenderTextureDescriptor(TextureSize, TextureSize, RenderTextureFormat.RFloat, 0), FilterMode.Bilinear, name: "_GrassMaskRT");
            RenderingUtils.ReAllocateHandleIfNeeded(ref colorRT, new RenderTextureDescriptor(TextureSize, TextureSize, RenderTextureFormat.ARGBFloat, 0), FilterMode.Bilinear, name: "_GrassColorRT");
            RenderingUtils.ReAllocateHandleIfNeeded(ref slopeRT, new RenderTextureDescriptor(TextureSize, TextureSize, RenderTextureFormat.ARGBFloat, 0), FilterMode.Bilinear, name: "_GrassSlopeRT");

            int requiredCapacity = Mathf.Max(1, (int)(1000000 * grass.maxBufferCount));
            if (grassPositionsBuffer == null || grassBufferCapacity != requiredCapacity)
            {
                grassPositionsBuffer?.Release();
                grassPositionsBuffer = new ComputeBuffer(requiredCapacity, sizeof(float) * 3, ComputeBufferType.Append);
                grassBufferCapacity = requiredCapacity;
            }

            using (var builder = renderGraph.AddUnsafePass<PassData>("Grass Data Pass", out PassData data))
            {
                // ---------- CPU-side calculations ----------
                data.spacing = grass.spacing;
                data.fullDensityDistance = grass.fullDensityDistance;
                data.drawDistance = grass.drawDistance;
                data.textureUpdateThreshold = grass.textureUpdateThreshold;

                Bounds b = CalculateCameraBounds(mainCam, data.drawDistance);
                data.cameraBounds = b;

                // Step the ortho camera in increments so textures don't flicker
                data.centerPos = new Vector2(
                    Mathf.Floor(mainCam.transform.position.x / data.textureUpdateThreshold) * data.textureUpdateThreshold,
                    Mathf.Floor(mainCam.transform.position.z / data.textureUpdateThreshold) * data.textureUpdateThreshold);

                data.viewMatrix = Matrix4x4.TRS(
                    new Vector3(data.centerPos.x, b.max.y, data.centerPos.y),
                    Quaternion.LookRotation(-Vector3.up),
                    new Vector3(1, 1, -1)).inverse;

                float ext = data.drawDistance + data.textureUpdateThreshold;
                data.projectionMatrix = Matrix4x4.Ortho(-ext, ext, -ext, ext, 0, b.size.y);

                data.camViewMatrix = cameraData.camera.worldToCameraMatrix;
                data.camProjectionMatrix = cameraData.camera.projectionMatrix;

                data.cameraPosition = mainCam.transform.position;
                data.vpMatrix = mainCam.projectionMatrix * mainCam.worldToCameraMatrix;

                data.gridSize = new Vector2Int(
                    Mathf.CeilToInt(b.size.x / data.spacing),
                    Mathf.CeilToInt(b.size.z / data.spacing));
                data.gridStartIndex = new Vector2Int(
                    Mathf.FloorToInt(b.min.x / data.spacing),
                    Mathf.FloorToInt(b.min.z / data.spacing));

                heightMapMat.SetVector("_BoundsYMinMax", new Vector2(b.min.y, b.max.y));

                // ---------- Renderer lists ----------
                DrawingSettings heightDraw = RenderingUtils.CreateDrawingSettings(
                    shaderTagsList, renderingData, cameraData, lightData, cameraData.defaultOpaqueSortFlags);
                heightDraw.overrideMaterial = heightMapMat;
                data.heightList = renderGraph.CreateRendererList(new RendererListParams(
                    renderingData.cullResults, heightDraw, new FilteringSettings(RenderQueueRange.all, heightMapLayer)));

                data.maskList = CreateList(renderGraph, "GrassMask", renderingData, cameraData, lightData);
                data.colorList = CreateList(renderGraph, "GrassColor", renderingData, cameraData, lightData);
                data.slopeList = CreateList(renderGraph, "GrassSlope", renderingData, cameraData, lightData);

                builder.UseRendererList(data.heightList);
                builder.UseRendererList(data.maskList);
                builder.UseRendererList(data.colorList);
                builder.UseRendererList(data.slopeList);

                // ---------- Import persistent textures ----------
                data.heightTex = renderGraph.ImportTexture(heightRT);
                data.heightDepthTex = renderGraph.ImportTexture(heightDepthRT);
                data.maskTex = renderGraph.ImportTexture(maskRT);
                data.colorTex = renderGraph.ImportTexture(colorRT);
                data.slopeTex = renderGraph.ImportTexture(slopeRT);

                builder.UseTexture(data.heightTex, AccessFlags.Write);
                builder.UseTexture(data.heightDepthTex, AccessFlags.Write);
                builder.UseTexture(data.maskTex, AccessFlags.Write);
                builder.UseTexture(data.colorTex, AccessFlags.Write);
                builder.UseTexture(data.slopeTex, AccessFlags.Write);

                builder.AllowPassCulling(false);
                builder.AllowGlobalStateModification(true);

                builder.SetRenderFunc((PassData d, UnsafeGraphContext ctx) => ExecutePass(d, ctx));
            }
        }

        private static RendererListHandle CreateList(
            RenderGraph renderGraph,
            string passName,
            UniversalRenderingData renderingData,
            UniversalCameraData cameraData,
            UniversalLightData lightData)
        {
            DrawingSettings draw = RenderingUtils.CreateDrawingSettings(
                new ShaderTagId(passName), renderingData, cameraData, lightData, SortingCriteria.CommonTransparent);

            return renderGraph.CreateRendererList(new RendererListParams(
                renderingData.cullResults, draw, new FilteringSettings(RenderQueueRange.all)));
        }

        private void ExecutePass(PassData data, UnsafeGraphContext ctx)
        {
            CommandBuffer cmd = CommandBufferHelpers.GetNativeCommandBuffer(ctx.cmd);

            // --- Height map (rendered from above with swapped matrices) ---
            cmd.BeginSample("Grass Height Map RT");
            cmd.SetRenderTarget(heightRT, heightDepthRT);
            cmd.ClearRenderTarget(true, true, Color.black);
            cmd.SetViewProjectionMatrices(data.viewMatrix, data.projectionMatrix);
            ctx.cmd.DrawRendererList(data.heightList);
            cmd.EndSample("Grass Height Map RT");

            // --- Mask ---
            cmd.BeginSample("Grass Mask RT");
            cmd.SetRenderTarget(maskRT);
            cmd.ClearRenderTarget(true, true, new Color(0, 0, 0, 0));
            ctx.cmd.DrawRendererList(data.maskList);
            cmd.EndSample("Grass Mask RT");

            // --- Color ---
            cmd.BeginSample("Grass Color RT");
            cmd.SetRenderTarget(colorRT);
            cmd.ClearRenderTarget(true, true, new Color(0, 0, 0, 0));
            ctx.cmd.DrawRendererList(data.colorList);
            cmd.EndSample("Grass Color RT");

            // --- Slope ---
            cmd.BeginSample("Grass Slope RT");
            cmd.SetRenderTarget(slopeRT);
            cmd.ClearRenderTarget(true, true, new Color(0, 0, 0, 0));
            ctx.cmd.DrawRendererList(data.slopeList);
            cmd.EndSample("Grass Slope RT");

            cmd.SetGlobalTexture("_GrassColorRT", colorRT);
            cmd.SetGlobalTexture("_GrassSlopeRT", slopeRT);

            // Restore the real camera matrices
            cmd.SetViewProjectionMatrices(data.camViewMatrix, data.camProjectionMatrix);

            // --- Compute grass positions ---
            cmd.SetBufferCounterValue(grassPositionsBuffer, 0);

            cmd.SetComputeMatrixParam(computeShader, "_VPMatrix", data.vpMatrix);
            cmd.SetComputeFloatParam(computeShader, "_FullDensityDistance", data.fullDensityDistance);
            cmd.SetComputeVectorParam(computeShader, "_BoundsMin", data.cameraBounds.min);
            cmd.SetComputeVectorParam(computeShader, "_BoundsMax", data.cameraBounds.max);
            cmd.SetComputeVectorParam(computeShader, "_CameraPosition", data.cameraPosition);
            cmd.SetComputeVectorParam(computeShader, "_CenterPos", data.centerPos);
            cmd.SetComputeFloatParam(computeShader, "_DrawDistance", data.drawDistance);
            cmd.SetComputeFloatParam(computeShader, "_TextureUpdateThreshold", data.textureUpdateThreshold);
            cmd.SetComputeFloatParam(computeShader, "_Spacing", data.spacing);
            cmd.SetComputeVectorParam(computeShader, "_GridStartIndex", (Vector2)data.gridStartIndex);
            cmd.SetComputeVectorParam(computeShader, "_GridSize", (Vector2)data.gridSize);
            cmd.SetComputeBufferParam(computeShader, 0, "_GrassPositions", grassPositionsBuffer);
            cmd.SetComputeTextureParam(computeShader, 0, "_GrassHeightMapRT", heightRT);
            cmd.SetComputeTextureParam(computeShader, 0, "_GrassMaskMapRT", maskRT);

            cmd.DispatchCompute(
                computeShader, 0,
                Mathf.CeilToInt((float)data.gridSize.x / 8),
                Mathf.CeilToInt((float)data.gridSize.y / 8),
                1);

            cmd.SetGlobalBuffer("_GrassPositions", grassPositionsBuffer);

            var grass = InfiniteGrassRenderer.instance;
            if (grass != null)
            {
                // Copy the append counter into the indirect args buffer (instance count)
                cmd.CopyCounterValue(grassPositionsBuffer, grass.argsBuffer, 4);

                if (grass.previewVisibleGrassCount)
                    cmd.CopyCounterValue(grassPositionsBuffer, grass.tBuffer, 0);
            }
        }

        private static Bounds CalculateCameraBounds(Camera camera, float drawDistance)
        {
            Vector3 ntopLeft = camera.ViewportToWorldPoint(new Vector3(0, 1, camera.nearClipPlane));
            Vector3 ntopRight = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));
            Vector3 nbottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
            Vector3 nbottomRight = camera.ViewportToWorldPoint(new Vector3(1, 0, camera.nearClipPlane));

            Vector3 ftopLeft = camera.ViewportToWorldPoint(new Vector3(0, 1, drawDistance));
            Vector3 ftopRight = camera.ViewportToWorldPoint(new Vector3(1, 1, drawDistance));
            Vector3 fbottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, drawDistance));
            Vector3 fbottomRight = camera.ViewportToWorldPoint(new Vector3(1, 0, drawDistance));

            float[] xValues = { ftopLeft.x, ftopRight.x, ntopLeft.x, ntopRight.x, fbottomLeft.x, fbottomRight.x, nbottomLeft.x, nbottomRight.x };
            float[] yValues = { ftopLeft.y, ftopRight.y, ntopLeft.y, ntopRight.y, fbottomLeft.y, fbottomRight.y, nbottomLeft.y, nbottomRight.y };
            float[] zValues = { ftopLeft.z, ftopRight.z, ntopLeft.z, ntopRight.z, fbottomLeft.z, fbottomRight.z, nbottomLeft.z, nbottomRight.z };

            float startX = xValues.Max(), endX = xValues.Min();
            float startY = yValues.Max(), endY = yValues.Min();
            float startZ = zValues.Max(), endZ = zValues.Min();

            Vector3 center = new Vector3((startX + endX) / 2, (startY + endY) / 2, (startZ + endZ) / 2);
            Vector3 size = new Vector3(Mathf.Abs(startX - endX), Mathf.Abs(startY - endY), Mathf.Abs(startZ - endZ));

            Bounds bounds = new Bounds(center, size);
            bounds.Expand(1);
            return bounds;
        }

        public void Cleanup()
        {
            heightRT?.Release();
            heightDepthRT?.Release();
            maskRT?.Release();
            colorRT?.Release();
            slopeRT?.Release();

            heightRT = null;
            heightDepthRT = null;
            maskRT = null;
            colorRT = null;
            slopeRT = null;

            grassPositionsBuffer?.Release();
            grassPositionsBuffer = null;
            grassBufferCapacity = 0;
        }
    }
}