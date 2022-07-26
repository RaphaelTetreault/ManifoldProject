using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzPathPositioner : MonoBehaviour
    {
        [SerializeField] private GfzTrackSegmentNode segment;
        [SerializeField, Range(-0.5f, +0.5f)] private float horizontalOffset;
        [SerializeField] private Vector3 positionOffset;
        [SerializeField] private Vector3 rotationOffset;
        [SerializeField] private Vector3 scaleOffset = Vector3.one;
        [SerializeField] private GameObject prefab;
        [SerializeField, Min(1)] private int totalInstances = 1;
        [SerializeField, Min(1)] private float maxStep = 1;
        [SerializeField, Range(0f, 1f)] private float from = 0f;
        [SerializeField, Range(0f, 1f)] private float to = 1f;
        [SerializeField] private bool removeFirst;
        [SerializeField] private bool removeLast;

        public void CreateObjects()
        {
            DeleteObjects();

            var hacTRS = segment.CreateHierarchichalAnimationCurveTRS(false);
            float rangeMin = from * segment.GetMaxTime();
            float rangeMax = to * segment.GetMaxTime();
            float range = rangeMax - rangeMin;
            float maxStep = range / totalInstances;
            var matrices = TristripGenerator.GenerateMatrixIntervals(hacTRS, maxStep, rangeMin, rangeMax);

            Vector3 hOffset = horizontalOffset * Vector3.right;
            var offset = Matrix4x4.TRS(positionOffset, Quaternion.Euler(rotationOffset), scaleOffset);

            for (int i = 0; i < matrices.Length; i++)
            {
                var child = GameObject.Instantiate(prefab, transform);
                var matrix = matrices[i];
                var h = matrix.MultiplyPoint(hOffset);
                matrix = Matrix4x4.TRS(Vector3.zero, matrix.rotation, Vector3.one); // remove scale
                matrix = matrix * offset;
                child.transform.position = matrix.Position() + h;
                child.transform.rotation = matrix.rotation;
                child.transform.localScale = matrix.lossyScale;
            }
        }

        public void DeleteObjects()
        {
            foreach (var child in transform.GetChildren())
            {
                DestroyImmediate(child.gameObject);
            }
        }

        private void OnValidate()
        {
            if (segment == null)
                segment = GetComponentInParent<GfzTrackSegmentNode>();
        }
    }
}
