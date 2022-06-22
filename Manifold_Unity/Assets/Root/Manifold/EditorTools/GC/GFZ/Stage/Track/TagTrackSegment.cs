using GameCube.GFZ;
using GameCube.GFZ.Stage;
using Manifold;
using Manifold.IO;
using System;
using Unity.Mathematics;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class TagTrackSegment : MonoBehaviour
    {
        public TrackSegmentType segmentType;
        public TrackEmbeddedPropertyType embeddedPropertyType;
        public TrackPerimeterFlags perimeterFlags;
        public TrackPipeCylinderFlags pipeCylinderFlags;
        //public Pointer trackCurvesPtr;
        //public Pointer trackCornerPtr;
        //public ArrayPointer childrenPtrs;
        public float3 localPosition;
        public float3 localRotation;
        public float3 localScale;
        public byte unk_0x38; // mixed flags
        public byte unk_0x39; // exclusive flags
        public byte unk_0x3A; // mixed flags
        public byte unk_0x3B; // mixed flags
        public float railHeightRight;
        public float railHeightLeft;
        public int branchIndex; // 0, 1, 2, 3

        public AnimationCurveTRS curves;
        public bool hasCorner;
        public Matrix4x4 cornerTransformMatrix4x4;

        public TagTrackSegment[] children;



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
    }
}