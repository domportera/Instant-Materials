﻿using UnityEngine;

namespace InstantMaterials
{
    public static class MaterialInstancer
    {
        public const string ShaderName = "Instant/BasicUnlit";
        public static readonly Shader DefaultShader = Shader.Find(ShaderName);
        public static Material GetNewMaterialNonInstanced() => new(DefaultShader);
        
        private static bool _createdDefaultMaterial;
        private static Material _defaultMaterial;
        public static Material ApplyMaterialTo(Renderer renderer, Color color)
        {
            CreateDefaultMaterialIfNeeded();
            
            renderer.sharedMaterial = _defaultMaterial;
            var instanced = renderer.material;
            instanced.color = color;
            return instanced;
        }

        private static void CreateDefaultMaterialIfNeeded()
        {
            if (!_createdDefaultMaterial)
            {
                _defaultMaterial = new(DefaultShader);
                _createdDefaultMaterial = true;
            }
        }

        public static Material ApplyMaterialTo(GameObject gameObject, Color color, bool applyToChildren = false)
        {
            var renderer = gameObject.GetComponent<Renderer>();
            if(renderer == null)
                throw new MissingComponentException($"GameObject {gameObject.name} does not have a renderer component!");
            
            if(!applyToChildren)
                return ApplyMaterialTo(renderer, color);
            
            CreateDefaultMaterialIfNeeded();
            var children = gameObject.GetComponentsInChildren<Renderer>();
            renderer.sharedMaterial = _defaultMaterial;
            var instanced = renderer.material;
            foreach (var child in children)
            {
                child.sharedMaterial = instanced;
                child.material.color = color;
            }
            
            return instanced;
        }

        /// <summary>
        ///  here for legacy convenience and example - uses material.color
        /// </summary>
        /// <param name="material">A material instance applied by this class</param>
        /// <param name="newColor">The color to set</param>
        public static void SetColor(Material material, Color newColor)
        {
            material.color = newColor;
        }
    }
}