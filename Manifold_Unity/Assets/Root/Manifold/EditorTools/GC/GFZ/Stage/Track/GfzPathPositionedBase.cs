using System.Collections;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage.Track
{
    public abstract class GfzPathPositionedBase : GfzUpdateable
    {
        [SerializeField] protected GfzSegmentNode segment;
        [SerializeField] protected bool autoUpdate = true;
        [SerializeField] protected GameObject prefab;
        [SerializeField] protected Vector3 positionOffset;
        [SerializeField] protected Vector3 rotationOffset;
        [SerializeField] protected Vector3 scaleOffset = Vector3.one;
        private bool waitForDelete = false;

        public GfzSegmentNode SegmentNode => segment;

        public abstract void CreateObjects();

        public void DeleteObjects()
        {
            foreach (var child in transform.GetChildren())
            {
                if (!child.gameObject.activeSelf)
                    continue;

                StartCoroutine(DestroyGameObject(child.gameObject));
            }
        }

        public override void OnUpdate()
        {
            var hasNoSegment = segment == null;
            var hasNoPrefab = prefab == null;
            var dontUpdate = !autoUpdate;
            if (hasNoSegment || hasNoPrefab || dontUpdate)
                return;

            CreateObjects();
        }

        protected override void OnValidate()
        {
            if (segment == null)
                segment = GetComponentInParent<GfzSegmentNode>();

            base.OnValidate();
        }

        // Hack from https://forum.unity.com/threads/onvalidate-and-destroying-objects.258782/
        IEnumerator DestroyGameObject(GameObject go)
        {
            yield return new WaitForSeconds(0);
            DestroyImmediate(go);
        }
    }
}