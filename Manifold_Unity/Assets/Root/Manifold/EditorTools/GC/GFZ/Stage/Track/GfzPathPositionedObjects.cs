using System;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzPathPositionedObjects : GfzPathPositionedBase
    {
        [SerializeField, Range(-0.5f, +0.5f)] private float horizontaPosition;
        [SerializeField, Range(0f, 1f)] private float lengthPositionFrom = 0f;
        [SerializeField, Range(0f, 1f)] private float lengthPositionTo = 1f;
        [SerializeField, Min(1)] private int totalInstances = 1;
        [SerializeField] private bool removeFirst;
        [SerializeField] private bool removeLast;

        public override void CreateObjects()
        {
            DeleteObjects();

            var hacTRS = segment.GetRoot().CreateHierarchichalAnimationCurveTRS(false);
            float rangeMin = lengthPositionFrom * segment.GetMaxTime();
            float rangeMax = lengthPositionTo * segment.GetMaxTime();
            float range = rangeMax - rangeMin;
            float maxStep = range / totalInstances;
            var matrices = TristripGenerator.GenerateMatrixIntervals(hacTRS, maxStep, rangeMin, rangeMax);

            Vector3 hOffset = horizontaPosition * Vector3.right;
            var rOffset = Quaternion.Euler(rotationOffset);
            var pOffset = rOffset * positionOffset;
            var offset = Matrix4x4.TRS(pOffset, rOffset, scaleOffset);

            int startIndex = removeFirst ? 1 : 0;
            int endIndex = removeLast ? matrices.Length - 2 : matrices.Length - 1;
            for (int i = startIndex; i < endIndex; i++)
            {
                var child = GameObject.Instantiate(prefab, transform);
                child.SetActive(true);
                var matrix = matrices[i];
                var h = matrix.MultiplyPoint(hOffset);
                matrix = Matrix4x4.TRS(Vector3.zero, matrix.rotation, Vector3.one); // remove scale
                matrix = matrix * offset;
                child.transform.position = matrix.Position() + h;
                child.transform.rotation = matrix.rotation;
                child.transform.localScale = matrix.lossyScale;
            }
        }
    }
}
