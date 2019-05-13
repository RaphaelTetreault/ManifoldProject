using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCube.FZeroGX.COLI_COURSE
{
    [Serializable]
    public class ColiScene : IBinarySerializable, IFile
    {
        [SerializeField] string name;

        public Header header;
        public TrackNode[] trackNodes;
        public TrackLength trackInformation;

        public string FileName
        {
            get => name;
            set => name = value;
        }

        public void Deserialize(BinaryReader reader)
        {
            BinaryIoUtility.PushEndianess(false);

            reader.ReadX(ref header, true);

            // 0x08
            reader.BaseStream.Seek(header.trackNodeAbsPtr, SeekOrigin.Begin);
            reader.ReadX(ref trackNodes, header.trackNodeCount, true);

            // 0x90
            reader.BaseStream.Seek(header.trackInfoAbsPtr, SeekOrigin.Begin);
            reader.ReadX(ref trackInformation, true);

            BinaryIoUtility.PopEndianess();
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
