using GameCube.GFZ.Stage;
using Manifold;
using Manifold.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public enum Scope
    {
        Local,
        Global,
    }

    public abstract class GfzTrackSegmentNode : MonoBehaviour
    {
        public abstract AnimationCurveTRS CreateAnimationCurveTRS(Scope scope, bool isGfzCoordinateSpace = false);


        public abstract TrackSegment CreateTrackSegment();

        // TODO? Consider adding children to construction?
        public HierarchichalAnimationCurveTRS CreateHierarchichalAnimationCurveTRS(Scope scope, bool isGfzCoordinateSpace)
        {
            var parentHacTRS = GetParentHacTRS(scope, isGfzCoordinateSpace, out bool isRoot);
            //var staticMatrix = GetStaticMatrix(isGfzCoordinateSpace, isRoot);
            var animTRS = CreateAnimationCurveTRS(scope, isGfzCoordinateSpace);

            var hacTRS = new HierarchichalAnimationCurveTRS()
            {
                Parent = parentHacTRS,
                //StaticMatrix = staticMatrix,
                AnimationCurveTRS = animTRS,
            };

            return hacTRS;
        }

        public abstract float GetMaxTime();

        public bool IsRoot()
        {
            var parent = GetParent();
            bool isRoot = parent == null;
            return isRoot;
        }

        public bool HasChildren()
        {
            var children = GetChildren();
            bool hasChildren = children.Length > 0;
            return hasChildren;
        }

        public GfzTrackSegmentRootNode GetRoot()
        {
            if (IsRoot())
            {
                bool isRootType = this is GfzTrackSegmentRootNode;
                if (!isRootType)
                    throw new System.Exception($"Root node is not of type {typeof(GfzTrackSegmentRootNode).Name}");

                var root = this as GfzTrackSegmentRootNode;
                return root;
            }
            else
            {
                // Recursive
                var parent = GetParent();
                var root = parent.GetRoot();
                return root;
            }
        }

        public TGfzTrackSegmentNode GetParent<TGfzTrackSegmentNode>() where TGfzTrackSegmentNode : GfzTrackSegmentNode
        {
            bool hasParent = transform.parent != null;
            if (hasParent)
            {
                var parentSegment = transform.parent.GetComponent<TGfzTrackSegmentNode>();
                return parentSegment;
            }
            else
                return null;
        }

        public GfzTrackSegmentNode GetParent()
        {
            var parent = GetParent<GfzTrackSegmentNode>();
            return parent;
        }

        public TGfzTrackSegmentNode[] GetChildren<TGfzTrackSegmentNode>() where TGfzTrackSegmentNode : GfzTrackSegmentNode
        {
            var children = new List<TGfzTrackSegmentNode>();
            foreach (var child in transform.GetChildren())
            {
                bool isActive = child.gameObject.activeInHierarchy;
                if (!isActive)
                    continue;

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
                : transform.localScale;

            // If GFZ, flip Z coordinate space
            if (useGfzCoordinateSpace)
            {
                position.z = -position.z;
                rotation.y = -rotation.y;
            }

            var matrix = Matrix4x4.TRS(position, Quaternion.Euler(rotation), scale);

            return matrix;
        }

        public HierarchichalAnimationCurveTRS GetParentHacTRS(Scope scope, bool isGfzCoordinateSpace, out bool isRoot)
        {
            var parent = GetParent();
            isRoot = parent == null;
            var parentHacTRS = isRoot ? null : parent.CreateHierarchichalAnimationCurveTRS(scope, isGfzCoordinateSpace);
            return parentHacTRS;
        }

        public byte GetBranchIndex()
        {
            // TODO: recurse up hierachy, stopping on branch node, getting ID
            return 0;
        }

        public TrackSegment[] CreateChildTrackSegments()
        {
            var children = GetChildren();
            var trackSegments = new TrackSegment[children.Length];
            for (int i = 0; i < trackSegments.Length; i++)
            {
                var trackSegment = children[i].CreateTrackSegment();
                trackSegments[i] = trackSegment;
            }
            return trackSegments;
        }

        public GfzTrackSegmentShapeNode[] GetShapeNodes()
        {
            var shapeNodes = new List<GfzTrackSegmentShapeNode>();
            var children = GetChildren();
            foreach (var child in children)
            {
                bool childIsShape = child is GfzTrackSegmentShapeNode;
                if (childIsShape)
                {
                    shapeNodes.Add(child as GfzTrackSegmentShapeNode);
                }

                var childShapeNodes = child.GetShapeNodes();
                shapeNodes.AddRange(childShapeNodes);
            }
            return shapeNodes.ToArray();
        }
    }
}
