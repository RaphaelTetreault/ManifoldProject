using GameCube.GFZ.Stage;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public class GfzShapeOpenPipeCylinder : GfzShape
    {
        [Header("Open-Pipe / Open-Cylinder")]
        [SerializeField] private PipeCylinderType type = PipeCylinderType.Pipe;
        [SerializeField] private OpenPipeStyle pipeStyle;
        [SerializeField] private OpenCylinderStyle cylinderStyle;
        [SerializeField] public EndcapMode endcapModeIn = EndcapMode.Automatic;
        [SerializeField] public EndcapMode endcapModeOut = EndcapMode.Automatic;
        [SerializeField, Min(2f)] private float subdivisionsPerLength = 20f;
        [SerializeField, Min(8)] private int subdivisionsPipe = 32;
        [SerializeField, Min(6)] private int subdivisionsCylinder = 32;
        [SerializeField] private UnityEngine.AnimationCurve opennessCurve = new(new(0, 0.5f), new(1, 1));

        public PipeCylinderType Type { get => type; set => type = value; }
        public OpenPipeStyle PipeStyle { get => pipeStyle; set => pipeStyle = value; }
        public OpenCylinderStyle CylinderStyle { get => cylinderStyle; set => cylinderStyle = value; }
        public float SubdivisionsPerLength { get => subdivisionsPerLength; set => subdivisionsPerLength = value; }
        public int SubdivisionsPipe { get => subdivisionsPipe; set => subdivisionsPipe = value; }
        public int SubdivisionsCylinder { get => subdivisionsCylinder; set => subdivisionsCylinder = value; }

        public UnityEngine.AnimationCurve OpennessCurveRenormalized => new(opennessCurve.GetRenormalizedKeyRangeAndTangents(0, GetRoot().GetMaxTime()));


        public enum OpenPipeStyle
        {
            MuteCityCOM,
            Debug = -1,
        }
        public enum OpenCylinderStyle
        {
            MuteCityCOM,
            Debug = -1,
        }

        public override ShapeID ShapeIdentifier
        {
            get
            {
                bool isPipe = type == PipeCylinderType.Pipe;

                if (isPipe)
                    return ShapeID.pipe;
                else
                    return ShapeID.cylinder;
            }
        }
        public override EndcapMode EndcapModeIn => endcapModeIn;
        public override EndcapMode EndcapModeOut => endcapModeOut;

        public override AnimationCurveTRS CopyAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            return new AnimationCurveTRS();
        }

        public override GcmfTemplate[] GetGcmfTemplates()
        {
            bool isPipe = type == PipeCylinderType.Pipe;
            var gcmfTemplates = isPipe
                ? GetPipeGcmfTemplates(pipeStyle)
                : GetCylinderGcmfTemplates(cylinderStyle);
            return gcmfTemplates;
        }
        public GcmfTemplate[] GetPipeGcmfTemplates(OpenPipeStyle pipeStyle)
        {
            switch (pipeStyle)
            {
                case OpenPipeStyle.MuteCityCOM:
                    return TristripTemplates.OpenPipe.MuteCityCOM.OpenPipeMaterials();
                default:
                    return new GcmfTemplate[]{ GcmfTemplates.Debug.CreateLitVertexColoredDoubleSided() };
            }
        }
        public GcmfTemplate[] GetCylinderGcmfTemplates(OpenCylinderStyle cylinderStyle)
        {
            switch (cylinderStyle)
            {
                case OpenCylinderStyle.MuteCityCOM:
                    return TristripTemplates.OpenCylinder.MuteCityCOM.OpenCylinderMaterials();
                default:
                    return new GcmfTemplate[] { GcmfTemplates.Debug.CreateLitVertexColored() };
            }
        }

        public override Tristrip[][] GetTristrips(bool isGfzCoordinateSpace)
        {
            bool isPipe = type == PipeCylinderType.Pipe;
            var tristrips = isPipe
                ? GetPipeTristrips(pipeStyle, isGfzCoordinateSpace)
                : GetCylinderTristrips(cylinderStyle, isGfzCoordinateSpace);
            return tristrips;
        }
        public Tristrip[][] GetPipeTristrips(OpenPipeStyle pipeStyle, bool isGfzCoordinateSpace)
        {
            var matrices = TristripGenerator.CreatePathMatrices(this, isGfzCoordinateSpace, subdivisionsPerLength);
            var maxTime = GetRoot().GetMaxTime();

            switch (pipeStyle)
            {
                case OpenPipeStyle.MuteCityCOM:
                    return TristripTemplates.OpenPipe.MuteCityCOM.OpenPipe(matrices, this, maxTime, isGfzCoordinateSpace);
                default:
                    return TristripTemplates.OpenPipe.DebugOpenPipe(matrices, this, isGfzCoordinateSpace);
            }
        }
        public Tristrip[][] GetCylinderTristrips(OpenCylinderStyle cylinderStyle, bool isGfzCoordinateSpace)
        {
            var matrices = TristripGenerator.CreatePathMatrices(this, isGfzCoordinateSpace, subdivisionsPerLength);
            var maxTime = GetRoot().GetMaxTime();

            switch (cylinderStyle)
            {
                case OpenCylinderStyle.MuteCityCOM:
                    return TristripTemplates.OpenCylinder.MuteCityCOM.OpenCylinder(matrices, this, maxTime, isGfzCoordinateSpace);
                default:
                    return TristripTemplates.OpenCylinder.DebugOpenCylinder(matrices, this, isGfzCoordinateSpace);
            }
        }

        public override TrackSegment CreateTrackSegment()
        {
            // Set flag on if cylinder
            var typeFlags = type == PipeCylinderType.Cylinder ? TrackPipeCylinderFlags.IsCylinderNotPipe : 0;
            // TRS is empty but with Scale.Y acting as the "openness" curve
            var trs = new AnimationCurveTRS();
            trs.Scale.y = OpennessCurveRenormalized;

            var leafEmbed = new TrackSegment();
            leafEmbed.OrderIndentifier = name + "_leaf";
            leafEmbed.SegmentType = TrackSegmentType.IsEmbed;
            leafEmbed.PipeCylinderFlags = TrackPipeCylinderFlags.IsOpenPipeOrCylinder;
            leafEmbed.BranchIndex = GetBranchIndex();
            leafEmbed.AnimationCurveTRS = trs.ToTrackSegment();
            leafEmbed.Children = CreateChildTrackSegments();

            // can define rail height... but ignored
            var rootEmbed = new TrackSegment();
            rootEmbed.OrderIndentifier = name + "_root";
            rootEmbed.SegmentType = TrackSegmentType.IsEmbed;
            rootEmbed.EmbeddedPropertyType = TrackEmbeddedPropertyType.IsOpenPipeOrCylinder;
            rootEmbed.PerimeterFlags = TrackPerimeterFlags.hasRailHeightLeft | TrackPerimeterFlags.hasRailHeightRight;
            rootEmbed.RailHeightLeft = 3;
            rootEmbed.RailHeightRight = 3;
            rootEmbed.PipeCylinderFlags = typeFlags; // DEFINES IF PIPE OR CYLINDER
            rootEmbed.BranchIndex = GetBranchIndex();
            rootEmbed.Children = new TrackSegment[] { leafEmbed };

            return rootEmbed;
        }

        public override void UpdateTRS()
        {
            // 
            RenormalizedOpennessCurve();
        }

        public void RenormalizedOpennessCurve()
        {
            var maxTime = GetRoot().GetMaxTime();
            var keys = opennessCurve.GetRenormalizedKeyRangeAndTangents(0, maxTime);
            opennessCurve = new UnityEngine.AnimationCurve(keys);
        }
    }
}
