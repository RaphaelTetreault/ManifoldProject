using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    public class TempDisplayUnknownTrigger1 : MonoBehaviour
    {
        public Color gizmosColor = new Color32(0, 255, 0, 128);
        public EnumFlags16 unk1;
        public EnumFlags16 unk2;

        private void OnDrawGizmosSelected()
        {
            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            Gizmos.color = gizmosColor;
            Gizmos.DrawMesh(mesh, 0, transform.position, transform.rotation, transform.localScale);
            Gizmos.DrawWireMesh(mesh, 0, transform.position, transform.rotation, transform.localScale);
        }
    }
}
