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
            // TODO: iterate mutiple times until length gets less than 1cm more length
            // per 10^x iterations?

            var maxTime = animTransform.GetMaxTime();
            var distance = animTransform.GetDistanceBetweenRepeated(0, maxTime, 2);

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

        public Checkpoint[] GetCheckpoints()
        {
            // Collect all possible checkpoint scripts on object
            var checkpointScripts = GetComponents<GfzTrackCheckpoints>();
            // Make sure there is only one
            Assert.IsTrue(checkpointScripts.Length == 1);
            var checkpointScript = checkpointScripts[0];

            // Get the gfz value for it, return
            var checkpoints = checkpointScript.GetCheckpoints();
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
