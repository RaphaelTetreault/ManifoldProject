using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    public class DisplayColiUnknownStruct6 : MonoBehaviour, IColiCourseDisplayable
    {
        [SerializeField]
        private ColiSceneSobj sceneSobj;

        public ColiSceneSobj SceneSobj
        {
            get => sceneSobj;
            set => sceneSobj = value;
        }



        private void OnDrawGizmos()
        {
            var red = new Color32(255, 0, 0, 127);
            var green = new Color32(0, 255, 0, 127);
            var blue = new Color32(0, 0, 255, 127);

            Gizmos.color = blue;
            foreach (var item in sceneSobj.Value.arcadeCheckpoints)
            {
                Gizmos.DrawCube(item.position, item.scaleOrRotation * 10f);
            }

            Gizmos.color = green;
            foreach (var item in sceneSobj.Value.unknownStruct6_0x94)
            {
                Gizmos.DrawCube(item.position, item.scaleOrRotation * 10f);
            }

            Gizmos.color = red;
            foreach (var item in sceneSobj.Value.unknownStruct6_0x9C)
            {
                Gizmos.DrawCube(item.position, item.scaleOrRotation * 10f);
            }
        }
    }
}
