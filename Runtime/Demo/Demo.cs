using System.Collections;
using UnityEngine;
using InstantMaterials;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InstantMaterials
{
    internal class Demo : MonoBehaviour
    {
        [Tooltip("Number of objects to be spawned")]
        [SerializeField] int numObjects = 50;

        [Tooltip("How long it takes for all the objects to spawn")]
        [SerializeField] float spawnDuration = 7f;

        [Tooltip("How spread out the spawned objects are")]
        [SerializeField] float spreadFactor = 10f;

        List<Transform> objectTransforms = new List<Transform>();

        void Start()
        {
            StartCoroutine(SpawnObjects());
            StartCoroutine(LineRenderer());
        }

        IEnumerator SpawnObjects()
        {
            float spawnInterval = spawnDuration / numObjects;
            for (int i = 0; i < numObjects; i++)
            {
                //spawn our primitive
                PrimitiveType primType = (PrimitiveType)(i % 4);
                GameObject obj = GameObject.CreatePrimitive(primType);
                obj.transform.position = UnityEngine.Random.insideUnitSphere * spreadFactor;

                //get our renderer
                MeshRenderer rend = obj.GetComponent<MeshRenderer>();

                //just shuffling up some variables
                bool lit = i % 2 == 0;
                bool transparent = i % 10 < 6;

                rend.material = MaterialInstancer.ApplyMaterialTo(rend, Color.clear);

                obj.name = $"{primType} {(transparent ? "Transparent" : "Opaque")} {(lit ? "Lit" : "Unlit")}";
                obj.AddComponent<DemoObject>();

                objectTransforms.Add(obj.transform);

                yield return new WaitForSeconds(spawnInterval);
            }
        }

        IEnumerator LineRenderer()
        {
            LineRenderer line = gameObject.AddComponent<LineRenderer>();
            _ = MaterialInstancer.ApplyMaterialTo(line, new Color(0.6f, 0f, 0.9f, 0.38f));

            line.startWidth = 0.1f;
            line.endWidth = 0.1f;

            while (true)
            {
                Vector3[] positions = objectTransforms.Select(x => x.position).ToArray();
                line.positionCount = objectTransforms.Count;
                line.SetPositions(positions);
                yield return null;
            }
        }
    }
}
