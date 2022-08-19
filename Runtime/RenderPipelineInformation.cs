using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstantMaterials
{
    internal static class RenderPipelineInformation
    {
        static readonly ShaderData defaultShaderData = new ShaderData(
               litName: "Standard",
               unlitName: "Unlit/Color",
               unlitTransparentName: "Unlit/Transparent",
               colorID: "_Color",
               unlitColorID: "_Color",
               textureID: "_MainTex");

        internal static readonly Dictionary<RenderPipeline, ShaderData> shaderData = new Dictionary<RenderPipeline, ShaderData>()
        {
            { RenderPipeline.Default, defaultShaderData },
            { RenderPipeline.Unknown, defaultShaderData },
            { RenderPipeline.URP, new ShaderData(
                colorID:                "_BaseColor",
                unlitColorID:           "_BaseColor",
                textureID:              "_BaseColorMap",
                litName:                "Universal Render Pipeline/Lit",
                unlitName:              "Universal Render Pipeline/Unlit",
                unlitTransparentName:   "Universal Render Pipeline/Unlit" )},

            { RenderPipeline.HDRP, new ShaderData(
                colorID:                "_BaseColor",
                unlitColorID:           "_UnlitColor",
                textureID:              "_BaseColorMap",
                litName:                "HDRP/Lit",
                unlitName:              "HDRP/Unlit",
                unlitTransparentName:   "HDRP/Unlit" )},

        };

        internal static string GetShaderName(MaterialConfig matType, RenderPipeline pipeline)
        {
            if (matType.lit)
            {
                return shaderData[pipeline].litShader;
            }
            else
            {
                return matType.transparent ? shaderData[pipeline].unlitTransparencyShader : shaderData[pipeline].unlitShader;
            }
        }

    }
}
