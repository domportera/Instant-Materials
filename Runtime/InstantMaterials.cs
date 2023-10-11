using System.Collections.Generic;
using UnityEngine;

namespace InstantMaterials
{
    public static class MaterialInstancer
    {
        static RenderPipeline _pipeline;
        static RenderPipeline Pipeline {
            get {
                if (_detectedPipeline)
                    return _pipeline;

                _pipeline = RenderPipelineUtilities.GetCurrentPipeline();
                _detectedPipeline = true;
                return _pipeline;
            }
        }
        static bool _detectedPipeline = false;

        static bool _cacheBlankMaterials = true;

        /// <summary>
        /// Use this to disable caching separate "blank" (fresh out the oven) materials. I recommend you keep this set to true.
        /// If this is disabled, instantiating another material could lead to unexpected effects, such as
        /// materials being instantiated with unwanted textures and colors.
        /// </summary>
        public static bool CacheBlankMaterials {
            get { return _cacheBlankMaterials; }
            set {
                _cacheBlankMaterials = value;

                if (value == false)
                    WipeMaterialTemplates();
            }
        }

        static readonly Dictionary<MaterialConfig, Material> MaterialTemplates = new Dictionary<MaterialConfig, Material>();
        static readonly Dictionary<Material, RuntimeMaterialData> InstantiatedMaterialData = new Dictionary<Material, RuntimeMaterialData>();

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
            InstantiatedMaterialData.Add(newMaterial, data);
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
            RuntimeMaterialData matInfo = InstantiatedMaterialData[material];
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
            RuntimeMaterialData matInfo = InstantiatedMaterialData[material];
            DefaultMaterialManipulator.SetTexture(material, texture, matInfo);
        }

        /// <summary>
        /// Returns color id of a previously instantiated material. This raise an exception on a material not instantiated by this class
        /// </summary>
        /// <param name="material">A material instantiated by this class</param>
        /// <returns>Color id for use with a shader</returns>
        public static int GetColorID(Material material)
        {
            return InstantiatedMaterialData[material].ColorID;
        }

        /// <summary>
        /// Returns texture id of a previously instantiated material. This will raise an exception on a material not instantiated by this class
        /// </summary>
        /// <param name="material">A material instantiated by this class</param>
        /// <returns>Texture id for use with a shader</returns>
        public static int GetTextureID(Material material)
        {
            return InstantiatedMaterialData[material].TextureID;
        }

        /// <summary>
        /// Use this to destroy all cached template materials.
        /// Best used in scenarios where you know you're not going to need to instantiate any more, but highly unlikely to be necessary.
        /// This is called automatically when CacheBlankMaterials is set to false.
        /// </summary>
        public static void WipeMaterialTemplates()
        {
            IEnumerable<Material> materials = MaterialTemplates.Values;

            foreach(Material mat in materials)
            {
                GameObject.Destroy(mat);
            }

            MaterialTemplates.Clear();
        }

        private static Material GetMaterialTemplate(MaterialConfig matConfig)
        {
            Material material;
            bool success = MaterialTemplates.TryGetValue(matConfig, out material);

            if (success)
                return material;
            
            string shaderName = RenderPipelineInformation.GetShaderName(matConfig, Pipeline);

            Shader shader = Shader.Find(shaderName);

            if (shader == null)
            {
                Debug.LogError($"Instant Materials: Could not find shader {shaderName} for pipeline {Pipeline}");
                return null;
            }
            
            material = new Material(shader);

            if (matConfig.Transparent)
            {
                DefaultMaterialManipulator.SetDefaultShaderMaterialTransparent(material, Pipeline);
            }

            material.enableInstancing = matConfig.GPUInstancing;

            CacheMaterial(matConfig, material);

            return material;
        }

        private static void CacheMaterial(MaterialConfig materialConfig, Material material)
        {
            if (_cacheBlankMaterials)
            {
                MaterialTemplates[materialConfig] = Object.Instantiate(material);
            }
            else
            {
                MaterialTemplates[materialConfig] = material;
            }
        }
    }
}