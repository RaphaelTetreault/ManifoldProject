using Manifold.IO;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

namespace GameCube.GFZ.CourseCollision
{
    [Serializable]
    public class Header : IBinarySerializable
    {
        public const int kSizeOfZero0x28 = 0x20;
        public const int kSizeOfZero0xD8 = 0x24;
        public const int kGxHeaderSize = 0xFC;
        public const int kAxHeaderSize = 0xF8;

        #region MEMBERS

        public float unk_0x00;
        public float unk_0x04;
        [Hex(8)]
        public int trackNodeCount;
        [Hex(8)]
        public int trackNodeAbsPtr;
        public int unk_0x10;
        public int unk_0x14;
        public int unk_0x18;
        public int unk_0x1C;
        [Hex(8)]
        public int unk_0x20;
        [Hex(8)]
        public int headerSize;
        public byte[] zero_0x28;
        public int gameObjectCount;
        public int unk_0x4C; // One of these not in AX
        public int unk_0x50; // One of these not in AX
        public int gameObjectAbsPtr;
        public int unk_0x58;
        public int zero_0x5C;
        public int unk_0x60;
        public int unk_0x64;
        public int unk_0x68;
        public int unk_0x6C;
        public int unk_0x70;
        public int zero_0x74;
        public int zero_0x78;
        public int unk_0x7C;
        public int unk_0x80;
        public int unk_0x84;
        public int zero_0x88;
        public int zero_0x8C;
        [Hex("90", 8)]
        public int trackInfoAbsPtr;
        public int unk_0x94;
        public int unk_0x98;
        public int unk_0x9C;
        public int unk_0xA0;
        public int unk_0xA4;
        public int unk_0xA8;
        public int unk_0xAC;
        public int unk_0xB0;
        public int unk_0xB4;
        public int unk_0xB8;
        public int unk_0xBC;
        public int unk_0xC0;
        public int unk_0xC4;
        public int unk_0xC8;
        public int unk_0xCC;
        public int unk_0xD0;
        public int unk_0xD4;
        public byte[] zero_0xD8;

        #endregion

        #region PROPERTIES

        public bool IsGX => headerSize == kGxHeaderSize;
        public bool IsAX => headerSize == kAxHeaderSize;

        #endregion

        #region METHODS 

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref unk_0x00);
            reader.ReadX(ref unk_0x04);
            reader.ReadX(ref trackNodeCount);
            reader.ReadX(ref trackNodeAbsPtr);
            reader.ReadX(ref unk_0x10);
            reader.ReadX(ref unk_0x14);
            reader.ReadX(ref unk_0x18);
            reader.ReadX(ref unk_0x1C);
            reader.ReadX(ref unk_0x20);
            reader.ReadX(ref headerSize);
            reader.ReadX(ref zero_0x28, kSizeOfZero0x28);
            reader.ReadX(ref gameObjectCount);
            if (IsGX)
                reader.ReadX(ref unk_0x4C); ////////////
            reader.ReadX(ref unk_0x50);//////////
            reader.ReadX(ref gameObjectAbsPtr);
            reader.ReadX(ref unk_0x58);
            reader.ReadX(ref zero_0x5C);
            reader.ReadX(ref unk_0x60);
            reader.ReadX(ref unk_0x64);
            reader.ReadX(ref unk_0x68);
            reader.ReadX(ref unk_0x6C);
            reader.ReadX(ref unk_0x70);
            reader.ReadX(ref zero_0x74);
            reader.ReadX(ref zero_0x78);
            reader.ReadX(ref unk_0x7C);
            reader.ReadX(ref unk_0x80);
            reader.ReadX(ref unk_0x84);
            reader.ReadX(ref zero_0x88);
            reader.ReadX(ref zero_0x8C);
            reader.ReadX(ref trackInfoAbsPtr);
            reader.ReadX(ref unk_0x94);
            reader.ReadX(ref unk_0x98);
            reader.ReadX(ref unk_0x9C);
            reader.ReadX(ref unk_0xA0);
            reader.ReadX(ref unk_0xA4);
            reader.ReadX(ref unk_0xA8);
            reader.ReadX(ref unk_0xAC);
            reader.ReadX(ref unk_0xB0);
            reader.ReadX(ref unk_0xB4);
            reader.ReadX(ref unk_0xB8);
            reader.ReadX(ref unk_0xBC);
            reader.ReadX(ref unk_0xC0);
            reader.ReadX(ref unk_0xC4);
            reader.ReadX(ref unk_0xC8);
            reader.ReadX(ref unk_0xCC);
            reader.ReadX(ref unk_0xD0);
            reader.ReadX(ref unk_0xD4);
            reader.ReadX(ref zero_0xD8, kSizeOfZero0xD8);

            // Assert assumptions
            System.Diagnostics.Debug.Assert(zero_0x5C == 0);
            System.Diagnostics.Debug.Assert(zero_0x74 == 0);
            System.Diagnostics.Debug.Assert(zero_0x78 == 0);
            System.Diagnostics.Debug.Assert(zero_0x88 == 0);
            System.Diagnostics.Debug.Assert(zero_0x8C == 0);

            foreach (var zero in zero_0x28)
                System.Diagnostics.Debug.Assert(zero == 0);
            foreach (var zero in zero_0xD8)
                System.Diagnostics.Debug.Assert(zero == 0);
        }

        public void Serialize(BinaryWriter writer)
        {

        }

        #endregion

    }
}
