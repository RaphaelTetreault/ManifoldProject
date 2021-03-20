using UnityEngine;
using GameCube.GFZ.CourseCollision;

namespace Manifold.IO.GFZ.CourseCollision
{
    public class DisplayTrigger : MonoBehaviour, IColiCourseDisplayable
    {
        [SerializeField]
        private ColiSceneSobj sceneSobj;

        public Comp
            compX = Comp.x,
            compY = Comp.y,
            compZ = Comp.z;

        public bool doInvertX = false;

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
            var red = new Color32(255, 0, 0, 127);
            var yellow = new Color32(255, 255, 0, 127);
            var green = new Color32(0, 255, 0, 127);
            var blue = new Color32(0, 0, 255, 127);

            //DisplayTriggerArray(sceneSobj.Value.arcadeCheckpointTriggers, blue);

            if (displayObjectGroup)
                DisplayTriggerArray(sceneSobj.Value.unknownTriggers_0x94, green);

            //if (displayEffectVolume)
            //    DisplayTriggerArray(sceneSobj.Value.effectTriggers, red, true);

        }

        public void DisplayTriggerArray(UnknownTrigger1[] unknownStruct, Color32 color, bool drawNumber = false)
        {
            var mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            Gizmos.color = color;

            var style = new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
                fontSize = 32,
            };

            int count = 0;
            foreach (var item in unknownStruct)
            {
                count++;

                var position = new Vector3(item.position.x, item.position.y, item.position.z);
                //var rotation = Quaternion.Euler(item.rotation.x, item.rotation.y, item.rotation.z);
                //var rotation = RemixQuaternion(item.rotation, compX, compY, compZ);

                Gizmos.DrawMesh(mesh, 0, item.position, item.Rotation, item.scale);
                Gizmos.DrawWireMesh(mesh, 0, item.position, item.Rotation, item.scale);

                //if (drawNumber)
                UnityEditor.Handles.Label(position, count.ToString(), style);
            }
        }

        public enum Comp
        {
            x, y, z,
            nx, ny, nz,
        }

        public Quaternion RemixQuaternion(Vector3 v3, Comp compX, Comp compY, Comp compZ)
        {
            return Quaternion.Euler(
                GetComp(v3, compX),
                GetComp(v3, compY),
                GetComp(v3, compZ)
                );
        }

        public float GetComp(Vector3 v3, Comp comp)
        {
            switch (comp)
            {
                case Comp.x:
                    return v3.x;
                case Comp.y:
                    return v3.y;
                case Comp.z:
                    return v3.z;

                case Comp.nx:
                    return -v3.x;
                case Comp.ny:
                    return -v3.y;
                case Comp.nz:
                    return -v3.z;
                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}
