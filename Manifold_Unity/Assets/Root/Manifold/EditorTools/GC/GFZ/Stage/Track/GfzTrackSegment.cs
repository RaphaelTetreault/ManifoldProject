using GameCube.GFZ.Stage;
using Manifold.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    [ExecuteInEditMode]
    public sealed class GfzTrackSegment : MonoBehaviour
    {
        // Fields
        [Header("Checkpoints")]
        [SerializeField] private GfzTrackSegment start;
        [SerializeField] private GfzTrackSegment end;
        [SerializeField] private float metersPerCheckpoint = 100;

        [Header("Track Curves")]
        [SerializeField] private SegmentGenerator segmentGenerator;
        [SerializeField] private AnimationCurveTRS animationCurveTRS = new();


        [field: SerializeField] private HierarchichalAnimationCurveTRS TrsHierarchy = new();


        // Properties
        public GfzTrackSegment PreviousSegment
        {
            get => start;
            set => start = value;
        }
        public GfzTrackSegment NextSegment
        {
            get => end;
            set => end = value;
        }
        public AnimationCurveTRS AnimationCurveTRS => animationCurveTRS;


        public float GetSegmentLength()
        {
            // 2022/01/31: current work assumes min and max of 0 and 1
            var maxTime = animationCurveTRS.GetMaxTime();
            Assert.IsTrue(maxTime == 1);
            // tODO: get min time, assert

            var distance = animationCurveTRS.GetDistanceBetweenRepeated(0, 1);
            return distance;
        }

        public EmbeddedTrackPropertyArea[] GetEmbededPropertyAreas()
        {
            // Get all properties on self and children.
            var embededProperties = GetComponentsInChildren<GfzTrackEmbededProperty>();

            // Iterate over collection
            var count = embededProperties.Length;
            var embededPropertyAreas = new EmbeddedTrackPropertyArea[count + 1];
            for (int i = 0; i < count; i++)
            {
                embededPropertyAreas[i] = embededProperties[i].GetEmbededProperty();
            }
            embededPropertyAreas[count] = EmbeddedTrackPropertyArea.Terminator();
            return embededPropertyAreas;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="convertCoordinateSpace"></param>
        /// <returns></returns>
        public Checkpoint[] CreateCheckpoints(bool convertCoordinateSpace)
        {
            //
            var segmentLength = GetSegmentLength();
            var numCheckpoints = Mathf.CeilToInt(segmentLength / metersPerCheckpoint);
            var checkpoints = new Checkpoint[numCheckpoints];

            var distanceOffset = 0f;

            // Get the AnimationCurveTransform appropriate for requester.
            // Use GFZ space (game) if 'true'
            // Use Unity space if 'false'
            var animationTRS = convertCoordinateSpace
                ? AnimationCurveTRS.GetGfzCoordSpaceTRS()
                : AnimationCurveTRS;

            Vector3 forward = -segmentGenerator.transform.forward;
            Vector3 backward = -forward;

            var curveMaxTime = animationTRS.GetMaxTime();
            var baseMtx = segmentGenerator.transform.localToWorldMatrix;
            //var pos = animationTRS.Position;
            //var rot = animationTRS.Rotation;
            //var scl = animationTRS.Scale;

            for (int i = 0; i < numCheckpoints; i++)
            {
                // Curve-sampling start and end times.
                double checkpointTimeStart = (double)(i + 0) / numCheckpoints;
                double checkpointTimeEnd = (double)(i + 1) / numCheckpoints;

                //
                var animMtx = animationTRS.EvaluateMatrix(checkpointTimeStart);
                var mtx = baseMtx * animMtx;
                var position = mtx.GetPosition();
                var rotation = mtx.rotation;
                //var scale = mtx.lossyScale;
                var scale = animationTRS.Scale.Evaluate(checkpointTimeStart);

                // Get origin of start plane, track width at start sampling point
                var origin = position;// + transform.position;
                var trackWidth = scale.x;
                var normal = rotation * forward;
                //
                var planeStart = new GameCube.GFZ.Stage.Plane() { origin = origin, normal = normal };
                planeStart.ComputeDotProduct();

                // DISTANCE
                // Compute the distance between these 2 points, keep track of total distance travelled along segment
                var distanceBetween = animationTRS.GetDistanceBetweenRepeated(checkpointTimeStart, checkpointTimeEnd);
                var distanceStart = distanceOffset;
                var distanceEnd = distanceOffset + distanceBetween;
                distanceOffset = distanceEnd;

                // CHECKPOINT
                checkpoints[i] = new Checkpoint();
                var checkpoint = checkpoints[i];
                checkpoint.CurveTimeStart = (float)checkpointTimeStart;
                checkpoint.StartDistance = distanceStart;
                checkpoint.EndDistance = distanceEnd;
                checkpoint.TrackWidth = trackWidth;
                checkpoint.ConnectToTrackIn = true;
                checkpoint.ConnectToTrackOut = true;
                checkpoint.PlaneStart = planeStart;
                // We construct (copy) the checkpoint.planeEnd later
            }

            // Copy values from one checkpoint to the previous one
            // NOTE: start at second index '1' since we refer to the previous checkpoint (i-1)
            for (int i = 1; i < checkpoints.Length; i++)
            {
                var prevCheckpoint = checkpoints[i - 1];
                var currCheckpoint = checkpoints[i];
                // Copy over values
                prevCheckpoint.CurveTimeEnd = currCheckpoint.CurveTimeStart;
                prevCheckpoint.PlaneEnd = currCheckpoint.PlaneStart.GetMirror();
            }

            // Index for last checkpoint
            var lastIndex = checkpoints.Length - 1;

            // Complete missing information in last checkpoint of segment
            {
                var lastCheckpoint = checkpoints[lastIndex];
                lastCheckpoint.CurveTimeEnd = curveMaxTime;

                var animMtx = animationTRS.EvaluateMatrix(curveMaxTime);
                var mtx = baseMtx * animMtx;

                var origin = mtx.GetPosition();
                var rotation = mtx.rotation;
                var normal = rotation * backward;

                var endPlane = new GameCube.GFZ.Stage.Plane() { origin = origin, normal = normal };
                endPlane.ComputeDotProduct();

                lastCheckpoint.PlaneEnd = endPlane;
            }

            // Set segment in/out connections
            var connectToTrackIn = start != null;
            var connectToTrackOut = end != null;
            checkpoints[0].ConnectToTrackIn = connectToTrackIn;
            checkpoints[lastIndex].ConnectToTrackOut = connectToTrackOut;

            // That's all!
            return checkpoints;
        }

        public TrackSegment GetSegment()
        {
            var trackSegment = new TrackSegment();

            trackSegment.LocalPosition = transform.localPosition;
            trackSegment.LocalRotation = transform.localRotation.eulerAngles;
            trackSegment.LocalScale = transform.localScale;

            // Get animation data
            trackSegment.AnimationCurveTRS = animationCurveTRS.ToTrackSegment();
            // Move rotation.z to child node, otherwise matrix is messed up with x, y, and z rotations
            trackSegment.Children = new TrackSegment[] { new() };
            trackSegment.Children[0].AnimationCurveTRS.RotationZ = trackSegment.AnimationCurveTRS.RotationZ;
            // Make root rotation.z empty
            trackSegment.AnimationCurveTRS.RotationZ = new();

            // TODO: currently hardcoded
            // Well, could be good actually. Other tracks can simply override this
            trackSegment.SegmentType = TrackSegmentType.IsMatrix;
            trackSegment.Children[0].SegmentType = TrackSegmentType.IsTrack;

            //
            return trackSegment;
        }





        private void OnValidate()
        {

        }


        private void Awake()
        {
            if (segmentGenerator is not null)
                return;

            segmentGenerator.OnEdited += GenerateAnimationCurves;
        }

        private void OnDestroy()
        {
            if (segmentGenerator is not null)
                return;

            segmentGenerator.OnEdited -= GenerateAnimationCurves;
        }



        /// <summary>
        /// Creates new animation TRS from segment generator
        /// </summary>
        public void GenerateAnimationCurves()
        {
            animationCurveTRS = segmentGenerator.GetAnimationCurveTRS();
        }


        public void GenerateCheckpointDebug()
        {
            var checkpoints = CreateCheckpoints(false);

            int index = 0;
            foreach (var checkpoint in checkpoints)
            {
                var gobj = new GameObject($"Checkpoint[{index++}]");
                gobj.transform.parent = this.transform;
                var script = gobj.AddComponent<GfzCheckpoint>();
                script.Init(checkpoint);
            }
        }
    }
}
