using System;
using System.Collections.Generic;
using UnityEngine;

namespace InstantMaterials
{
    public enum RenderPipeline { Default, URP, HDRP, Unknown }

    internal struct ShaderData
    {
        internal ShaderData(string litName, string unlitName, string unlitTransparentName, string colorID, string unlitColorID, string textureID)
        {
            litShader = litName;
            unlitTransparencyShader = unlitTransparentName;
            unlitShader = unlitName;

            this.colorID = Shader.PropertyToID(colorID);
            this.unlitColorID = Shader.PropertyToID(unlitColorID);
            this.textureID = Shader.PropertyToID(textureID);
        }

        internal readonly int colorID, unlitColorID, textureID;
        internal readonly string litShader, unlitTransparencyShader, unlitShader;
    }

    internal readonly struct RuntimeMaterialData
    {
        internal readonly int colorID, textureID;
        internal readonly bool lit, transparent, isTransparentUnlitDefault;
        internal readonly MaterialConfig materialConfig;

        internal RuntimeMaterialData(MaterialConfig config, RenderPipeline pipeline)
        {
            materialConfig = config;
            lit = config.lit;
            transparent = config.transparent;

            isTransparentUnlitDefault = pipeline == RenderPipeline.Default && transparent && !lit;

            Dictionary<RenderPipeline, ShaderData> shaderData = RenderPipelineInformation.shaderData;
            colorID = config.lit ? shaderData[pipeline].colorID : shaderData[pipeline].unlitColorID;
            textureID = shaderData[pipeline].textureID;
        }
    }

    internal readonly struct MaterialConfig
    {
        internal readonly bool transparent, lit, gpuInstancing;
        internal readonly HideFlags hideFlags;

        internal MaterialConfig(bool lit, bool transparent, bool gpuInstancing, HideFlags hideFlags)
        {
            this.lit = lit;
            this.transparent = transparent;
            this.gpuInstancing = gpuInstancing;
            this.hideFlags = hideFlags;
        }

        public override int GetHashCode()
        {
            return (gpuInstancing ? 1 : 0)
                | (transparent ? 1 << 1 : 0)
                | (lit ? 1 << 2 : 0)
                | (hideFlags.GetHashCode() << 3);
        }
    }
}