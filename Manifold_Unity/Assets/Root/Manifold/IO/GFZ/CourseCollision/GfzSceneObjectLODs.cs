using GameCube.GFZ.CourseCollision;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
{
    public class GfzSceneObjectLODs : MonoBehaviour,
        IGfzConvertable<SceneObjectLOD[]>
    {
        [System.Serializable]
        private struct GfzLOD
        {
            public string modelName;
            //public MeshFilter model; // <-- ideal to have at some point
            public float lodDistance;
        }

        [SerializeField] private GfzLOD[] levelOfDetails;


        public SceneObjectLOD[] ExportGfz()
        {
            Assert.IsTrue(levelOfDetails.Length > 0);

            var sceneObjectLODs = new SceneObjectLOD[levelOfDetails.Length];
            for (int i = 0; i < sceneObjectLODs.Length; i++)
            {
                var levelOfDetail = levelOfDetails[i];
                sceneObjectLODs[i] = new SceneObjectLOD()
                {
                    name = levelOfDetail.modelName,
                    lodDistance = levelOfDetail.lodDistance,
                };
            }
            return sceneObjectLODs;
        }

        public void ImportGfz(SceneObjectLOD[] sceneObjectLODs)
        {
            Assert.IsTrue(sceneObjectLODs.Length > 0);

            levelOfDetails = new GfzLOD[sceneObjectLODs.Length];
            for (int i = 0; i < levelOfDetails.Length; i++)
            {
                var sceneObjectLOD = sceneObjectLODs[i];
                levelOfDetails[i] = new GfzLOD()
                {
                    modelName = sceneObjectLOD.name,
                    lodDistance = sceneObjectLOD.lodDistance,
                };
            }
        }
    }
}
