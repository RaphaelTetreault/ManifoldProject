using GameCube.GFZ;
using GameCube.GFZ.Stage;
using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class TagTrackSegment : MonoBehaviour
    {
        public TrackSegmentType segmentType;
        public TrackEmbeddedPropertyType embeddedPropertyType;
        public TrackPerimeterFlags perimeterFlags;
        public TrackPipeCylinderFlags pipeCylinderFlags;
        public float3 localPosition;
        public float3 localRotation;
        public float3 localScale;
        public ushort root_unk_0x38;
        public ushort root_unk_0x3A; 
        public float railHeightRight;
        public float railHeightLeft;
        public int branchIndex; // 0, 1, 2, 3

        public AnimationCurveTRS curves;
        public bool hasCorner;
        public float3 cornerPosition;
        public float3 cornerRotation;
        public float3 cornerScale;
        public float width;
        public TrackPerimeterFlags cornerPerimeterFlags;

        public List<TagTrackSegment> children = new List<TagTrackSegment>();
        public int depth;


        private void OnDrawGizmos()
        {
            if (hasCorner)
            {
                var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
                Gizmos.color = Color.green;
                var scale = new Vector3(width, 5f, 5f);
                var rot = Quaternion.Euler(cornerRotation);
                var pos = cornerPosition;
                pos.x = -pos.x;
                Gizmos.DrawMesh(mesh, 0, pos, rot, scale);
            }
        }

        public void SetCurves(TrackSegment trackSegment)
        {
            curves = new AnimationCurveTRS();
            var gfzCurves = trackSegment.AnimationCurveTRS.AnimationCurvesOrderedPRS;

            // Convert from animation curves from Gfz to Unity formats
            for (int i = 0; i < gfzCurves.Length; i++)
            {
                var animationCurve = gfzCurves[i];
                var keyables = AnimationCurveConverter.EnforceNoDuplicateTimes(animationCurve.KeyableAttributes);
                var keyframes = AnimationCurveConverter.KeyablesToKeyframes(keyables);
                // Unity 2022.1.5f is broken... use workaround
                //curves[i] = new UnityEngine.AnimationCurve(keyframes);
                curves[i] = new UnityEngine.AnimationCurve();
                curves[i].keys = keyframes;
                var unityCurve = curves[i];

                if (unityCurve.length != keyframes.Length)
                {
                    //Assert.IsTrue(curves[i].length == keyframes.Length);
                    Debug.Log($"Error? UnityCurve:{unityCurve.length}, Keyframes:{keyframes.Length}, Keyables:{keyables.Length}, GfzCurve:{animationCurve.Length}");
                }

                //
                AnimationCurveConverter.SetGfzTangentsToUnityTangents(keyables, unityCurve);

                // TEST - re-apply key values.
                // Not being respected by Unity?
                for (int j = 0; j < unityCurve.length; j++)
                {
                    unityCurve.keys[j].inTangent = keyframes[j].inTangent;
                    unityCurve.keys[j].outTangent = keyframes[j].outTangent;
                }
            }
        }

        //private void OnValidate()
        //{
        //    var tangent = curves.Scale.x.keys[0].outTangent;
        //    Debug.Log(tangent);
        //}

    }
}