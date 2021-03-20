using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
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
            foreach (var unknownStruct2 in sceneSobj.Value.unknownTrigger2s)
            {
                Gizmos.DrawWireSphere(unknownStruct2.position, size);
            }
        }
    }
}
