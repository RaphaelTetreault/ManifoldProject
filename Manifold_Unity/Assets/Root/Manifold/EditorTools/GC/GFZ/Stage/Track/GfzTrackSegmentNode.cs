using GameCube.GFZ.Stage;
using Manifold;
using Manifold.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public abstract class GfzTrackSegmentNode : MonoBehaviour
    {
        [field: SerializeField] public GfzTrackSegmentNode Prev { get; set; }
        [field: SerializeField] public GfzTrackSegmentNode Next { get; set; }
        [field: SerializeField] private float SegmentLength { get; set; } = -1;


        public abstract TrackSegmentType TrackSegmentType { get; }

        public abstract AnimationCurveTRS CreateAnimationCurveTRS(bool isGfzCoordinateSpace);
        public abstract TrackSegment CreateTrackSegment();

        // TODO? Consider adding children to construction?
        public HierarchichalAnimationCurveTRS CreateHierarchichalAnimationCurveTRS(bool isGfzCoordinateSpace)
        {
            var parentHacTRS = GetParentHacTRS(isGfzCoordinateSpace, out bool isRoot);
            var staticMatrix = GetStaticMatrix(isGfzCoordinateSpace, isRoot);
            var animTRS = CreateAnimationCurveTRS(isGfzCoordinateSpace);

            var hacTRS = new HierarchichalAnimationCurveTRS()
            {
                Parent = parentHacTRS,
                StaticMatrix = staticMatrix,
                AnimationCurveTRS = animTRS,
            };

            return hacTRS;
        }

        public bool IsRoot()
        {
            var parent = GetComponentInParent<GfzTrackSegmentShapeNode>();
            bool isRoot = parent != null;
            return isRoot;
        }

        public bool HasChildren()
        {
            var children = GetChildren();
            bool hasChildren = children.Length > 0;
            return hasChildren;
        }

        public GfzTrackSegmentNode GetRoot()
        {
            if (IsRoot())
                return this;
            else
                return GetParent().GetRoot();
        }

        public TGfzTrackSegmentNode GetParent<TGfzTrackSegmentNode>() where TGfzTrackSegmentNode : GfzTrackSegmentNode
        {
            var parentSegment = transform.parent.GetComponent<TGfzTrackSegmentNode>();
            return parentSegment;
        }

        public GfzTrackSegmentNode GetParent()
        {
            var parent = GetParent<GfzTrackSegmentShapeNode>();
            return parent;
        }

        public TGfzTrackSegmentNode[] GetChildren<TGfzTrackSegmentNode>() where TGfzTrackSegmentNode : GfzTrackSegmentNode
        {
            var children = new List<TGfzTrackSegmentNode>();
            foreach (var child in transform.GetChildren())
            {
                var childSegment = child.GetComponent<TGfzTrackSegmentNode>();
                bool childIsSegment = childSegment != null;
                if (childIsSegment)
                    children.Add(childSegment);
            }
            return children.ToArray();
        }

        public GfzTrackSegmentNode[] GetChildren()
        {
            var children = GetChildren<GfzTrackSegmentNode>();
            return children;
        }

        public Matrix4x4 GetStaticMatrix(bool useGfzCoordinateSpace, bool isRoot)
        {
            // If no parent, use world space, if child, use local space
            var position = isRoot
                ? transform.position
                : transform.localPosition;

            var rotation = isRoot
                ? transform.rotation.eulerAngles
                : transform.localRotation.eulerAngles;
            
            var scale = isRoot
                ? transform.lossyScale
                : transform.localPosition;

            // If GFZ, flip Z coordinate space
            if (useGfzCoordinateSpace)
            {
                position.z = -position.z;
                rotation.y = -rotation.y;
            }

            var matrix = Matrix4x4.TRS(position, Quaternion.Euler(rotation), scale);

            return matrix;
        }

        public HierarchichalAnimationCurveTRS GetParentHacTRS(bool isGfzCoordinateSpace, out bool isRoot)
        {
            var parent = GetParent();
            isRoot = parent == null;
            var parentHacTRS = isRoot ? null : parent.CreateHierarchichalAnimationCurveTRS(isGfzCoordinateSpace);
            return parentHacTRS;
        }

        public float GetSegmentLength()
        {
            var root = GetRoot();
            var segmentLength = root.SegmentLength;
            if (segmentLength <= 0f)
            {
                var msg = "Distance is 0 which is invalid. TRS animation curves must define path.";
                throw new System.ArgumentException(msg);
            }
            return segmentLength;
        }

        /// <summary>
        /// Sum of lengths from all previous segments.
        /// </summary>
        /// <returns></returns>
        public float GetDistanceOffset()
        {
            var track = FindObjectOfType<GfzTrack>();
            Assert.IsTrue(track != null, $"track is null.");
            
            // Call functions to update references
            //track.FindChildSegments();
            //track.AssignContinuity();
            Assert.IsTrue(track.FirstRoot != null, $"track.StartSegment is null.");

            // If we are the start segment, offset is 0
            var startSegment = track.FirstRoot;
            if (this == startSegment)
                return 0f;

            var distanceOffset = 0f;
            var previousSegment = Prev;
            while (previousSegment is not null)
            {
                distanceOffset += previousSegment.GetSegmentLength();

                // If we strumble onto the first segment, stop getting lengths
                // (we are at the start, don't get subsequent previous node)
                if (previousSegment == startSegment)
                    break;

                previousSegment = previousSegment.Prev;

                // If somehow previous segments wrap to this segment, we done goofed
                Assert.IsTrue(previousSegment != this, $"You done goofed. 'track.StartSegment' is probably not set.");
            }
            return distanceOffset;
        }

        public byte GetBranchIndex()
        {
            // TODO: recurse up hierachy, stopping on branch node, getting ID
            return 0;
        }



    }
}
