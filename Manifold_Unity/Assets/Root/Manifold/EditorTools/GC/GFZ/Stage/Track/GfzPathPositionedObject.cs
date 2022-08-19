using System;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzPathPositionedObject : GfzPathPositionedBase
    {
        [SerializeField, Range(-0.5f, +0.5f)] private float widthPosition;
        [SerializeField, Range(0f, 1f)] private float LengthPosition;

        public override void CreateObjects()
        {
            DeleteObjects();

            var hacTRS = segment.CreateHierarchichalAnimationCurveTRS(false);
            float time = LengthPosition * hacTRS.GetMaxTime();
            var pathMatrix = hacTRS.EvaluateHierarchyMatrix(time);
            var offsetMatrix = Matrix4x4.TRS(positionOffset, Quaternion.Euler(rotationOffset), scaleOffset);
            var matrix = pathMatrix * offsetMatrix;

            Vector3 horizontalOffset = widthPosition * Vector3.right;
            var position = matrix.MultiplyPoint(horizontalOffset);
            var rotation = matrix.rotation;
            var scale = offsetMatrix.lossyScale;

            var child = GameObject.Instantiate(prefab, transform);
            child.SetActive(true);
            child.transform.position = position;
            child.transform.rotation = rotation;
            child.transform.localScale = scale;
        }
    }
}