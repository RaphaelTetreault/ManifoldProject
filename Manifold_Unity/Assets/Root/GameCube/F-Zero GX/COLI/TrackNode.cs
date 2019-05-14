using StarkTools.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

namespace GameCube.FZeroGX.COLI_COURSE
{
    [Serializable]
    public class TrackNode : IBinarySerializable, IBinaryAddressable
    {

        #region MEMBERS

        [SerializeField, Hex] long startAddress;
        [SerializeField, Hex] long endAddress;

        [Hex(8), Space]
        public int trackBranchCount;
        [Hex(8)]
        public int trackPointAbsPtr;
        [Hex(8)]
        public int trackTransformAbsPtr;

        public TrackPoint point;
        public TrackTransform transform;

        #endregion

        #region PROPERTIES

        public long StartAddress
        {
            get => startAddress;
            set => startAddress = value;
        }

        public long EndAddress
        {
            get => endAddress;
            set => endAddress = value;
        }

        #endregion

        #region METHODS

        public void Deserialize(BinaryReader reader)
        {
            startAddress = reader.BaseStream.Position;

            reader.ReadX(ref trackBranchCount);
            reader.ReadX(ref trackPointAbsPtr);
            reader.ReadX(ref trackTransformAbsPtr);

            endAddress = reader.BaseStream.Position;

            // Get point
            reader.BaseStream.Seek(trackPointAbsPtr, SeekOrigin.Begin);
            reader.ReadX(ref point, true);

            // Get transform
            //reader.BaseStream.Seek(trackTransformAbsPtr, SeekOrigin.Begin);
            //reader.ReadX(ref transform, true);

            reader.BaseStream.Seek(endAddress, SeekOrigin.Begin);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(trackBranchCount);
            writer.WriteX(trackPointAbsPtr);
            writer.WriteX(trackTransformAbsPtr);
        }

        #endregion

    }
}
