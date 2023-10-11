using System.Collections.Generic;

namespace InstantMaterials
{
    public static class RenderPipelineInformation
    {
        static readonly ShaderData DefaultShaderData = new ShaderData(
               litName: "Standard",
               unlitName: "Unlit/Color",
               unlitTransparentName: "Unlit/Transparent",
               colorID: "_Color",
               unlitColorID: "_Color",
               textureID: "_MainTex");

        public static readonly IReadOnlyDictionary<RenderPipeline, ShaderData> ShaderData = new Dictionary<RenderPipeline, ShaderData>()
        {
            { RenderPipeline.Default, DefaultShaderData },
            { RenderPipeline.Unknown, DefaultShaderData },
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
            if (matType.Lit)
            {
                return ShaderData[pipeline].LitShader;
            }
            else
            {
                return matType.Transparent ? ShaderData[pipeline].UnlitTransparencyShader : ShaderData[pipeline].UnlitShader;
            }
        }

    }
}
