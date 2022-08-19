using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using Manifold.EditorTools.GC.GFZ.TPL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public abstract class GfzPropertyObject : MonoBehaviour
    {
        [SerializeField] private GfzSegmentShape Shape;
        [SerializeField] private MeshDisplay MeshDisplay;
        [SerializeField, Range(0f, 1f)] private float positionWidth = 0.5f;
        [SerializeField, Range(0f, 1f)] private float positionLength = 0f;

        public abstract EmbeddedTrackPropertyType Type { get; }
        public abstract float PropertyWidth { get; }
        public abstract float PropertyLength { get; }
        public float PositionWidth => positionWidth - 0.5f;
        public float PositionLength => positionLength;

        public abstract string[] GetModelNames();
        public abstract Model[] GetModels(TplTextureContainer tpl);
        public abstract SceneObjectDynamic[] GetSceneObjectDynamics();

        public EmbeddedTrackPropertyArea GetEmbeddedTrackPropertyArea()
        {
            var positionPoint = GetPositioningPoint();
            float trackWidth = positionPoint.lossyScale.x;

            // Edges left and right are normalized to -0.5 to 0.5
            float propertyHalfWidth = (PropertyWidth * 0.5f) / trackWidth;
            float edgeLeft = PositionWidth - propertyHalfWidth;
            float edgeRight = PositionWidth + propertyHalfWidth;
            // Edge from and to are "true" length positions
            float segmentBasePosition = Shape.GetRoot().GetDistanceOffset();
            float segmentLength = Shape.CopyAnimationCurveTRS(false).GetMaxTime();
            float lengthPosition = segmentBasePosition + segmentLength;
            float propertyHalfLength = PropertyLength * 0.5f;
            float edgeFrom = lengthPosition - propertyHalfLength;
            float edgeTo = lengthPosition + propertyHalfLength;

            // Construct
            var area = new EmbeddedTrackPropertyArea();
            area.TrackBranchID = Shape.GetBranchIndex();
            area.PropertyType = Type;
            area.WidthLeft = edgeLeft;
            area.WidthRight = edgeRight;
            area.LengthFrom = edgeFrom;
            area.LengthTo = edgeTo;

            return area;
        }

        public Matrix4x4 GetPositioningPoint()
        {
            return new Matrix4x4();
        }

        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            if (Shape == null)
                Shape = GetComponent<GfzSegmentShape>();

            if (MeshDisplay != null)
            {
                // TODO: make generic the code from shape that makes the path mesh
                //MeshDisplay.MeshFilter.mesh;
            }
        }
    }
}
