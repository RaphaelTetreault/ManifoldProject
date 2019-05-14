using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCube.FZeroGX.COLI_COURSE
{
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
