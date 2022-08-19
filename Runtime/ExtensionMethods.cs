using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstantMaterials
{
    public static class ExtensionMethods
    {
        public static Material InstantMaterial (this Renderer renderer)
        {
            return renderer.sharedMaterial;
        }
    }
}
