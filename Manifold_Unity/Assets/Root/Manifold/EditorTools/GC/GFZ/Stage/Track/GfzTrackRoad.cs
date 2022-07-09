using GameCube.GFZ.Stage;
using Manifold.EditorTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class GfzTrackRoad : GfzSegmentShape,
        IRailSegment
    {
        [field: Header("Mesh Properties")]
        [field: SerializeField] public Material DefaultMaterial { get; private set; }
        [field: SerializeField] public MeshFilter MeshFilter { get; private set; }
        [field: SerializeField] public MeshRenderer MeshRenderer { get; private set; }
        [field: SerializeField] public Mesh GenMesh { get; private set; }
        [field: SerializeField] public bool DoGenMesh { get; private set; }
        [field: SerializeField, Min(1)] public int WidthDivisions { get; private set; } = 4;
        [field: SerializeField, Min(1f)] public float LengthDistance { get; private set; } = 10f;

        [field: Header("Road Properties")]
        [field: SerializeField, Min(0f)] public float RailHeightLeft { get; private set; } = 3f;
        [field: SerializeField, Min(0f)] public float RailHeightRight { get; private set; } = 3f;



        public override Mesh[] GenerateMeshes()
        {
            var tristrips = TrackGeoGenerator.CreateAllTemp(this, WidthDivisions, LengthDistance, false);
            GenMesh = TristripsToMesh(tristrips);
            GenMesh.name = $"Auto Gen - {this.name}";

            if (MeshFilter != null)
                MeshFilter.mesh = GenMesh;

            if (MeshRenderer != null)
            {
                int numTristrips = GenMesh.subMeshCount;
                var materials = new Material[numTristrips];
                for (int i = 0; i < materials.Length; i++)
                    materials[i] = DefaultMaterial;
                MeshRenderer.sharedMaterials = materials;
            }

            return new Mesh[] { GenMesh };
        }

        public override TrackSegment GenerateTrackSegment()
        {
            var trackSegment = Segment.GetTrackSegment();
            var lastNode = trackSegment.Children[0];

            // Override the rail properies
            IO.Assert.IsTrue(lastNode.SegmentType == TrackSegmentType.IsTrack);

            // Rail height
            lastNode.SetRails(RailHeightLeft, RailHeightRight);

            //
            return trackSegment;
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (DoGenMesh)
            {
                GenerateMeshes();
                DoGenMesh = false;
            }

            if (MeshRenderer == null)
                MeshRenderer = GetComponent<MeshRenderer>();

            if (MeshFilter == null)
                MeshFilter = GetComponent<MeshFilter>();
        }

    }
}
