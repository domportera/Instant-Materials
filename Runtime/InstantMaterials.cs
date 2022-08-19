using System.Collections.Generic;
using UnityEngine;

namespace InstantMaterials
{
    public static class MaterialInstancer
    {
        static RenderPipeline pipeline;
        static RenderPipeline Pipeline {
            get {
                if (detectedPipeline)
                    return pipeline;

                pipeline = RenderPipelineUtilities.GetCurrentPipeline();
                detectedPipeline = true;
                return pipeline;
            }
        }
        static bool detectedPipeline = false;

        static bool cacheBlankMaterials = true;

        /// <summary>
        /// Use this to disable caching separate "blank" (fresh out the oven) materials. I recommend you keep this set to true.
        /// If this is disabled, instantiating another material could lead to unexpected effects, such as
        /// materials being instantiated with unwanted textures and colors.
        /// </summary>
        public static bool CacheBlankMaterials {
            get { return cacheBlankMaterials; }
            set {
                cacheBlankMaterials = value;

                if (value == false)
                    WipeMaterialTemplates();
            }
        }

        static Dictionary<MaterialConfig, Material> materialTemplates = new Dictionary<MaterialConfig, Material>();
        static Dictionary<Material, RuntimeMaterialData> instantiatedMaterialData = new Dictionary<Material, RuntimeMaterialData>();

        /// <summary>
        /// Get a new default material with specified parameters. Lit and transparent with gpu instancing disabled by default.
        /// </summary>
        /// <param name="lit">Is it lit (true) or unlit (false)?</param>
        /// <param name="transparent">Is it transparent (true) or non-transparent (false)?</param>
        /// <param name="gpuInstancing">Is it GPU instanced? Note that this being true will disable the SRP batcher for this material.</param>
        /// <param name="hideFlags">You can specify the hide flags for uses in the editor.</param>
        /// <returns></returns>
        public static Material GetNewMaterialInstance(bool lit = true, bool transparent = false, bool gpuInstancing = false, HideFlags hideFlags = HideFlags.None)
        {
            MaterialConfig materialConfig = new MaterialConfig(lit, transparent, gpuInstancing, hideFlags);
            Material material = GetMaterialTemplate(materialConfig);

            Material newMaterial = Object.Instantiate(material);
            RuntimeMaterialData data = new RuntimeMaterialData(materialConfig, Pipeline);
            instantiatedMaterialData.Add(newMaterial, data);
            newMaterial.hideFlags = hideFlags;

            return newMaterial;
        }

        /// <summary>
        /// Sets color of a previously instantiated material. This will raise an exception on a material not instantiated by this class.
        /// For materials that are both unlit and transparent in the Default renderer, this will override the color texture with a simple 1x1 texture.
        /// </summary>
        /// <param name="material">A material instantiated by this class</param>
        /// <param name="color">Desired color</param>
        public static void SetColor(Material material, Color color)
        {
            RuntimeMaterialData matInfo = instantiatedMaterialData[material];
            DefaultMaterialManipulator.SetColor(material, color, matInfo);
        }

        /// <summary>
        /// Sets texture of a previously instantiated material. This will raise an exception on a material not instantiated by this class.
        /// For materials that are both unlit and transparent in the Default renderer, this will override the color texture.
        /// </summary>
        /// <param name="material">A material instantiated by this class</param>
        /// <param name="color">Desired color</param>
        public static void SetTexture(Material material, Texture2D texture)
        {
            RuntimeMaterialData matInfo = instantiatedMaterialData[material];
            DefaultMaterialManipulator.SetTexture(material, texture, matInfo);
        }

        /// <summary>
        /// Returns color id of a previously instantiated material. This raise an exception on a material not instantiated by this class
        /// </summary>
        /// <param name="material">A material instantiated by this class</param>
        /// <returns>Color id for use with a shader</returns>
        public static int GetColorID(Material material)
        {
            return instantiatedMaterialData[material].colorID;
        }

        /// <summary>
        /// Returns texture id of a previously instantiated material. This will raise an exception on a material not instantiated by this class
        /// </summary>
        /// <param name="material">A material instantiated by this class</param>
        /// <returns>Texture id for use with a shader</returns>
        public static int GetTextureID(Material material)
        {
            return instantiatedMaterialData[material].textureID;
        }

        /// <summary>
        /// Use this to destroy all cached template materials.
        /// Best used in scenarios where you know you're not going to need to instantiate any more, but highly unlikely to be necessary.
        /// This is called automatically when CacheBlankMaterials is set to false.
        /// </summary>
        public static void WipeMaterialTemplates()
        {
            IEnumerable<Material> materials = materialTemplates.Values;

            foreach(Material mat in materials)
            {
                GameObject.Destroy(mat);
            }

            materialTemplates.Clear();
        }

        private static Material GetMaterialTemplate(MaterialConfig matConfig)
        {
            Material material;
            bool success = materialTemplates.TryGetValue(matConfig, out material);

            if (success)
                return material;
            
            string shaderName = RenderPipelineInformation.GetShaderName(matConfig, Pipeline);

            Shader shader = Shader.Find(shaderName);
            material = new Material(shader);

            if (matConfig.transparent)
            {
                DefaultMaterialManipulator.SetDefaultShaderMaterialTransparent(material, Pipeline);
            }

            material.enableInstancing = matConfig.gpuInstancing;

            CacheMaterial(matConfig, material);

            return material;
        }

        private static void CacheMaterial(MaterialConfig materialConfig, Material material)
        {
            if (cacheBlankMaterials)
            {
                materialTemplates[materialConfig] = Object.Instantiate(material);
            }
            else
            {
                materialTemplates[materialConfig] = material;
            }
        }
    }
}