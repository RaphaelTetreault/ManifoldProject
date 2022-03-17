using GameCube.GFZ.Stage;
using System;
using UnityEngine;

namespace Manifold.EditorTools.GC.GFZ.Stage
{
    public class GfzSceneObjectLODs : MonoBehaviour,
        IGfzConvertable<SceneObjectLOD[]>,
        IEquatable<GfzSceneObjectLODs>
    {
        [System.Serializable]
        private struct GfzLOD : IEquatable<GfzLOD>
        {
            //public MeshFilter model; // <-- ideal to have at some point?
            public string modelName;
            public float lodDistance;

            public bool Equals(GfzLOD other)
            {
                return
                    other.lodDistance == lodDistance &&
                    other.modelName == modelName;
            }
        }


        [SerializeField] private GfzLOD[] levelOfDetails;

        public SceneObjectLOD[] ExportGfz()
        {
            IO.Assert.IsTrue(levelOfDetails.Length > 0);

            var sceneObjectLODs = new SceneObjectLOD[levelOfDetails.Length];
            for (int i = 0; i < sceneObjectLODs.Length; i++)
            {
                var levelOfDetail = levelOfDetails[i];
                sceneObjectLODs[i] = new SceneObjectLOD()
                {
                    Name = levelOfDetail.modelName,
                    LodDistance = levelOfDetail.lodDistance,
                };
            }
            return sceneObjectLODs;
        }

        public void ImportGfz(SceneObjectLOD[] sceneObjectLODs)
        {
            IO.Assert.IsTrue(sceneObjectLODs.Length > 0);

            levelOfDetails = new GfzLOD[sceneObjectLODs.Length];
            for (int i = 0; i < levelOfDetails.Length; i++)
            {
                var sceneObjectLOD = sceneObjectLODs[i];
                levelOfDetails[i] = new GfzLOD()
                {
                    modelName = sceneObjectLOD.Name,
                    lodDistance = sceneObjectLOD.LodDistance,
                };
            }
        }

        public bool Equals(GfzSceneObjectLODs other)
        {
            // array must be same length
            if (other.levelOfDetails.Length != levelOfDetails.Length)
                return false;

            // All values must equate
            for (int i = 0; i < levelOfDetails.Length; i++)
            {
                var isSame = other.levelOfDetails[i].Equals(levelOfDetails[i]);
                if (!isSame)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
