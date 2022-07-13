//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace Manifold.EditorTools.GC.GFZ.Stage.Track
//{
//    public abstract class GfzTrackShapeLine : GfzTrackSegmentShapeNode
//    {
//        [field: Header("Mesh Properties")]
//        [field: SerializeField] public Material DefaultMaterial { get; protected set; }
//        [field: SerializeField] public MeshFilter MeshFilter { get; protected set; }
//        [field: SerializeField] public MeshRenderer MeshRenderer { get; protected set; }
//        [field: SerializeField] public Mesh GenMesh { get; protected set; }
//        [field: SerializeField] public bool DoGenMesh { get; protected set; }
//        [field: SerializeField, Min(1)] public int WidthDivisions { get; protected set; } = 4;
//        [field: SerializeField, Min(1f)] public float LengthDistance { get; protected set; } = 10f;

//        protected override void OnValidate()
//        {
//            base.OnValidate();

//            if (MeshRenderer == null)
//                MeshRenderer = GetComponent<MeshRenderer>();

//            if (MeshFilter == null)
//                MeshFilter = GetComponent<MeshFilter>();

//            if (DoGenMesh)
//            {
//                GenerateMeshes();
//                DoGenMesh = false;
//            }
//        }

//        public override TrackSegmentType TrackSegmentType => throw new System.NotImplementedException();

//        public override AnimationCurveTRS CreateAnimationCurveTRS(bool isGfzCoordinateSpace)
//        {
//            throw new System.NotImplementedException();
//        }

//        public override Gcmf CreateGcmf()
//        {
//            throw new System.NotImplementedException();
//        }

//        public override Mesh CreateMesh()
//        {
//            throw new System.NotImplementedException();
//        }

//        public override TrackSegment[] CreateTrackSegments()
//        {
//            throw new System.NotImplementedException();
//        }
//    }
//}
