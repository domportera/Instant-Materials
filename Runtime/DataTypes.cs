using UnityEngine;

namespace InstantMaterials
{
    public enum RenderPipeline { Default, URP, HDRP, Unknown }

    public struct ShaderData
    {
        internal ShaderData(string litName, string unlitName, string unlitTransparentName, string colorID, string unlitColorID, string textureID)
        {
            LitShader = litName;
            UnlitTransparencyShader = unlitTransparentName;
            UnlitShader = unlitName;

            this.ColorID = Shader.PropertyToID(colorID);
            this.UnlitColorID = Shader.PropertyToID(unlitColorID);
            this.TextureID = Shader.PropertyToID(textureID);
        }

        public readonly int ColorID, UnlitColorID, TextureID;
        public readonly string LitShader, UnlitTransparencyShader, UnlitShader;
    }

    internal readonly struct RuntimeMaterialData
    {
        internal readonly int ColorID, TextureID;
        internal readonly bool Lit, Transparent, IsTransparentUnlitDefault;
        internal readonly MaterialConfig MaterialConfig;

        internal RuntimeMaterialData(in MaterialConfig config, RenderPipeline pipeline)
        {
            MaterialConfig = config;
            Lit = config.Lit;
            Transparent = config.Transparent;

            IsTransparentUnlitDefault = pipeline == RenderPipeline.Default && Transparent && !Lit;

            var shaderData = RenderPipelineInformation.ShaderData;
            ColorID = config.Lit ? shaderData[pipeline].ColorID : shaderData[pipeline].UnlitColorID;
            TextureID = shaderData[pipeline].TextureID;
        }
    }

    internal readonly struct MaterialConfig
    {
        internal readonly bool Transparent, Lit, GPUInstancing;
        internal readonly HideFlags HideFlags;

        internal MaterialConfig(bool lit, bool transparent, bool gpuInstancing, HideFlags hideFlags)
        {
            Lit = lit;
            Transparent = transparent;
            GPUInstancing = gpuInstancing;
            HideFlags = hideFlags;
        }

        public override int GetHashCode()
        {
            return (GPUInstancing ? 1 : 0)
                | (Transparent ? 1 << 1 : 0)
                | (Lit ? 1 << 2 : 0)
                | (HideFlags.GetHashCode() << 3);
        }
    }
}