using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manifold.IO;
using Manifold.IO.GFZ;
using Manifold.IO.GFZ.CourseCollision;

namespace Manifold
{
    public class TempViewUnknownStruct2 : MonoBehaviour
    {
        public ColiSceneSobj sceneSobj;
        public Color color = Color.magenta;
        public float size = 10f;

        private void OnDrawGizmos()
        {
            if (sceneSobj == null)
                return;

            Gizmos.color = color;
            foreach (var unk2 in sceneSobj.Value.unknownStruct2s)
            {
                Gizmos.DrawWireSphere(unk2.unk_0x04, size);
            }
        }
    }
}
