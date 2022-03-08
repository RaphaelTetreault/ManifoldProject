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
        internal static class Tooltips
        {
            public const string samplesSizeInBytes =
                "The amount of disk space the amount of samples for this curve will take. " +
                "Includes animation data for Position.XYZ and Rotation.XY. Scale.XYZ and " +
                "Rotation.Z are independent of this data.";
        }


        [SerializeField] private Mesh mesh;

        [Min(1)]
        [SerializeField]
        private int samples = 32;

        [Tooltip(Tooltips.samplesSizeInBytes)]
        [ReadOnlyGUI]
        [SerializeField]
        private string samplesSizeInBytes;

        [SerializeField]
        private bool ignoreWidth = false;

        [Min(1)]
        [SerializeField]
        private float width = 64;


        [SerializeField]
        private bool ignoreRoll = false;

        [SerializeField]
        private float roll = 0f;




        public int Samples => samples;
        public float Width => width;
        public float Roll => roll;
        public bool IgnoreWidth => ignoreWidth;
        public bool IgnoreRoll => ignoreRoll;

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

            const int sizeofKeyableAttributes = 0x20;
            const int numKeyableAttributeCurves = 5; // Pos.XYZ, Rot.XY
            int size = samples * sizeofKeyableAttributes * numKeyableAttributeCurves;
            samplesSizeInBytes = size.ToString("n0");
        }

        private void OnDrawGizmos()
        {
            bool isActive = Selection.Contains(gameObject);
            if (isActive)
            {
                //// Get edges of dimensions from centerpoint
                //var left = -transform.right * width / 2f;
                //var right = transform.right * width / 2f;
                //var up = transform.up * transform.localScale.y / 2f;
                //var down = -transform.up * transform.localScale.y / 2f;
                //var forward = transform.forward * transform.localScale.z / 2f;
                //var backward = -transform.forward * transform.localScale.z / 2f;

                //// Translate edges to global position
                //left += transform.position;
                //right += transform.position;
                //up += transform.position;
                //down += transform.position;
                //forward += transform.position;
                //backward += transform.position;

                //// Draw lines
                //Gizmos.color = Color.red;
                //Gizmos.DrawLine(left, right);
                //Gizmos.color = Color.green;
                //Gizmos.DrawLine(up, down);
                //Gizmos.color = Color.blue;
                //Gizmos.DrawLine(forward, backward);
            }
            else
            {
                Gizmos.color = new Color32(255, 255, 255, 128);
                Gizmos.DrawMesh(mesh, 0, transform.position, transform.rotation, Vector3.one * 5f);
            }
        }

    }
}
