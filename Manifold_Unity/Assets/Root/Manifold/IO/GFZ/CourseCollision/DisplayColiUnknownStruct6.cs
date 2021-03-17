using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    public class DisplayColiUnknownStruct6 : MonoBehaviour, IColiCourseDisplayable
    {
        [SerializeField]
        private ColiSceneSobj sceneSobj;

        public bool
            displayEffectVolume = true,
            displayObjectGroup = true;

        public ColiSceneSobj SceneSobj
        {
            get => sceneSobj;
            set => sceneSobj = value;
        }



        private void OnDrawGizmos()
        {
            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            var red = new Color32(255, 0, 0, 127);
            var green = new Color32(0, 255, 0, 127);
            var blue = new Color32(0, 0, 255, 127);

            Gizmos.color = blue;
            foreach (var item in sceneSobj.Value.arcadeCheckpoints)
            {
                var rotation = Quaternion.Euler(item.rotation.z, item.rotation.y, item.rotation.x);
                Gizmos.DrawMesh(mesh, 0, item.position, rotation, item.scaleOrRotation * 10f);
            }

            if (displayObjectGroup)
            {
                Gizmos.color = green;
                foreach (var item in sceneSobj.Value.unknownStruct6_0x94)
                {
                    var rotation = Quaternion.Euler(item.rotation.x, item.rotation.y, item.rotation.z);
                    Gizmos.DrawMesh(mesh, 0, item.position, rotation, item.scaleOrRotation * 10f);
                }
            }

            if (displayEffectVolume)
            {
                Gizmos.color = red;
                foreach (var item in sceneSobj.Value.unknownStruct6_0x9C)
                {
                var rotation = Quaternion.Euler(item.rotation.x, item.rotation.y, item.rotation.z);
                    Gizmos.DrawMesh(mesh, 0, item.position, rotation, item.scaleOrRotation * 10f);
                }
            }
        }
    }
}
