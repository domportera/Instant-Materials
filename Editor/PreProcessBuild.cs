#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;

namespace InstantMaterials.Editor
{
    public class PreProcessBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }

        // big thanks to https://forum.unity.com/threads/modify-always-included-shaders-with-pre-processor.509479/#post-3509413
        public void OnPreprocessBuild(BuildReport report)
        {
            // get graphics settings AlwaysIncludedShaders array
            try
            {
                var graphicsSettingsObj =
                    AssetDatabase.LoadAssetAtPath<GraphicsSettings>("ProjectSettings/GraphicsSettings.asset");
                var serializedObject = new SerializedObject(graphicsSettingsObj);
                var alwaysIncludedShadersProperty = serializedObject.FindProperty("m_AlwaysIncludedShaders");

                AddAlwaysIncludedShader(MaterialInstancer.ShaderName, alwaysIncludedShadersProperty);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        static void AddAlwaysIncludedShader(string shaderName, SerializedProperty alwaysIncludedShaders)
        {
            var shader = Shader.Find(shaderName);
            if (shader == null)
                return;

            // check if we already have the requested shader
            bool hasShader = false;
            for (int i = 0; i < alwaysIncludedShaders.arraySize; ++i)
            {
                var element = alwaysIncludedShaders.GetArrayElementAtIndex(i);
                if (shader != element.objectReferenceValue)
                    continue;
                
                hasShader = true;
                break;
            }

            if (hasShader)
                return;
            
            // add the shader to the array
            int arrayIndex = alwaysIncludedShaders.arraySize;
            alwaysIncludedShaders.InsertArrayElementAtIndex(arrayIndex);
            var arrayElem = alwaysIncludedShaders.GetArrayElementAtIndex(arrayIndex);
            arrayElem.objectReferenceValue = shader;
            
            alwaysIncludedShaders.serializedObject.ApplyModifiedProperties();
            
            Debug.Log($"Instant Materials: Added shader {shaderName} to Always Included Shaders. New total count: {alwaysIncludedShaders.arraySize}");

            AssetDatabase.SaveAssets();
        }
    }
}
#endif