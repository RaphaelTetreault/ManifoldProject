using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;


namespace Manifold.EditorTools.GC.GFZ.Stage
{
    [ExecuteInEditMode]
    public class CatmullRomPoint : MonoBehaviour
    {
        [SerializeField] private Mesh mesh;

        private void OnEnable()
        {
            SetReferenceInParent();
        }

        private void OnDisable()
        {
            SetReferenceInParent();
        }

        private GfzCatmullRomSegment GetParent()
        {
            var parent = GetComponentInParent<GfzCatmullRomSegment>();
            if (parent != null)
            {
                parent.CollectChildren();
            }
            else
            {
                throw new Exception($"{name} (type: {nameof(CatmullRomPoint)}) is not a child of a {nameof(GfzCatmullRomSegment)}!");
            }
            return parent;
        }

        public void SetReferenceInParent()
        {
            var parent = GetParent();
            parent.CollectChildren();
        }

        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            if (mesh == null)
            {
                mesh = Resources.GetBuiltinResource<Mesh>(EditorTools.Const.Resources.Cube);
            }
        }

        private void OnDrawGizmos()
        {
            bool isActive = Selection.Contains(gameObject);
            if (isActive)
            {
                // Get edges of dimensions from centerpoint
                var left = -transform.right * transform.localScale.x / 2f;
                var right = transform.right * transform.localScale.x / 2f;
                var up = transform.up * transform.localScale.y / 2f;
                var down = -transform.up * transform.localScale.y / 2f;
                var forward = transform.forward * transform.localScale.z / 2f;
                var backward = -transform.forward * transform.localScale.z / 2f;

                // Translate edges to global position
                left += transform.position;
                right += transform.position;
                up += transform.position;
                down += transform.position;
                forward += transform.position;
                backward += transform.position;

                // Draw lines
                Gizmos.color = Color.red;
                Gizmos.DrawLine(left, right);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(up, down);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(forward, backward);
            }
            else
            {
                Gizmos.color = new Color32(255, 255, 255, 64);
                Gizmos.DrawMesh(mesh, 0, transform.position, transform.rotation, Vector3.one * 5f);
            }
        }

    }
}
