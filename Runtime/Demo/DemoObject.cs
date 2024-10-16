using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstantMaterials
{
    public class DemoObject : MonoBehaviour
    {
        #region Movement Variables
        Color originalColor;
        Vector3 startPosition;
        Vector3 movementRangeModifier;
        private const float COLOR_OSCILLATION_MAGNITUDE = 0.5f;
        float speedModifier;
        #endregion

        Material material;
        public bool oscillateColors = true;

        void Start()
        {
            originalColor = Random.ColorHSV();

            //this requires using the sharedMaterial property of the renderer, but I have this handy extension method for ease of remembering
            material = GetComponent<Renderer>().material; 

            movementRangeModifier = Random.insideUnitCircle * 2f;
            speedModifier = Random.Range(0.25f, 1.5f);

            startPosition = transform.position;
        }

        void Update()
        {
            float oscillator = Mathf.Sin(Time.time * speedModifier);
            Move(oscillator);

            if (!oscillateColors)
                return;

            Color newColor = GetNewColor(oscillator);
            MaterialInstancer.SetColor(material, newColor);
        }

        private Color GetNewColor(float oscillator)
        {
            float redModifier = Mathf.Clamp01(oscillator * movementRangeModifier.x * COLOR_OSCILLATION_MAGNITUDE);
            float greenModifier = Mathf.Clamp01(oscillator * movementRangeModifier.y * COLOR_OSCILLATION_MAGNITUDE);
            float blueModifier = Mathf.Clamp01(oscillator * movementRangeModifier.z * COLOR_OSCILLATION_MAGNITUDE);
            float alphaModifier = oscillator * 0.5f;

            Color newColor = new Color(originalColor.r + redModifier, originalColor.g + greenModifier, originalColor.b + blueModifier, 0.5f + alphaModifier );
            return newColor;
        }

        private void Move(float oscillator)
        {
            Vector3 movement = oscillator * movementRangeModifier;
            transform.position = new Vector3(startPosition.x + movement.x, startPosition.y + movement.y, startPosition.z + movement.z);
        }
    }
}
