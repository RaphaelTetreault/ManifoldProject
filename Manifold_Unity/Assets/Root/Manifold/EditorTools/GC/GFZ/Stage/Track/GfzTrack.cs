using GameCube.GFZ.Stage;
using Manifold;
using Manifold.IO;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    /// <summary>
    /// Entry point for collecting and generating track data.
    /// </summary>
    public sealed class GfzTrack : MonoBehaviour
    {
        [field: SerializeField] public GfzTrackSegmentRootNode FirstRoot { get; private set; }
        [field: SerializeField] public GfzTrackSegmentRootNode[] AllRoots { get; private set; }
        [field: SerializeField] public bool DoFind { get; private set; }

        public TrackMinHeight TrackMinHeight { get; private set; }
        public TrackLength TrackLength { get; private set; }
        public TrackSegment[] RootSegments { get; private set; }
        public TrackSegment[] AllSegments { get; private set; }
        public Checkpoint[] Checkpoints { get; private set; }
        public TrackNode[] TrackNodes { get; private set; }
        public EmbeddedTrackPropertyArea[] EmbeddedPropertyAreas { get; private set; }
        public CheckpointGrid TrackCheckpointMatrix { get; private set; }
        public GridXZ TrackCheckpointMatrixBoundsXZ { get; private set; }
        public CircuitType CircuitType { get; private set; }


        public void InitTrackData()
        {
            FindChildSegments();
            AssignContinuity();

            if (this.AllRoots.Length == 0)
                throw new MissingReferenceException($"No references to any {typeof(GfzTrackSegmentNode).Name}! Make sure references existin in inspector.");

            foreach (var seg in this.AllRoots)
            {
                if (seg == null)
                {
                    throw new Exception($"Segment in {name} is null! Cannot export null segments.");
                }

                if (!seg.isActiveAndEnabled)
                {
                    throw new Exception($"Segment {seg.name} is disabled! Can only export active segments.");
                }
            }

            // Track metadata
            var trackMinHeight = new TrackMinHeight();
            var trackLength = new TrackLength();

            // Track Animation Data
            // Get all track segments as GFZ values
            var rootSegments = new List<TrackSegment>();
            // Get all of these segments in an order proper for serialization
            var allSegments = new List<TrackSegment>();

            // TrackNodes, Checkpoints
            var allCheckpointsList = new List<Checkpoint>();
            var trackNodes = new List<TrackNode>();

            // AI data
            var trackEmbeddedPropertyAreas = new List<EmbeddedTrackPropertyArea>();


            // Get all ROOT track segments
            foreach (var rootTrackSegmentNode in AllRoots)
            {
                var rootTrackSegment = rootTrackSegmentNode.CreateTrackSegment();
                rootSegments.Add(rootTrackSegment);

                // Get segments in proper order for binary serialization
                var trackSegmentHierarchy = rootTrackSegment.GetGraphSerializableOrder();
                allSegments.AddRange(trackSegmentHierarchy);

                // CREATE CHECKPOINTS
                var checkpointGenerators = rootTrackSegmentNode.GetComponentsInChildren<GfzCheckpointGenerator>();
                // For now, should only have 1
                if (checkpointGenerators.Length == 1)
                {
                    var msg =
                        $"Track segment hierachy does not have exactly 1 {nameof(GfzCheckpointGenerator)} in it " +
                        $"(it has {checkpointGenerators.Length}). Make sure it has exactly 1!";
                    throw new ArgumentException(msg);
                }

                // Get all checkpoints for this segment
                var checkpoints = CheckpointUtility.CreateCheckpoints(checkpointGenerators[0], true);
                allCheckpointsList.AddRange(checkpoints); // checkpoints flat
                // Create TrackNodes
                foreach (var checkpoint in checkpoints)
                {
                    var trackNode = new TrackNode()
                    {
                        // Add checkpoints. TODO: support branching
                        Checkpoints = new Checkpoint[] { checkpoint },
                        RootSegment = rootTrackSegment,
                    };

                    // Add to master list
                    trackNodes.Add(trackNode);
                }

                // TODO: interface
                //var embededPropertyAreas = rootSegment.GetEmbededPropertyAreas();
                //trackEmbeddedPropertyAreas.AddRange(embededPropertyAreas);

                // Compute some metadata based on segments
                trackLength.Value += rootTrackSegmentNode.GetSegmentLength();
            }

            trackEmbeddedPropertyAreas = new List<EmbeddedTrackPropertyArea>();
            trackEmbeddedPropertyAreas.Add(EmbeddedTrackPropertyArea.Terminator());

            //// Set circuit type depending on if 
            //var lastSegmentIndex = this.rootSegments.Length - 1;
            //CircuitType = this.rootSegments[lastSegmentIndex].Segment.NextSegment != null
            //    ? CircuitType.OpenCircuit
            //    : CircuitType.ClosedCircuit;

            // TODO: name this something better
            var allCheckpoints = allCheckpointsList.ToArray();
            trackMinHeight.SetMinHeight(allCheckpoints);

            //
            var checkpointMatrixBoundsXZ = CheckpointGrid.GetMatrixBoundsXZ(allCheckpoints);
            var trackCheckpointMatrix = new CheckpointGrid();
            trackCheckpointMatrix.GenerateIndexes(checkpointMatrixBoundsXZ, allCheckpoints);

            //
            TrackMinHeight = trackMinHeight;
            TrackLength = trackLength;
            RootSegments = rootSegments.ToArray();
            AllSegments = allSegments.ToArray();
            Checkpoints = allCheckpoints;
            TrackNodes = trackNodes.ToArray();
            EmbeddedPropertyAreas = trackEmbeddedPropertyAreas.ToArray();
            TrackCheckpointMatrix = trackCheckpointMatrix;
            TrackCheckpointMatrixBoundsXZ = checkpointMatrixBoundsXZ;
            // TEMP: because it caused me so much strife hunting a bug before...
            CircuitType = CircuitType.ClosedCircuit;
        }

        public void FindChildSegments()
        {
            FirstRoot = GetComponentInChildren<GfzTrackSegmentRootNode>(false);
            AllRoots = GetAllRootSegments();
            Assert.IsTrue(FirstRoot is not null);
            Assert.IsTrue(FirstRoot == AllRoots[0]);
        }

        public void AssignContinuity()
        {
            var allRootSegments = GetAllRootSegments();

            bool hasSegments = allRootSegments.Length > 0;
            if (!hasSegments)
                return;

            for (int i = 0; i < allRootSegments.Length - 1; i++)
            {
                var curr = allRootSegments[i + 0];
                var next = allRootSegments[i + 1];

                curr.Next = next;
                next.Prev = curr;
            }

            // Patch the first/last indexes which wrap around
            int lastIndex = allRootSegments.Length - 1;
            allRootSegments[0].Prev = allRootSegments[lastIndex];
            allRootSegments[lastIndex].Next = allRootSegments[0];
        }

        public GfzTrackSegmentRootNode[] GetAllRootSegments()
        {
            var rootSegments = new List<GfzTrackSegmentRootNode>();
            foreach (var child in transform.GetChildren())
            {
                var isActive = child.gameObject.activeSelf;
                if (!isActive)
                    continue;

                var rootSegment = child.GetComponent<GfzTrackSegmentRootNode>();
                var exists = rootSegment != null;
                if (exists)
                    rootSegments.Add(rootSegment);
            }
            return rootSegments.ToArray();
        }

        private void OnValidate()
        {
            if (DoFind)
            {
                FindChildSegments();
                AssignContinuity();
                DoFind = false;
            }
        }

    }
}
