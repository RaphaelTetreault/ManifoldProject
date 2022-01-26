using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public abstract class GfzTrackSegment : MonoBehaviour
    {
        // Define delegates
        public delegate void OnEditCallback(GfzTrackSegment value);

        // Events
        public event OnEditCallback onEdit;

        // Fields
        [Header("Track Segment")]
        [SerializeField] protected GfzTrackSegment prev;
        [SerializeField] protected GfzTrackSegment next;

        [Header("Track Curves")]
        [SerializeField] protected AnimationCurve3 position = new AnimationCurve3();
        [SerializeField] protected AnimationCurve3 rotation = new AnimationCurve3();
        [SerializeField] protected AnimationCurve3 scale = new AnimationCurve3();

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

        // Methods
        public virtual void OnValidate()
        {
            onEdit?.Invoke(this);
        }
    }
}
