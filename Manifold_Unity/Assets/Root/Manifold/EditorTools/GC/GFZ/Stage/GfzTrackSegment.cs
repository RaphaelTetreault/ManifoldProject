using GameCube.GFZ.CourseCollision;
using Manifold.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public abstract class GfzTrackSegment : MonoBehaviour,
        IEditableComponent<GfzTrackSegment>
    {
        // Fields
        [Header("Links")]
        [SerializeField] protected GfzTrackSegment prev;
        [SerializeField] protected GfzTrackSegment next;
        [SerializeField] protected int metersPerCheckpoint = 100;


        [Header("Unknown properties")]
        [SerializeField] protected byte unk0x3B;


        [Header("Track Curves")]
        [SerializeField] protected bool genRotationXY;
        [SerializeField] protected AnimationCurveTransform animTransform = new AnimationCurveTransform();


        public event IEditableComponent<GfzTrackSegment>.OnEditCallback OnEdited;




        // Properties
        public GfzTrackSegment PreviousSegment
        {
            get => prev;
            set => prev = value;
        }
        public GfzTrackSegment NextSegment
        {
            get => next;
            set => next = value;
        }
        public AnimationCurveTransform AnimTransform => animTransform;


        // init track segment
        //protected TrackSegment trackSegment;
        //public TrackSegment TrackSegment => trackSegment;



        public abstract TrackSegment GenerateTrackSegment();

        public virtual float GetSegmentLength()
        {
            // 2022/01/31: current work assumes min and max of 0 and 1
            var maxTime = animTransform.GetMaxTime();
            Assert.IsTrue(maxTime == 1);

            var distance = animTransform.GetDistanceBetweenRepeated(0, 1);
            return distance;
        }

        public abstract Mesh[] GenerateMeshes();

        public EmbeddedTrackPropertyArea[] GetEmbededPropertyAreas()
        {
            // Get all properties on self and children.
            var embededProperties = GetComponentsInChildren<GfzTrackEmbededProperty>();

            // Iterate over collection
            var count = embededProperties.Length;
            var embededPropertyAreas = new EmbeddedTrackPropertyArea[count+1];
            for (int i = 0; i < count; i++)
            {
                embededPropertyAreas[i] = embededProperties[i].GetEmbededProperty();
            }
            embededPropertyAreas[count] = EmbeddedTrackPropertyArea.Terminator();
            return embededPropertyAreas;
        }

        public Checkpoint[] GetCheckpoints(bool convertCoordinateSpace)
        {
            //
            var segmentLength = GetSegmentLength();
            var numCheckpoints = Mathf.CeilToInt(segmentLength / metersPerCheckpoint);
            var checkpoints = new Checkpoint[numCheckpoints];

            var distanceOffset = 0f;

            // Get the AnimationCurveTransform appropriate for requester.
            // Use GFZ space (game) if 'true'
            // Use Unity space if 'false'
            var animTransform = convertCoordinateSpace
                ? AnimTransform.GetGfzCoordSpaceAnimTransform()
                : AnimTransform;

            var curveMaxTime = animTransform.GetMaxTime();
            var pos = animTransform.Position;
            var rot = animTransform.Rotation;
            var scl = animTransform.Scale;

            for (int i = 0; i < numCheckpoints; i++)
            {
                // Curve-sampling start and end times.
                double checkpointTimeStart = (double)(i + 0) / numCheckpoints;
                double checkpointTimeEnd = (double)(i + 1) / numCheckpoints;

                // Get origin of start plane, track width at start sampling point
                var origin = pos.Evaluate(checkpointTimeStart);
                var normal = Quaternion.Euler(rot.Evaluate(checkpointTimeStart)) * Vector3.back;
                var trackWidth = scl.x.Evaluate((float)checkpointTimeStart);

                // DISTANCE
                // Compute the distance between these 2 points, keep track of total distance travelled along segment
                var distanceBetween = animTransform.GetDistanceBetweenRepeated(checkpointTimeStart, checkpointTimeEnd);
                var distanceStart = distanceOffset;
                var distanceEnd = distanceOffset + distanceBetween;
                distanceOffset = distanceEnd;

                // CHECKPOINT
                checkpoints[i] = new Checkpoint();
                var checkpoint = checkpoints[i];
                checkpoint.curveTimeStart = (float)checkpointTimeStart;
                checkpoint.startDistance = distanceStart;
                checkpoint.endDistance = distanceEnd;
                checkpoint.trackWidth = trackWidth;
                checkpoint.connectToTrackIn = true;
                checkpoint.connectToTrackOut = true;
                checkpoint.planeStart.origin = origin;
                checkpoint.planeStart.normal = normal;
                checkpoint.planeStart.ComputeDotProduct();
                // We construct (copy) the checkpoint.planeEnd later
            }

            // Copy values from one checkpoint to the previous one
            // NOTE: start at second index '1' since we refer to the previous checkpoint (i-1)
            for (int i = 1; i < checkpoints.Length; i++)
            {
                var prevCheckpoint = checkpoints[i - 1];
                var currCheckpoint = checkpoints[i];
                // Copy over values
                prevCheckpoint.curveTimeEnd = currCheckpoint.curveTimeStart;
                prevCheckpoint.planeEnd = currCheckpoint.planeStart.GetMirror();
            }

            // Complete missing information in last checkpoint of segment
            {
                var lastCheckpoint = checkpoints[checkpoints.Length - 1];
                lastCheckpoint.curveTimeEnd = curveMaxTime;
                var origin = pos.Evaluate(curveMaxTime);
                var normal = Quaternion.Euler(rot.Evaluate(curveMaxTime)) * Vector3.forward;
                lastCheckpoint.planeEnd.origin = origin;
                lastCheckpoint.planeEnd.normal = normal;
                lastCheckpoint.planeEnd.ComputeDotProduct();
            }

            return checkpoints;
        }


        public virtual void OnValidate()
        {
            // Once this has been edited, let listeners know
            OnEdited?.Invoke(this);

            if (genRotationXY)
            {
                var anims = animTransform.ComputerRotationXY();
                animTransform.Rotation.x = anims.x;
                animTransform.Rotation.y = anims.y;
                //animTransform.Rotation.z = anims.z;
                genRotationXY = false;
            }

        }
    }
}
