using GameCube.GFZ.Stage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public sealed class GfzTrackCheckpoints : MonoBehaviour
    {
        [SerializeField] private GfzTrackSegment segment;
        [Delayed]
        [SerializeField] private float metersPerCheckpoint = 10f;
        [SerializeField] private bool genCheckpoints = true;

        public GfzTrackSegment Segment => segment;

        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            if (segment == null)
            {
                segment = GetComponent<GfzTrackSegment>();
            }

            if (genCheckpoints)
            {
                var checkpoints = segment.CreateCheckpoints(false);

                int index = 0;
                foreach (var checkpoint in checkpoints)
                {
                    var gobj = new GameObject($"Checkpoint[{index++}]");
                    gobj.transform.parent = this.transform;
                    var script = gobj.AddComponent<GfzCheckpoint>();
                    script.Init(checkpoint);
                }
                genCheckpoints = false;
            }
        }

    }
}
