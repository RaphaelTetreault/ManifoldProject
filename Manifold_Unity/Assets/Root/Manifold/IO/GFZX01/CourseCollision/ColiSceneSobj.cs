using GameCube.GFZX01.CourseCollision;
using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;

namespace Manifold.IO.GFZX01.CourseCollision
{
    [CreateAssetMenu(menuName = MenuConst.GFZX01_CourseCollision + "COLI Scene")]
    public class ColiSceneSobj : ScriptableObject, IBinarySerializable, IFile
    {
        public string fileName;
        public string filePath;
        public ColiScene scene;

        public string FileName
        {
            get => fileName;
            set => fileName = value;
        }

        public string FilePath
        {
            get => filePath;
            set => filePath = value;
        }

        public void Deserialize(BinaryReader reader)
        {
            scene.Deserialize(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
