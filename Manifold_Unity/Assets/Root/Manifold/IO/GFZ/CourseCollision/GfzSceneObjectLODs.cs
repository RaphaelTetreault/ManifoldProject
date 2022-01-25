﻿using GameCube.GFZ.CourseCollision;
using System;
using UnityEngine;

namespace Manifold.IO.GFZ.CourseCollision
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
