using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace InstantMaterials
{
    public static class RenderPipelineUtilities
    {
        internal const string URP_PIPELINE_NAME = "UniversalRenderPipelineAsset";
        internal const string HDRP_PIPELINE_NAME = "HDRenderPipelineAsset";

        public static RenderPipeline GetCurrentPipeline()
        {
            var pipelineAsset = GraphicsSettings.renderPipelineAsset;

            if (pipelineAsset == null)
                return RenderPipeline.Default;
            
            Type pipelineType = pipelineAsset.GetType();

            switch (pipelineType.Name)
            {
                case URP_PIPELINE_NAME:
                    return RenderPipeline.URP;
                case HDRP_PIPELINE_NAME:
                    return RenderPipeline.HDRP;
                default:
                    Debug.LogError($"Render pipeline {pipelineType.Name} not supported. If this is custom, it is not currently compatible." +
                        $"If this is a native Unity pipeline, please report this as a bug or missing feature.");
                    return RenderPipeline.Unknown;
            }
        }
    }
}
