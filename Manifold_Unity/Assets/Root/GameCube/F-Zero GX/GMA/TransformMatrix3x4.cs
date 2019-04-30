using GameCube.GX;
using StarkTools.IO;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace GameCube.FZeroGX.GMA
{
    [Serializable]
    public struct TransformMatrix3x4 : IBinarySerializable
    {
        public const int kBinarySize = 0x30;

        [SerializeField, LabelPrefix("00")]
        Vector3 row0;

        [SerializeField, LabelPrefix("0C")]
        Vector3 row1;

        [SerializeField, LabelPrefix("18")]
        Vector3 row2;

        [SerializeField, LabelPrefix("24")]
        Vector3 row3;

        public Vector3 Row0
        {
            get => row0;
            set => row0 = value;
        }

        public Vector3 Row1
        {
            get => row1;
            set => row1 = value;
        }

        public Vector3 Row2
        {
            get => row2;
            set => row2 = value;
        }

        public Vector3 Row3
        {
            get => row3;
            set => row3 = value;
        }

        public Vector4 Col0
        {
            get => new Vector4(row0.x, row1.x, row2.x, row3.x);
            set
            {
                row0.x = value.x;
                row1.x = value.y;
                row2.x = value.z;
                row3.x = value.w;
            }
        }

        public Vector4 Col1
        {
            get => new Vector4(row0.x, row1.x, row2.x, row3.x);
            set
            {
                row0.y = value.x;
                row1.y = value.y;
                row2.y = value.z;
                row3.y = value.w;
            }
        }

        public Vector4 Col2
        {
            get => new Vector4(row0.x, row1.x, row2.x, row3.x);
            set
            {
                row0.z = value.x;
                row1.z = value.y;
                row2.z = value.z;
                row3.z = value.w;
            }
        }

        public void Deserialize(BinaryReader reader)
        {
            reader.ReadX(ref row0);
            reader.ReadX(ref row1);
            reader.ReadX(ref row2);
            reader.ReadX(ref row3);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.WriteX(row0);
            writer.WriteX(row1);
            writer.WriteX(row2);
            writer.WriteX(row3);
        }
    }
}
