using GameCube.GFZ.GMA;
using GameCube.GFZ.Stage;
using Manifold;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public enum SurfaceEmbedType : byte
    {
        Recover,
        Damage,
        Slip,
        Dirt,
    }


    public class GfzTrackSurfaceEmbed : GfzTrackSegmentShapeNode
    {
        [field: Header("Properties")]
        [field: SerializeField] public SurfaceEmbedType Type { get; private set; } = SurfaceEmbedType.Recover;
        [field: SerializeField, Range(0f, 1f)] public float From { get; private set; } = 0f;
        [field: SerializeField, Range(0f, 1f)] public float To { get; private set; } = 1f;
        [field: SerializeField] public UnityEngine.AnimationCurve Width { get; private set; } = new UnityEngine.AnimationCurve(new(0, 0.5f), new(1, 0.5f));
        [field: SerializeField] public UnityEngine.AnimationCurve Offset { get; private set; } = new UnityEngine.AnimationCurve(new(0, 0), new(1, 0));


        public Color32 GetColor(SurfaceEmbedType type)
        {
            switch (type)
            {
                case SurfaceEmbedType.Recover: return new Color32(240, 25, 55, 255); // hot-pink (sampled from GFZ)
                case SurfaceEmbedType.Damage: return new Color32(255, 0, 0, 255); // red
                case SurfaceEmbedType.Slip: return new Color32(109, 170, 210, 255); // blue (approximate, sampled from GFZ)
                case SurfaceEmbedType.Dirt: return new Color32(52, 28, 8, 255); // brown (sampled from GFZ)
                default:
                    throw new System.ArgumentException();
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
        }

        public override TrackSegmentType TrackSegmentType => throw new System.NotImplementedException();

        public override AnimationCurveTRS CreateAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            throw new System.NotImplementedException();
        }

        public override Gcmf CreateGcmf()
        {
            throw new System.NotImplementedException();
        }

        public override Mesh CreateMesh()
        {
            throw new System.NotImplementedException();

            //var hacTRS = new HierarchichalAnimationCurveTRS();

            //var animationCurveTRS = AnimationCurveTRS.CreateDeepCopy();
            //var staticMatrix = transform.localToWorldMatrix;

            //// THE TODO HERE is proper matrix hierarchy

            //var matrices = TrackGeoGenerator.GenerateMatrixIntervals(animationCurveTRS, staticMatrix, LengthDistance);

            ////
            //var endpointA = new Vector3(-0.5f, 0.01f, 0f);
            //var endpointB = new Vector3(+0.5f, 0.01f, 0f);
            //var color0 = GetColor(Type);
            //var tristrips = TrackGeoGenerator.CreateTristrips(matrices, endpointA, endpointB, 1, color0, Vector3.up, 0, false);
            //GenMesh = TristripsToMesh(tristrips);
            //GenMesh.name = $"Auto Gen - {this.name}";

            //if (MeshFilter != null)
            //    MeshFilter.mesh = GenMesh;

            //if (MeshRenderer != null)
            //{
            //    int numTristrips = GenMesh.subMeshCount;
            //    var materials = new Material[numTristrips];
            //    for (int i = 0; i < materials.Length; i++)
            //        materials[i] = DefaultMaterial;
            //    MeshRenderer.sharedMaterials = materials;
            //}

            //return new Mesh[] { GenMesh };
        }

        public override TrackSegment CreateTrackSegment()
        {
            throw new System.NotImplementedException();
        }
    }
}
