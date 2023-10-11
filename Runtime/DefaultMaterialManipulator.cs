using System;
using UnityEngine;

namespace InstantMaterials
{
    public static class DefaultMaterialManipulator
    {
        const string SRP_SURFACE_TYPE_TRANSPARENT_KEYWORD = "_SURFACE_TYPE_TRANSPARENT";
        public static void SetDefaultShaderMaterialTransparent(Material mat, RenderPipeline pipeline)
        {
            switch (pipeline)
            {
                case RenderPipeline.Default:
                    SetMaterialTransparentDefaultRP(mat, pipeline);
                    break;
                case RenderPipeline.URP:
                    SetMaterialTransparentURP(mat);
                    break;
                case RenderPipeline.HDRP:
                    SetMaterialTransparentHDRP(mat);
                    break;
                case RenderPipeline.Unknown:
                default:
                    Debug.LogWarning($"Pipeline {pipeline} transparency not supporterd");
                    break;
            }
        }

        private static void SetMaterialTransparentDefaultRP(Material mat, RenderPipeline pipeline)
        {
            if(pipeline == RenderPipeline.Default)
            {
                mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.SetFloat("_DstBlend", 10);
                mat.SetFloat("_Zwrite", 0);
                mat.SetFloat("_Mode", 3);
                mat.renderQueue = 3000;
            }
            else
            {
                mat.EnableKeyword(SRP_SURFACE_TYPE_TRANSPARENT_KEYWORD);
            }
        }

        private static void SetMaterialTransparentHDRP(Material mat)
        {
            mat.EnableKeyword(SRP_SURFACE_TYPE_TRANSPARENT_KEYWORD);
            mat.SetFloat("_SurfaceType", 1);
            mat.SetFloat("_AlphaDstBlend", 10);
            mat.SetFloat("_DstBlend", 10);
            mat.SetFloat("_ZTestDepthEqualForOpaque", 4);
            mat.SetFloat("_Zwrite", 0);
            mat.renderQueue = 2000;
        }

        private static void SetMaterialTransparentURP(Material mat)
        {
            mat.EnableKeyword(SRP_SURFACE_TYPE_TRANSPARENT_KEYWORD);
            mat.SetFloat("_DstBlend", 10);
            mat.SetFloat("_SrcBlend", 5);
            mat.SetFloat("_Surface", 1);
            mat.SetFloat("_Zwrite", 0);
            mat.SetShaderPassEnabled("DepthOnly", false);
            mat.SetShaderPassEnabled("SHADOWCASTER", false);
            mat.renderQueue = 2000;
        }

        internal static void SetColor(Material mat, Color color, RuntimeMaterialData data)
        {
            if (data.IsTransparentUnlitDefault)
            {
                ApplyColorTransparentUnlitDefaultMaterial(mat, color, data);
                return;
            }

            int colorID = data.ColorID;
            mat.SetColor(colorID, color);
        }

        private static void ApplyColorTransparentUnlitDefaultMaterial(Material material, Color color, RuntimeMaterialData materialData)
        {
            Texture2D texture = (Texture2D)material.GetTexture(materialData.TextureID);

            if (texture == null)
            {
                texture = CreateAndApplyTexture(material, materialData, 1, 1);
            }

            SetTextureColor(color, texture);
        }

        private static void SetTextureColor(Color color, Texture2D texture)
        {
            Color[] colorArray = new Color[texture.width * texture.height];

            for (int i = 0; i < colorArray.Length; i++)
                colorArray[i] = color;

            texture.SetPixels(colorArray);
            texture.Apply();
        }

        private static Texture2D CreateAndApplyTexture(Material material, RuntimeMaterialData materialData, int xResolution, int yResolution)
        {
            Texture2D texture = new Texture2D(xResolution, yResolution);
            material.SetTexture(materialData.TextureID, texture);
            return texture;
        }

        internal static void SetTexture(Material material, Texture2D texture, RuntimeMaterialData materialData)
        {
            material.SetTexture(materialData.TextureID, texture);
        }
    }
}