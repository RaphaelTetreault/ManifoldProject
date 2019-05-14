using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public List<TrackTransform> trackTransforms = new List<TrackTransform>();

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

            // Let's build the transform after-the-fact
            var usedKeys = new List<int>();
            foreach (var node in trackNodes)
            {
                var transformAbsPtr = node.trackTransformAbsPtr;
                if (!usedKeys.Contains(transformAbsPtr))
                {
                    usedKeys.Add(transformAbsPtr);

                    reader.BaseStream.Seek(transformAbsPtr, SeekOrigin.Begin);
                    var value = new TrackTransform();
                    value.Deserialize(reader);
                    trackTransforms.Add(value);
                }
            }

            BinaryIoUtility.PopEndianess();
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
